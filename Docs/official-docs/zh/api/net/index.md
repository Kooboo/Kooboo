# k.net

> HTTP、WebSocket、DNS、IP 与远程连接

## 概述

`k.net` 提供站点脚本中的网络相关能力，按子模块划分：

| 子模块 | 路径 | 用途 |
|--------|------|------|
| [httpClient](./httpclient.md) | `k.net.httpClient` | 推荐：代理转发、JSON/表单/multipart、`send` 统一发请求 |
| [url](./url.md) | `k.net.url` | 简便同步 HTTP；适合快速拉取二进制（如图片） |
| [webSocket](./websocket.md) | `k.net.webSocket` | 升级当前请求为 WebSocket、管理连接 |
| [DNS](./dns.md) | `k.net.DNS` | 解析 A/MX/NS、反向 DNS、Whois（注意大小写） |
| [IP](./ip.md) | `k.net.IP` | 当前请求 IP、IP 地理信息（注意大小写） |
| [ping](./ping.md) | `k.net.ping` | ICMP 主机连通性与往返时间探测 |
| [sshClient](./sshClient.md) | `k.net.sshClient` | SSH 命令执行 |
| [sftpClient](./sftpClient.md) | `k.net.sftpClient` | SFTP 文件上传与下载 |
| [telnetClient](./telnetClient.md) | `k.net.telnetClient` | Telnet 会话读写 |

::: tip 选型：httpClient 与 url
| 场景 | 建议 |
|------|------|
| API 代理、带自定义 Header/Body、multipart 上传 | **`k.net.httpClient`** |
| 批量下载、需 `KHttpResponse.save` 落盘 | **`k.net.httpClient`**（含 batch） |
| 快速 GET 远程**图片/文件二进制** | **`k.net.url.getAsBinary(url)`** |
| 一行 GET 字符串或 JSON | `k.net.url.get` / `k.net.url.getJson` |
:::

## 相关文档

- [k.file](../file/file.md) — 站点磁盘文件（与出站 HTTP 不同）
- [k.request](../request/) — 当前入站请求
- [k.utils](../utils/) — 编码转换等通用工具
