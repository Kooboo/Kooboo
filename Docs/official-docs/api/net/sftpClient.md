# k.net.sftpClient

> SFTP file upload and download

## Overview

The connection methods are the same as [sshClient](./sshClient.md); use `ConnectedClient` inside the callback.

## passwordConnect() / privateKeyConnect()

`passwordConnect(host, user, password, callback, timeout?)` connects with a password; `privateKeyConnect(host, user, privateKey, callback, timeout?)` connects with a PEM private key string. Operate on files inside the callback; the connection closes after the callback finishes.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `host` | `string` | Yes | SFTP host name or IP |
| `user` | `string` | Yes | Login user |
| `password` | `string` | Yes | Login password |
| `privateKey` | `string` | Yes | PEM private key content |
| `callback` | `(client) => void` | Yes | Callback after the connection succeeds |
| `timeout` | `number` | No | Connection timeout in seconds; default is 30 |

**Returns:** `void`. Use `ConnectedClient` inside `callback` to upload or download files.

```ts
k.net.sftpClient.passwordConnect(
    "sftp.example.com",
    "deploy",
    "password",
    client => {
        const report = k.file.readBinary("exports/report.csv")
        client.upload(report, "/incoming/report.csv")

        const remoteCopy = client.download("/incoming/report.csv")
        k.file.writeBinary("exports/report.remote-copy.csv", remoteCopy)
    },
    30
)
```

When using key authentication, pass the private key content as a string:

```ts
const privateKey = k.file.read("keys/deploy_rsa.pem")

k.net.sftpClient.privateKeyConnect(
    "sftp.example.com",
    "deploy",
    privateKey,
    client => {
        const backup = client.download("/backups/latest.zip")
        k.file.writeBinary("backups/latest.zip", backup)
    }
)
```

## ConnectedClient

| Method | Description |
|--------|-------------|
| `upload(data, path)` | Upload bytes to a remote path |
| `download(path)` | Download a remote file as a byte array |

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `data` | `number[]` | Yes | Bytes to upload |
| `path` | `string` | Yes | Remote file path |

**Returns:** `upload()` returns `void`; `download()` returns `number[]`.

```ts
k.net.sftpClient.passwordConnect(host, user, password, client => {
    const bytes = client.download("/exports/orders.csv")
    const text = k.utils.bytesToString(bytes, "utf-8")

    return text.split("\n").length
})
```

## Related Docs

- [k.net.sshClient](./sshClient.md)
