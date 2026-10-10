# k.net.DNS

> DNS lookups, reverse resolution, and Whois

## Overview

`k.net.DNS` provides host name resolution and email/domain name related record query. The attribute name is **`DNS`** (case sensitive).

## getIPs()

Resolve the hostname to get a list of IPs.

```ts
k.api.get("resolve", () => {
    const ips = k.net.DNS.getIPs("kooboo.com")
    return {
        count: ips ? ips.length : 0,
        first: ips && ips[0] ? ips[0].address : null
    }
})
```

| Overload | Description |
|------|------|
| `getIPs(host)` | Use system DNS |
| `getIPs(nsServer, host)` | Specify NS server to query A records |

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `host` | `string` | Yes | Hostname or domain to resolve. |
| `nsServer` | `string` | No | DNS server to query. |

**Returns:** `KIPAddress[]` or `null`

### KIPAddress

| Field/Method | Description |
|-------------|------|
| `address` | IP string |
| `isIP4` / `isIP6` | address family |
| `toIP4()` / `toIP6()` | mapped string |

## getMXs()

Query MX records.

| Overload | Description |
|------|------|
| `getMXs(host)` | Default parsing |
| `getMXs(nsServer, host)` | Specify NS |

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `host` | `string` | Yes | Domain to query MX records for. |
| `nsServer` | `string` | No | DNS server to query. |

**Returns:** `string[]` or `null`

```ts
const records = k.net.DNS.getMXs("kooboo.com")
```

## getNsServers()

Get the NS server list of the domain name.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `host` | `string` | Yes | Domain to query NS records for. |

**Returns**: `string[] | null`. NS server list.

```ts
const ns = k.net.DNS.getNsServers("kooboo.com")
```

## getReverseDNS()

Do reverse DNS on the IP.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `ip` | `string` | Yes | IP address to reverse resolve. |

**Returns**: `string | null`. Resolved hostname.

```ts
const host = k.net.DNS.getReverseDNS("8.8.8.8")
```

## whoisInfo()

Returns the original Whois record text for a domain name.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `domain` | `string` | Yes | Domain to query Whois for. |

**Returns**: `string | null`. Raw Whois text.

```ts
const raw = k.net.DNS.whoisInfo("kooboo.com")
```

## Related Docs

- [k.net.IP](./ip.md) — IP geography (complementary to DNS resolution)
