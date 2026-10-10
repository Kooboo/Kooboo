# k.net.sftpClient

> SFTP 文件上传与下载

## 概述

连接方式与 [sshClient](./sshClient.md) 相同；回调内使用 `ConnectedClient`。

## passwordConnect() / privateKeyConnect()

`passwordConnect(host, user, password, callback, timeout?)` 使用密码连接；`privateKeyConnect(host, user, privateKey, callback, timeout?)` 使用 PEM 私钥字符串连接。连接建立后在回调内操作文件，回调结束后连接会关闭。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `host` | `string` | 是 | SFTP 主机名或 IP |
| `user` | `string` | 是 | 登录用户名 |
| `password` | `string` | 是 | 登录密码 |
| `privateKey` | `string` | 是 | PEM 私钥内容 |
| `callback` | `(client) => void` | 是 | 连接成功后的操作回调 |
| `timeout` | `number` | 否 | 连接超时秒数，默认 30 |

**返回：** `void`。在 `callback` 内通过 `ConnectedClient` 上传或下载。

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

使用私钥连接时，把私钥内容作为字符串传入：

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

| 方法 | 说明 |
|------|------|
| `upload(data, path)` | 上传字节到远程路径 |
| `download(path)` | 下载远程文件为字节数组 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `data` | `number[]` | 是 | 要上传的字节数组 |
| `path` | `string` | 是 | 远程文件路径 |

**返回：** `upload()` 返回 `void`；`download()` 返回 `number[]`。

```ts
k.net.sftpClient.passwordConnect(host, user, password, client => {
    const bytes = client.download("/exports/orders.csv")
    const text = k.utils.bytesToString(bytes, "utf-8")

    return text.split("\n").length
})
```

## 相关文档

- [k.net.sshClient](./sshClient.md)
