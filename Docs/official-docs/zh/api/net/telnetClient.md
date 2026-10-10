# k.net.telnetClient

> Telnet 会话

## 概述

`k.net.telnetClient.connect` 建立 Telnet 连接，在回调中读写终端。

## connect()

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `host` | `string` | 是 | Telnet 主机名或 IP |
| `port` | `number` | 是 | 端口 |
| `callback` | `(client) => void` | 是 | 连接后的会话回调 |
| `timeout` | `number` | 否 | 超时秒数，默认 30 |

**返回：** `void`。

```ts
k.net.telnetClient.connect(host, port, (client) => {
    client.writeLine("help")
    const text = client.read(10)
}, 30)
```

## ConnectedClient

| 方法 | 说明 |
|------|------|
| `write` / `writeLine` | 发送命令 |
| `read(timeout?)` | 读取输出 |
| `terminatedRead(terminator, timeout?)` | 读到终止符 |

| 方法 | 参数 | 返回 |
|------|------|------|
| `write(command)` | `string` | `void` |
| `writeLine(command)` | `string` | `void` |
| `read(timeout?)` | `number?` | `string` |
| `terminatedRead(terminator, timeout?)` | `string, number?` | `string` |

```ts
k.net.telnetClient.connect("smtp.example.com", 25, client => {
    const hello = client.terminatedRead("\r\n", 10)
    client.writeLine("HELO kooboo.local")
    const response = client.read(10)
    return { hello, response }
})
```

## 相关文档

- [k.net 概述](./index.md)
