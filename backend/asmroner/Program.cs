using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Backend.Asmoner.Services;
using Backend.Asmoner.Data;
using Backend.Asmoner.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

// Simple manual CLI parsing: commands: listen, search <q>, download <id>, sync
if (args.Length > 0)
{
    var cmd = args[0].ToLowerInvariant();
    if (cmd == "listen")
    {
        var port = 9999;
        for (int i = 1; i < args.Length; i++)
        {
            if ((args[i] == "-p" || args[i] == "--port") && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out var p)) port = p;
            }
        }

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddJsonOptions(opt => { opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; });
        builder.Services.AddSingleton<FolderScanner>();
        builder.Services.AddSingleton<DownloadService>();
        // register DbContext
        builder.Services.AddDbContext<AppDbContext>();
        var app = builder.Build();

        app.MapGet("/", () => Results.Text("ASM R C# Backend - minimal listen"));
        app.MapGet("/api/list", (FolderScanner scanner, int page, int pageSize) =>
        {
            var (infos, total) = scanner.GetPage(page <= 0 ? 1 : page, pageSize <= 0 ? 20 : pageSize);
            var data = new { infos = infos, total = total, page = page, pageSize = pageSize };
            return Results.Json(new { code = 200, msg = "", data = data });
        });

        app.MapGet("/api/search", async (AppDbContext db, string q, int page, int pageSize) =>
        {
            if (string.IsNullOrEmpty(q)) return Results.Json(new { code = 400, msg = "query required" });
            var pp = page <= 0 ? 1 : page;
            var ps = pageSize <= 0 ? 20 : pageSize;
            var query = db.MetadataWorks.AsQueryable();
            query = query.Where(m => (m.Title != null && EF.Functions.Like(m.Title, $"%{q}%")) || (m.Name != null && EF.Functions.Like(m.Name, $"%{q}%")) || (m.Tags != null && EF.Functions.Like(m.Tags, $"%{q}%")));
            var total = await query.CountAsync();
            var items = await query.Skip((pp - 1) * ps).Take(ps).ToListAsync();
            return Results.Json(new { code = 200, msg = "", data = new { items = items, total = total, page = pp, pageSize = ps } });
        });

        // Proxy search to remote API if Authorization present, otherwise use local DB
        app.MapGet("/api/works", async (HttpRequest req, AppDbContext db, int page, int pageSize) =>
        {
            var pp = page <= 0 ? 1 : page;
            var ps = pageSize <= 0 ? 20 : pageSize;
            var auth = req.Headers.ContainsKey("Authorization") ? req.Headers["Authorization"].ToString() : null;
            if (!string.IsNullOrEmpty(auth))
            {
                // forward to remote works endpoint
                using var http = new HttpClient();
                if (!string.IsNullOrEmpty(auth)) http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
                var remoteUrl = $"https://api.asmr-300.com/api/works?page={pp}&pageSize={ps}";
                var resp = await http.GetAsync(remoteUrl);
                var body = await resp.Content.ReadAsStringAsync();
                req.HttpContext.Response.StatusCode = (int)resp.StatusCode;
                var ct = resp.Content.Headers.ContentType?.ToString() ?? "application/json";
                return Results.Content(body, ct);
            }

            // local DB fallback
            var query = db.MetadataWorks.AsQueryable();
            var total = await query.CountAsync();
            var items = await query.Skip((pp - 1) * ps).Take(ps).ToListAsync();
            return Results.Json(new { code = 200, msg = "", data = new { works = items, total = total, page = pp, pageSize = ps } });
        });

        // remote-proxy search: forwards query and auth header
        app.MapGet("/api/search/remote", async (HttpRequest req, string q, int page, int pageSize) =>
        {
            if (string.IsNullOrEmpty(q)) return Results.Json(new { code = 400, msg = "query required" });
            var auth = req.Headers.ContainsKey("Authorization") ? req.Headers["Authorization"].ToString() : null;
            using var http = new HttpClient();
            if (!string.IsNullOrEmpty(auth)) http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
            var pp = page <= 0 ? 1 : page;
            var ps = pageSize <= 0 ? 20 : pageSize;
            var remoteUrl = $"https://api.asmr-300.com/api/search?q={Uri.EscapeDataString(q)}&page={pp}&pageSize={ps}";
            var resp = await http.GetAsync(remoteUrl);
            var body = await resp.Content.ReadAsStringAsync();
            req.HttpContext.Response.StatusCode = (int)resp.StatusCode;
            var ct = resp.Content.Headers.ContentType?.ToString() ?? "application/json";
            return Results.Content(body, ct);
        });

        // Proxy authentication endpoint to remote API (accepts form-urlencoded name/password)
        app.MapPost("/api/auth/me", async (HttpRequest req) =>
        {
            try
            {
                var form = await req.ReadFormAsync();
                var name = form["name"].FirstOrDefault() ?? form["username"].FirstOrDefault();
                var password = form["password"].FirstOrDefault() ?? form["pwd"].FirstOrDefault();
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                    return Results.Json(new { code = 400, msg = "name and password required" });

                using var http = new HttpClient();
                var payload = new[] {
                    new KeyValuePair<string,string>("name", name),
                    new KeyValuePair<string,string>("password", password)
                };
                var content = new FormUrlEncodedContent(payload);
                var remoteUrl = "https://api.asmr-300.com/api/auth/me";
                var resp = await http.PostAsync(remoteUrl, content);
                var body = await resp.Content.ReadAsStringAsync();
                var ct = resp.Content.Headers.ContentType?.ToString() ?? "application/json";
                req.HttpContext.Response.StatusCode = (int)resp.StatusCode;
                return Results.Content(body, ct);
            }
            catch (Exception ex)
            {
                return Results.Json(new { code = 500, msg = ex.Message });
            }
        });

        // trigger download (transitional: runs existing asmroner executable)
        app.MapPost("/api/download", async (HttpRequest req, DownloadService dl) =>
        {
            try
            {
                var body = await System.Text.Json.JsonDocument.ParseAsync(req.Body);
                if (!body.RootElement.TryGetProperty("mediaId", out var mid))
                    return Results.Json(new { code = 400, msg = "mediaId required" });
                var mediaId = mid.GetString();
                if (string.IsNullOrEmpty(mediaId)) return Results.Json(new { code = 400, msg = "mediaId required" });
                var ok = await dl.StartDownloadAsync(mediaId, (line) => Console.WriteLine(line));
                return Results.Json(new { code = ok ? 200 : 500, msg = ok ? "started" : "failed" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { code = 500, msg = ex.Message });
            }
        });

        // download status
        app.MapGet("/api/download/status", (HttpRequest req, DownloadService dl, string mediaId) =>
        {
            if (string.IsNullOrEmpty(mediaId)) return Results.Json(new { code = 400, msg = "mediaId required" });
            var running = dl.IsRunning(mediaId);
            return Results.Json(new { code = 200, msg = "", data = new { mediaId = mediaId, running = running } });
        });

        // cancel download
        app.MapPost("/api/download/cancel", async (HttpRequest req, DownloadService dl) =>
        {
            try
            {
                var body = await System.Text.Json.JsonDocument.ParseAsync(req.Body);
                if (!body.RootElement.TryGetProperty("mediaId", out var mid))
                    return Results.Json(new { code = 400, msg = "mediaId required" });
                var mediaId = mid.GetString();
                if (string.IsNullOrEmpty(mediaId)) return Results.Json(new { code = 400, msg = "mediaId required" });
                var ok = dl.Cancel(mediaId);
                return Results.Json(new { code = ok ? 200 : 500, msg = ok ? "canceled" : "not found" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { code = 500, msg = ex.Message });
            }
        });

        app.MapControllers();

        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{port}");
        Console.WriteLine($"Starting listen on port {port}");
        await app.RunAsync();
        return 0;
    }
    else if (cmd == "search")
    {
        if (args.Length <= 1)
        {
            Console.WriteLine("Search requires query argument");
            return 1;
        }
        var q = string.Join(' ', args.Skip(1));
        Console.WriteLine($"Search requested: {q}");
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            var query = db.MetadataWorks.AsQueryable();
            query = query.Where(m => (m.Title != null && EF.Functions.Like(m.Title, $"%{q}%")) || (m.Name != null && EF.Functions.Like(m.Name, $"%{q}%")) || (m.Tags != null && EF.Functions.Like(m.Tags, $"%{q}%")));
            var items = query.OrderByDescending(m => m.UpdatedAt).Take(50).ToList();
            Console.WriteLine($"Found: {items.Count}");
            foreach (var it in items)
            {
                Console.WriteLine($"{it.ID}\t{it.SourceID}\t{it.Title}");
            }
        }
        return 0;
    }
    else if (cmd == "download")
    {
        if (args.Length > 1) Console.WriteLine($"Download requested: {args[1]}");
        else Console.WriteLine("Download requires mediaId argument");
        Console.WriteLine("Not implemented: download engine.");
        return 0;
    }
    else if (cmd == "sync")
    {
        Console.WriteLine("Sync requested: scanning folders and updating metadata DB");
        var scanner = new FolderScanner();
        var all = new List<FolderInfo>();
        int page = 1; int pageSize = 500;
        while (true)
        {
            var (list, total) = scanner.GetPage(page, pageSize);
            if (list == null || list.Count == 0) break;
            all.AddRange(list);
            if (all.Count >= total) break;
            page++;
        }

        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            int inserted = 0, updated = 0;
            foreach (var f in all)
            {
                if (string.IsNullOrEmpty(f.MediaId)) continue;
                var existing = db.MetadataWorks.FirstOrDefault(m => m.SourceID == f.MediaId);
                if (existing == null)
                {
                    var nw = new MetadataWork
                    {
                        Title = f.Title ?? f.Name,
                        Name = f.Name,
                        SourceID = f.MediaId,
                        CreateDate = f.Date,
                        UpdatedAt = DateTime.UtcNow
                    };
                    db.MetadataWorks.Add(nw);
                    inserted++;
                }
                else
                {
                    existing.Title = f.Title ?? f.Name;
                    existing.Name = f.Name;
                    existing.CreateDate = f.Date;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updated++;
                }
            }
            db.SaveChanges();
            Console.WriteLine($"Sync complete. Inserted={inserted} Updated={updated} TotalScanned={all.Count}");
        }
        return 0;
    }
}

// no args: show simple help
Console.WriteLine("asmroner (C#) - minimal replacement\nCommands:\n  listen [-p port]\n  search <query>\n  download <mediaId>\n  sync");
return 0;
