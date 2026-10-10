# k.security

> 安全相关工具 - 哈希、加密、编码、GUID、JWT 等

## 概述

`k.security` 提供一系列安全相关的工具方法，包括：哈希算法、加密解密、Base64 编解码、GUID 生成、JWT 操作等。

## TypeScript 定义

```ts
interface Security {
  // 哈希算法
  md5(input: string): string;
  sha1(input: string): string;
  sha256(input: string): string;
  sha512(input: string): string;
  sha256Binary(input: string): string;
  hmacMd5(input: string, key: string): string;
  hmacSha1(input: string, key: string): string;
  hmacSha256(input: string, key: string): string;

  // 加密解密
  encrypt(input: string, key: string): string;
  decrypt(input: string, key: string): string;
  aesEncrypt(input: string, key: string): string;
  aesDecrypt(input: string, key: string): string;

  // Base64 编解码
  toBase64(input: string | number[]): string;
  fromBase64(input: string): string;
  decodeBase64(input: string): number[];

  // GUID
  newGuid(): string;
  shortGuid(): string;
  hashGuid(input: string, options?: { source?: string }): any;

  // 密码
  hashPassword(password: string): string;
  verifyPassword(password: string, saltedPassword: string): boolean;

  // JWT
  jwt: {
    encode(payload: object): string;
    decode(token?: string): string;  // 返回 JSON 字符串，需 JSON.parse()
  };

  // RSA
  rsa: {
    generateKeys(keySize: number): { publicKey: string; privateKey: string };
    encrypt(publicKey: string, content: string): string;
    decrypt(privateKey: string, content: string): string;
  };
}
```

## 哈希算法

### md5()

计算字符串的 MD5 哈希值。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要计算哈希的字符串 |

**返回：** `string`，MD5 哈希字符串。

```ts
k.api.get(() => {
    return { hash: k.security.md5("hello") }
})
// 返回: { "hash": "5D41402ABC4B2A76B9719D911017C592" }
```

### sha256()

计算字符串的 SHA256 哈希值。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要计算哈希的字符串 |

**返回：** `string`，SHA256 哈希字符串。

```ts
k.api.get(() => {
    return { hash: k.security.sha256("hello") }
})
// 返回: { "hash": "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824" }
```

### sha512()

计算字符串的 SHA512 哈希值。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要计算哈希的字符串 |

**返回：** `string`，SHA512 哈希字符串。

```ts
k.api.get(() => {
    return { hash: k.security.sha512("hello") }
})
```

### sha256Binary()

计算字符串的 SHA256 哈希值，返回二进制格式。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要计算哈希的字符串 |

**返回：** `string`，二进制格式的 SHA256 哈希结果。

```ts
k.api.get(() => {
    return { hash: k.security.sha256Binary("hello") }
})
```

## 编码解码

### toBase64()

将字符串或二进制数组转换为 Base64 编码。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string \| number[]` | 是 | 要编码的字符串或字节数组 |

**返回：** `string`，Base64 字符串。

```ts
k.api.get(() => {
    return { encoded: k.security.toBase64("hello") }
})
// 返回: { "encoded": "aGVsbG8=" }
```

### fromBase64()

将 Base64 字符串解码为普通字符串。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | Base64 字符串 |

**返回：** `string`，解码后的文本。

```ts
k.api.get(() => {
    return { decoded: k.security.fromBase64("aGVsbG8=") }
})
// 返回: { "decoded": "hello" }
```

### decodeBase64()

将 Base64 字符串解码为字节数组。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | Base64 字符串 |

**返回：** `number[]`，解码后的字节数组。

```ts
k.api.get(() => {
    return { bytes: k.security.decodeBase64("aGVsbG8=") }
})
// 返回: { "bytes": [104, 101, 108, 108, 111] }
```

## GUID

### newGuid()

生成一个新的 GUID。

**参数：** 无。

**返回：** `string`，新的 GUID 字符串。

```ts
k.api.get(() => {
    return { guid: k.security.newGuid() }
})
// 返回: { "guid": "85ef9c4fe0ae4a3aa21e9c0a25f609c6" }
```

### shortGuid()

生成一个短格式的 GUID。

**参数：** 无。

**返回：** `string`，短 GUID 字符串。

```ts
k.api.get(() => {
    return { guid: k.security.shortGuid() }
})
```

### hashGuid()

根据输入生成一个 Hash GUID。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 生成 Hash GUID 的输入 |
| `options` | `{ source?: string }` | 否 | 来源配置，例如 `{ source: "FileIo" }` |

**返回：** `any`，Hash GUID 结果对象或字符串（以运行时返回为准）。

```ts
k.api.get(() => {
    return k.security.hashGuid("hello")
})

k.api.get(() => {
    return k.security.hashGuid("folder/1.jpg", { source: "FileIo" })
})
```

## 密码

### hashPassword()

使用内置最佳实践对密码进行哈希运算。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `password` | `string` | 是 | 原始密码 |

**返回：** `string`，带盐的密码哈希。

```ts
k.api.get(() => {
    return { hashed: k.security.hashPassword("mypassword") }
})
```

### verifyPassword()

验证密码是否匹配。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `password` | `string` | 是 | 待验证的原始密码 |
| `saltedPassword` | `string` | 是 | `hashPassword()` 生成的带盐哈希 |

**返回：** `boolean`。

```ts
k.api.get(() => {
    const hashed = k.security.hashPassword("mypassword")
    return { valid: k.security.verifyPassword("mypassword", hashed) }
})
```

## JWT

::: tip 前提条件
JWT 功能需要在后台配置：**站点设置 → 服务集成 → Others → JwtSetting**
:::

### encode()

生成 JWT 令牌。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `payload` | `object` | 是 | 要写入 JWT 的负载对象 |

**返回：** `string`，JWT token。

```ts
k.api.get(() => {
    return { token: k.security.jwt.encode({ name: "user" }) }
})
```

### decode()

解码 JWT 令牌。默认从请求 header 的 `Authorization` 中获取 token，也可以手动传入。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `token` | `string` | 否 | JWT token；不传时从 `Authorization` header 读取 |

**返回：** `string`，JSON 字符串，需要 `JSON.parse()` 后读取。

::: tip 注意
`decode()` 返回的是**字符串**，需要 `JSON.parse()` 解析。返回格式为 `"{ code: 0, value: {...} }"` 或 `"{ code: 1, value: \"错误信息\" }"`

**不同错误类型返回不同的 HTTP 状态码：**

- 格式错误（不是3段）：返回 HTTP 400
- 签名错误：返回 HTTP 200
:::

```ts
// 方式1: 不传 token，从 Authorization header 自动获取
k.api.get(() => {
    return k.security.jwt.decode()
})

// 方式2: 手动传入 token
k.api.get(() => {
    const token = "eyJ0eXAi..."
    return k.security.jwt.decode(token)
})

// 解析返回结果
k.api.get(() => {
    const result = JSON.parse(k.security.jwt.decode(token))
    if (result.code === 0) {
        // 成功
        return result.value
    } else {
        // 失败
        return { error: result.value }
    }
})
```

::: tip 最佳实践：try-catch + code 判断

```ts
k.api.post(() => {
    try {
        const result = JSON.parse(k.security.jwt.decode())
        if (result.code === 0) {
            return { success: true, data: result.value }
        } else {
            return { success: false, error: result.value }
        }
    } catch (e) {
        // 格式错误等异常
        return { success: false, error: "Invalid token format" }
    }
})
```

:::


## 加密解密

### encrypt() / decrypt()

使用对称加密。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `encrypt(input, key)` | `input: string`, `key: string` | `string` | 加密文本 |
| `decrypt(input, key)` | `input: string`, `key: string` | `string` | 解密文本 |

```ts
k.api.get(() => {
    const encrypted = k.security.encrypt("hello", "mykey")
    const decrypted = k.security.decrypt(encrypted, "mykey")
    return { encrypted, decrypted }
})
```

### aesEncrypt() / aesDecrypt()

使用 AES 加密。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `aesEncrypt(input, key)` | `input: string`, `key: string` | `string` | AES 加密文本 |
| `aesDecrypt(input, key)` | `input: string`, `key: string` | `string` | AES 解密文本 |

```ts
k.api.get(() => {
    const encrypted = k.security.aesEncrypt("hello", "hashkey")
    const decrypted = k.security.aesDecrypt(encrypted, "hashkey")
    return { encrypted, decrypted }
})
```

## HMAC

### hmacMd5()

使用 HMAC-MD5 算法。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要签名的内容 |
| `key` | `string` | 是 | HMAC 密钥 |

**返回：** `string`，HMAC-MD5 签名。

```ts
k.api.get(() => {
    return { hmac: k.security.hmacMd5("hello", "key") }
})
```

### hmacSha1()

使用 HMAC-SHA1 算法。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要签名的内容 |
| `key` | `string` | 是 | HMAC 密钥 |

**返回：** `string`，HMAC-SHA1 签名。

```ts
k.api.get(() => {
    return { hmac: k.security.hmacSha1("hello", "key") }
})
```

### hmacSha256()

使用 HMAC-SHA256 算法。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 要签名的内容 |
| `key` | `string` | 是 | HMAC 密钥 |

**返回：** `string`，HMAC-SHA256 签名。

```ts
k.api.get(() => {
    return { hmac: k.security.hmacSha256("hello", "key") }
})
```

## RSA

### generateKeys()

生成 RSA 密钥对。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `keySize` | `number` | 是 | RSA 密钥长度，例如 `2048` |

**返回：** `{ publicKey: string; privateKey: string }`。

```ts
k.api.get(() => {
    return k.security.rsa.generateKeys(2048)
})
// 返回: { "publicKey": "...", "privateKey": "..." }
```

### encrypt() / decrypt()

使用 RSA 加密解密。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `rsa.encrypt(publicKey, content)` | `publicKey: string`, `content: string` | `string` | 使用公钥加密 |
| `rsa.decrypt(privateKey, content)` | `privateKey: string`, `content: string` | `string` | 使用私钥解密 |

```ts
k.api.get(() => {
    const keys = k.security.rsa.generateKeys(2048)
    const encrypted = k.security.rsa.encrypt(keys.publicKey, "message")
    const decrypted = k.security.rsa.decrypt(keys.privateKey, encrypted)
    return { encrypted, decrypted }
})
```

## 相关文档

- [k.request](../request/) - HTTP 请求信息
- [k.response](../response/) - HTTP 响应处理
- [k.cookie](../cookie/) - Cookie 管理
