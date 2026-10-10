# k.security

> Security utilities — hashing, encryption, encoding, GUIDs, JWT, and more

## Overview

`k.security` provides security utilities for hashing, encryption and decryption, Base64 encoding and decoding, GUID generation, JWT operations, and more.

## TypeScript definition

```ts
interface Security {
  // 哈希算法
  md5(input: string): string;
  sha1(input: string): string;
  sha256(input: string): string;
  sha512(input: string): string;
  sha256Binary(input: string): number[];
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
  hashGuid(input: string, options?: { source?: "Input" | "FileIo" }): string;

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

## Hashing algorithms

### md5()

Computes the MD5 hash of a string.

```ts
k.api.get(() => {
    return { hash: k.security.md5("hello") }
})
// 返回: { "hash": "5D41402ABC4B2A76B9719D911017C592" }
```

### sha256()

Computes the SHA256 hash of a string.

```ts
k.api.get(() => {
    return { hash: k.security.sha256("hello") }
})
// 返回: { "hash": "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824" }
```

### sha512()

Computes the SHA512 hash of a string.

```ts
k.api.get(() => {
    return { hash: k.security.sha512("hello") }
})
```

### sha256Binary()

Computes the SHA256 hash of a string and returns the raw digest as a byte array (`number[]`), not a hexadecimal string.

```ts
k.api.get(() => {
    return { hash: k.security.sha256Binary("hello") }
})
```

## Encoding and decoding

### toBase64()

Converts a string or binary array to Base64.

```ts
k.api.get(() => {
    return { encoded: k.security.toBase64("hello") }
})
// 返回: { "encoded": "aGVsbG8=" }
```

### fromBase64()

Decodes a Base64 string to a regular string.

```ts
k.api.get(() => {
    return { decoded: k.security.fromBase64("aGVsbG8=") }
})
// 返回: { "decoded": "hello" }
```

### decodeBase64()

Decodes a Base64 string to a byte array.

```ts
k.api.get(() => {
    return { bytes: k.security.decodeBase64("aGVsbG8=") }
})
// 返回: { "bytes": [104, 101, 108, 108, 111] }
```

## GUIDs

### newGuid()

Generates a new GUID.

```ts
k.api.get(() => {
    return { guid: k.security.newGuid() }
})
// 返回: { "guid": "85ef9c4fe0ae4a3aa21e9c0a25f609c6" }
```

### shortGuid()

Generates a short-form GUID.

```ts
k.api.get(() => {
    return { guid: k.security.shortGuid() }
})
```

### hashGuid()

Generates a deterministic hash GUID and returns it as a 32-character hexadecimal string. With `{ source: "FileIo" }`, the input is resolved as a site file path and the file contents are hashed; a missing file returns an empty string.

```ts
k.api.get(() => {
    return k.security.hashGuid("hello")
})

k.api.get(() => {
    return k.security.hashGuid("folder/1.jpg", { source: "FileIo" })
})
```

## Passwords

### hashPassword()

Hashes a password using the built-in best practices.

```ts
k.api.get(() => {
    return { hashed: k.security.hashPassword("mypassword") }
})
```

### verifyPassword()

Verifies whether a password matches.

```ts
k.api.get(() => {
    const hashed = k.security.hashPassword("mypassword")
    return { valid: k.security.verifyPassword("mypassword", hashed) }
})
```

## JWT

::: tip Prerequisite
Configure an explicit JWT secret at **Site settings → Service integration → Others → JwtSetting** for normal use. If the setting or its secret is missing, the runtime falls back to the site ID as the signing secret.
:::

### encode()

Generates a JWT token.

```ts
k.api.get(() => {
    return { token: k.security.jwt.encode({ name: "user" }) }
})
```

### decode()

Decodes a JWT token. With no argument, the runtime first reads `Authorization: Bearer <token>` and then falls back to the `jwt_access_token` cookie. A token can also be passed explicitly.

::: tip Note
`decode()` returns a **JSON string** and must be parsed with `JSON.parse()`. The parsed result uses `code` and `value`: `code === 0` means verification succeeded; `code === 1` contains an error message. The function itself does not set an HTTP status code; an authentication rule or the surrounding API decides how to respond.

When an explicit token is passed, malformed-token exceptions may be thrown to the caller. The no-argument overload catches decode exceptions and returns a `code === 1` result.
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

::: tip Best practice: try-catch plus code checks
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

## Encryption and decryption

### encrypt() / decrypt()

Uses symmetric encryption.

```ts
k.api.get(() => {
    const encrypted = k.security.encrypt("hello", "mykey")
    const decrypted = k.security.decrypt(encrypted, "mykey")
    return { encrypted, decrypted }
})
```

### aesEncrypt() / aesDecrypt()

Uses AES encryption.

```ts
k.api.get(() => {
    const encrypted = k.security.aesEncrypt("hello", "hashkey")
    const decrypted = k.security.aesDecrypt(encrypted, "hashkey")
    return { encrypted, decrypted }
})
```

## HMAC

### hmacMd5()

Uses the HMAC-MD5 algorithm.

```ts
k.api.get(() => {
    return { hmac: k.security.hmacMd5("hello", "key") }
})
```

### hmacSha1()

Uses the HMAC-SHA1 algorithm.

```ts
k.api.get(() => {
    return { hmac: k.security.hmacSha1("hello", "key") }
})
```

### hmacSha256()

Uses the HMAC-SHA256 algorithm.

```ts
k.api.get(() => {
    return { hmac: k.security.hmacSha256("hello", "key") }
})
```

## RSA

### generateKeys()

Generates an RSA key pair.

```ts
k.api.get(() => {
    return k.security.rsa.generateKeys(2048)
})
// 返回: { "publicKey": "...", "privateKey": "..." }
```

### encrypt() / decrypt()

Uses RSA encryption and decryption.

```ts
k.api.get(() => {
    const keys = k.security.rsa.generateKeys(2048)
    const encrypted = k.security.rsa.encrypt(keys.publicKey, "message")
    const decrypted = k.security.rsa.decrypt(keys.privateKey, encrypted)
    return { encrypted, decrypted }
})
```

## Related documents

- [k.request](./request.md) - HTTP request information
- [k.response](./response.md) - HTTP response handling
- [k.cookie](./cookie.md) - Cookie management
