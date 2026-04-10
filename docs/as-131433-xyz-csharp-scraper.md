# C# 抓取 as.131433.xyz 页面文本示例
本文档展示如何使用 C# 抓取 `https://as.131433.xyz/` 的页面内容。
## 完整示例代码
```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
class As131433Scraper
{
    private static readonly HttpClient HttpClient = new HttpClient();
    static async Task Main(string[] args)
    {
        const string url = "https://as.131433.xyz/";
        
        try
        {
            // 设置请求头（模拟浏览器）
            HttpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            HttpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
            
            // 设置超时
            HttpClient.Timeout = TimeSpan.FromSeconds(30);
            // 发送 GET 请求
            var response = await HttpClient.GetAsync(url);
            
            // 确保请求成功
            response.EnsureSuccessStatusCode();
            
            // 读取响应内容
            var content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine("=== 页面内容 ===");
            Console.WriteLine(content);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"请求失败: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"请求超时: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
    }
}
```
## 注意事项
1. **SSL/TLS 兼容性**：如果目标站点使用较旧的 TLS 版本，可能需要在代码中配置 `HttpClientHandler`。
2. **忽略 SSL 证书验证**（仅开发/测试环境）：
```csharp
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
};
using var httpClient = new HttpClient(handler);
```
3. **编码处理**：如果页面使用非 UTF-8 编码：
```csharp
var bytes = await response.Content.ReadAsByteArrayAsync();
string content = Encoding.GetEncoding("gb2312").GetString(bytes);
```
## 运行结果示例
```text
=== 页面内容 ===
ASMR Online 最新域名
asmr-300.com 随缘墙224 ms
asmr-200.com 随缘墙205 ms
asmr-100.com 国内墙连接失败
asmr.one 国内墙连接失败
```