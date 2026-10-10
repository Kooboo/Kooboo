# k.net.sshClient

> SSH command execution

## Overview

`k.net.sshClient` establishes an SSH connection and executes commands inside the callback.

## passwordConnect() / privateKeyConnect()

`passwordConnect(host, user, password, callback, timeout?)` connects with a password; `privateKeyConnect(host, user, privateKey, callback, timeout?)` connects with a PEM private key string.

```ts
k.net.sshClient.passwordConnect(host, user, password, (client) => {
    const output = client.runCommand("uname -a")
}, 30)
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `host` | `string` | Yes | SSH host name or IP |
| `user` | `string` | Yes | Login user |
| `password` | `string` | Yes | Login password |
| `privateKey` | `string` | Yes | PEM private key content |
| `callback` | `(client) => void` | Yes | Callback after the connection succeeds |
| `timeout` | `number` | No | Connection timeout in seconds; default is 30 |

**Returns:** `void`. The connection closes after the callback runs.

```ts
const privateKey = k.file.read("keys/deploy_rsa.pem")
k.net.sshClient.privateKeyConnect(host, user, privateKey, client => {
    const output = client.runCommand("systemctl status nginx", 10)
    k.logger.information("SSH", output)
})
```

## ConnectedClient.runCommand()

Execute a shell command and return its output.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `command` | `string` | Yes | Shell command to execute |
| `timeout` | `number` | No | Command timeout in seconds; default is 30 |

**Returns:** `string`, command output.

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

## Related Docs

- [k.net.sftpClient](./sftpClient.md)
