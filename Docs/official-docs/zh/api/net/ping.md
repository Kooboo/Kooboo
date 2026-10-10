# k.net.ping

> 使用 ICMP 探测主机连通性与往返时间

## ping()

`ping(host)` 向主机名或 IP 地址发送 ICMP 请求。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `host` | `string` | 是 | 要探测的主机名或 IP 地址 |

**返回：** `{ ip: string, time: number, success: boolean }`

| 字段 | 说明 |
|------|------|
| `ip` | 解析后的 IP 地址 |
| `time` | 往返时间，单位为毫秒 |
| `success` | ICMP 请求是否成功 |

```ts
const result = k.net.ping("kooboo.com")

return {
    ip: result.ip,
    milliseconds: result.time,
    reachable: result.success
}
```

## 相关文档

- [k.net.DNS](./dns.md)
- [k.net.IP](./ip.md)
