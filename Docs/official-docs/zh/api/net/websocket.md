# k.net.webSocket

> 将当前请求升级为 WebSocket 并管理站点内连接

## 概述

通过 `k.net.webSocket`（注意 **webSocket** 大小写）处理 WebSocket：在当前 API 请求上 `accept` 升级连接，或通过 `get` 向已存在连接发消息。

客户端须使用 **`ws://` 或 `wss://`** 访问对应 API 地址，否则会报错。

## accept()

将**当前 HTTP 请求**升级为 WebSocket，并在连接关闭前**阻塞**当前脚本（常用于专用 WS 端点）。

| 参数 | 类型 | 说明 |
|------|------|------|
| `id` | `string` | 连接标识，同站点内唯一，供 `get` / `list` 使用 |
| `receive` | `function` | 收到消息回调，参数 `ctx` 含 `text` / `binary` |

**返回值**：`void`。连接关闭前当前脚本会保持运行。

```ts
k.api.get("ws", () => {
    k.net.webSocket.accept("user_1", (ctx) => {
        if (ctx.text) {
            // 处理文本帧
        }
        if (ctx.binary) {
            // 处理二进制帧
        }
    })
})
```

受站点 WebSocket 配置限制（最大连接数、收发大小、超时等），超限会抛出异常。

## list()

返回当前站点已注册连接 id 列表。

**参数**：无。

**返回值**：`string[]`。当前站点已注册连接 ID 列表。

```ts
const ids = k.net.webSocket.list()
```

## get()

按 id 获取连接，用于服务端主动推送。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `id` | `string` | 是 | 连接 ID。 |

**返回值**：`WebSocketConnection | null`。匹配到的连接；不存在时为空值。

```ts
const conn = k.net.webSocket.get("user_1")
if (conn) {
    conn.sendText("hello")
    conn.sendBinary([1, 2, 3])
    conn.close()
}
```

| 方法 | 说明 |
|------|------|
| `sendText(text, callback?)` | 发送文本帧 |
| `sendBinary(bytes, callback?)` | 发送二进制帧 |
| `close(status?, description?)` | 关闭连接 |

## 相关文档

- [k.net 概述](./index.md)
- [k.net.httpClient](./httpclient.md)
