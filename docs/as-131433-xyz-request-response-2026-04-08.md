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