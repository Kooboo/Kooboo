# k.net.IP

> Current requesting IP and IP geographic information

## Overview

`k.net.IP` is used to read the client IP of the current inbound request and query city/country information based on any IP. The attribute name is **`IP`** (case sensitive).

## myIP

The client IP of the current request (consistent with the `k.request` context).

```ts
k.api.get("whoami", () => {
    return { ip: k.net.IP.myIP }
})
```

## getCity() / getCountry() / getCityOrCountry()

Query geographical information based on IP address.

```ts
k.api.get("geo", () => {
    const info = k.net.IP.getCountry("8.8.8.8")
    return {
        countryCode: info?.countryCode,
        city: info?.city
    }
})
```

| Method | Description |
|------|------|
| `getCity(ip)` | City level information |
| `getCountry(ip)` | National level information |
| `getCityOrCountry(ip)` | Prioritize the city, fall back to the country if there is no city |

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `ip` | `string` | Yes | IP address to query. |

**Returns:** `IPInfo` or `null`

### IPInfo common fields

| Field | Description |
|------|------|
| `city` | City |
| `state` | Province/State |
| `countryCode` | country code |
| `continentCode` | continent code |
| `latitude` / `longitude` | Latitude and longitude |
| `asnDomain` | ASN domain name |
| `ipUserType` | User type enum |

## Related Docs

- [k.net.DNS](./dns.md)
- [k.request](../request/)
