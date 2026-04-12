# as.131433.xyz 访问记录（2026-04-08）

## 请求 1：curl HTTP 级探测

- 请求 URL：`https://as.131433.xyz/`
- 执行命令：`curl.exe -v -k --http1.1 --tls-max 1.2 --max-time 20 https://as.131433.xyz/`

响应：

```text
* Host as.131433.xyz:443 was resolved.
* IPv6: (none)
* IPv4: 172.67.160.56, 104.21.90.193
*   Trying 172.67.160.56:443...
* schannel: disabled automatic use of client certificate
* ALPN: curl offers http/1.1
* schannel: next InitializeSecurityContext failed: SEC_E_ILLEGAL_MESSAGE (0x80090326) - This error usually occurs when a fatal SSL/TLS alert is received (e.g. handshake failed). More detail may be available in the Windows System event log.
* closing connection #0
curl: (35) schannel: next InitializeSecurityContext failed: SEC_E_ILLEGAL_MESSAGE (0x80090326) - This error usually occurs when a fatal SSL/TLS alert is received (e.g. handshake failed). More detail may be available in the Windows System event log.
```

## 请求 2：页面内容抓取

- 请求 URL：`https://as.131433.xyz/`

响应：

```text
ASMR Online 最新域名asmr-300.com 随缘墙224 ms
asmr-200.com 随缘墙205 ms
asmr-100.com 国内墙连接失败
asmr.one 国内墙连接失败
```

## 附录：C# 获取 HTTPS 页面文本的实现方式

### 1. 基础方式（HttpClient）

```csharp
using var httpClient = new HttpClient();
var response = await httpClient.GetAsync("https://example.com");
response.EnsureSuccessStatusCode();
string content = await response.Content.ReadAsStringAsync();
```

### 2. 带请求头和超时配置

```csharp
using var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(30);
httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
var response = await httpClient.GetStringAsync("https://example.com");
```

### 3. 处理编码问题

```csharp
var response = await httpClient.GetAsync(url);
var bytes = await response.Content.ReadAsByteArrayAsync();
string content = Encoding.UTF8.GetString(bytes);
```

### 4. 忽略 SSL 证书验证（仅开发环境）

```csharp
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
};
using var httpClient = new HttpClient(handler);
```

### 关键要点

| 场景              | 建议                                  |
| ----------------- | ------------------------------------- |
| 生产环境          | 使用 `HttpClient`，建议单例或依赖注入 |
| .NET Core/.NET 5+ | 优先使用 `HttpClient`                 |
| 需要序列化 JSON   | 配合 `System.Text.Json` 使用          |