# k.net.sshClient

> SSH 命令执行

## 概述

`k.net.sshClient` 建立 SSH 连接并在回调中执行命令。

## passwordConnect() / privateKeyConnect()

`passwordConnect(host, user, password, callback, timeout?)` 使用密码连接；`privateKeyConnect(host, user, privateKey, callback, timeout?)` 使用 PEM 私钥字符串连接。

```ts
k.net.sshClient.passwordConnect(host, user, password, (client) => {
    const output = client.runCommand("uname -a")
}, 30)
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `host` | `string` | 是 | SSH 主机名或 IP |
| `user` | `string` | 是 | 登录用户名 |
| `password` | `string` | 是 | 登录密码 |
| `privateKey` | `string` | 是 | PEM 私钥内容 |
| `callback` | `(client) => void` | 是 | 连接成功后的操作回调 |
| `timeout` | `number` | 否 | 连接超时秒数，默认 30 |

**返回：** `void`。连接会在回调执行后关闭。

```ts
const privateKey = k.file.read("keys/deploy_rsa.pem")
k.net.sshClient.privateKeyConnect(host, user, privateKey, client => {
    const output = client.runCommand("systemctl status nginx", 10)
    k.logger.information("SSH", output)
})
```

## ConnectedClient.runCommand()

执行 shell 命令并返回输出字符串。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `command` | `string` | 是 | 要执行的 shell 命令 |
| `timeout` | `number` | 否 | 命令超时秒数，默认 30 |

**返回：** `string`，命令输出。

```ts
k.net.sshClient.passwordConnect(host, user, password, client => {
    const disk = client.runCommand("df -h /", 10)
    const kernel = client.runCommand("uname -a")

    k.logger.information("SSH health", {
        disk,
        kernel
    })
})
```

## 相关文档

- [k.net.sftpClient](./sftpClient.md)
