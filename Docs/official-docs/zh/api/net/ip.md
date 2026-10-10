# k.net.IP

> 当前请求 IP 与 IP 地理信息

## 概述

`k.net.IP` 用于读取**当前入站请求**的客户端 IP，以及根据任意 IP 查询城市/国家信息。属性名为 **`IP`**（大小写敏感）。

## myIP

当前请求的客户端 IP（与 `k.request` 上下文一致）。

```ts
k.api.get("whoami", () => {
    return { ip: k.net.IP.myIP }
})
```

## getCity() / getCountry() / getCityOrCountry()

根据 IP 地址查询地理信息。

```ts
k.api.get("geo", () => {
    const info = k.net.IP.getCountry("8.8.8.8")
    return {
        countryCode: info?.countryCode,
        city: info?.city
    }
})
```

| 方法 | 说明 |
|------|------|
| `getCity(ip)` | 城市级信息 |
| `getCountry(ip)` | 国家级信息 |
| `getCityOrCountry(ip)` | 优先城市，无城市时回退国家 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `ip` | `string` | 是 | 要查询地理信息的 IP 地址。 |

**返回：** `IPInfo` 或 `null`

### IPInfo 常用字段

| 字段 | 说明 |
|------|------|
| `city` | 城市 |
| `state` | 省/州 |
| `countryCode` | 国家代码 |
| `continentCode` | 大洲代码 |
| `latitude` / `longitude` | 经纬度 |
| `asnDomain` | ASN 域名 |
| `ipUserType` | 用户类型枚举 |

## 相关文档

- [k.net.DNS](./dns.md)
- [k.request](../request/)
