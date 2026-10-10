# k.net.ping

> Probe host connectivity and round-trip time with ICMP

## ping()

`ping(host)` sends an ICMP request to a host name or IP address.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `host` | `string` | Yes | Host name or IP address to probe |

**Returns:** `{ ip: string, time: number, success: boolean }`

| Field | Description |
|-------|-------------|
| `ip` | Resolved IP address |
| `time` | Round-trip time in milliseconds |
| `success` | Whether the ICMP request succeeded |

```ts
const result = k.net.ping("kooboo.com")

return {
    ip: result.ip,
    milliseconds: result.time,
    reachable: result.success
}
```

## Related Docs

- [k.net.DNS](./dns.md)
- [k.net.IP](./ip.md)
