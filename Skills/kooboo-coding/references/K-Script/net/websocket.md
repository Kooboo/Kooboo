# k.net.webSocket

> WebSocket real-time communication — accept connections, exchange messages, and push across APIs

## Overview

KScript provides WebSocket support through `k.net.webSocket`. Connections are isolated by **site**, and each connection is identified by a string `id` (typically a user ID for logged-in scenarios or a clientId for anonymous scenarios; see “Connection ID strategy”).

There are two typical usage patterns:

1. **Connection endpoint**: when a client starts a WebSocket handshake, call `accept()` in KScript to accept the connection and listen for messages.
2. **Server push**: in a regular HTTP API, use `get(id)` to retrieve an established connection and push messages with `sendText()`.

::: warning Difference from HTTP APIs
A WebSocket endpoint **does not need** to be wrapped in `k.api.get()`; write the connection logic directly in the file (a common pattern for CLI-synced sites).
:::

## TypeScript definition

```ts
interface KWebSocket {
  /** 接受当前 WebSocket 请求；阻塞直到连接关闭 */
  accept(id: string, receive: (ctx: WebSocketContext) => void): void;

  /** 当前站点所有连接 id */
  list(): string[];

  /** 按 id 获取连接；不存在时返回 undefined */
  get(id: string): WebSocketConnection | undefined;
}

interface WebSocketContext {
  /** UTF-8 文本消息 */
  text: string;
  /** 原始二进制（字节数组） */
  binary: number[];
  /** 消息类型：`Text` 或 `Binary` */
  type: string;
}

interface WebSocketConnection {
  /** 发送文本消息；可选发送完成回调 */
  sendText(text: string, callback?: () => void): void;
  /** 发送二进制消息；可选发送完成回调 */
  sendBinary(binary: number[], callback?: () => void): void;
  /** 关闭连接 */
  close(status?: number, statusDescription?: string): void;
}
```

## Route definition

WebSocket endpoints are bound to a route through the API file URL (Kooboo AI Chat uses the `url` parameter of `create_api`). Clients connect to that path with the `ws://` or `wss://` protocol.

```ts
// 鉴权失败时返回 HTTP 状态码（握手阶段）
if (!k.account.isLogin) {
  return k.api.httpCode(401)
}

const sid = k.account.user.current.userName

k.net.webSocket.accept(sid, ctx => {
  k.logger.information('WebSocket', `收到消息: ${ctx.text}`)
})
```

::: tip Connection URL
If the site domain is `example.com` and the endpoint is `/api/websocket/connect`, the client URL is `wss://example.com/api/websocket/connect` (use `wss` for HTTPS sites and `ws` for HTTP sites).
:::

## Methods

### accept(id, receive)

Upgrades the **current HTTP request** to a WebSocket connection.

```ts
k.net.webSocket.accept('user_1', ctx => {
  // ctx.text   — 文本帧内容
  // ctx.binary — 二进制帧（number[]）
  // ctx.type   — 'Text' | 'Binary'
})
```

**Behavior:**

- Available only when the request headers contain a WebSocket upgrade; otherwise it throws `Invalid WebSocket request url`.
- `accept()` **blocks the current request thread** until the connection closes. Code after `accept()` runs only after the method returns, so it is not a disconnect callback and should not be used for timely cleanup.
- Handle client messages such as `leave` in the `receive` callback, or rely on the framework to remove idle connections according to `Timeout`.
- When `accept` is called again with the same `id`, the framework closes the old connection before registering the new one.
- A closed connection is removed from the connection pool automatically; the framework **does not** invoke a server callback to report that a connection has disconnected.

### list()

Returns the IDs of all online connections for the current site.

```ts
const ids = k.net.webSocket.list()
// ['user_1', 'user_2']
```

### get(id)

Gets a connection by ID so another API can push messages to a specific client.

```ts
const connection = k.net.webSocket.get('user_1')
if (connection) {
  connection.sendText(JSON.stringify({ event: 'notify', data: { msg: 'hello' } }))
}
```

## WebSocketConnection

### sendText(text, callback?)

Sends a UTF-8 text frame.

```ts
connection.sendText('hello', () => {
  k.logger.debug('WebSocket', '消息已发送')
})
```

### sendBinary(binary, callback?)

Sends a binary frame.

```ts
connection.sendBinary([0x48, 0x65, 0x6c, 0x6c, 0x6f])
```

### close(status?, statusDescription?)

Closes the connection explicitly.

```ts
connection.close()
```

## Connection ID strategy

The first argument, `id`, in `accept(id, ...)` determines how the connection is identified by `list()` / `get()`. A new connection with the same `id` **replaces** the old one. Choose a strategy for the business scenario:

| Scenario | Recommended ID | Behavior |
|------|---------|------|
| Logged in, one connection per user | User ID (such as `k.account.user.current.id`) | Refreshing or logging in on a new device removes the old connection |
| Anonymous, same ID after a Tab refresh | A clientId persisted in `sessionStorage` | The same ID after refresh replaces the old connection, so the online count does not increase |
| Anonymous, independent connections per Tab | A separate clientId for each Tab (for example, generated per Tab in `sessionStorage`) | Each Tab has its own connection and can be online simultaneously |

**Pass clientId in the URL (common for anonymous scenarios):**

The client places clientId in the WebSocket URL query, and the server reads it before `accept()`:

```js
function getOrCreateClientId() {
  let id = sessionStorage.getItem('wsClientId')
  if (!id) {
    id = crypto.randomUUID()
    sessionStorage.setItem('wsClientId', id)
  }
  return id
}

function getWebSocketUrl(path = '/api/websocket/connect') {
  const protocol = location.protocol === 'https:' ? 'wss:' : 'ws:'
  const clientId = getOrCreateClientId()
  return `${protocol}//${location.host}${path}?clientId=${encodeURIComponent(clientId)}`
}
```

```ts
const sid =
  k.request.queryString.get('clientId') ||
  (k.account.isLogin ? k.account.user.current.id : '')

if (!sid) {
  return k.api.httpCode(400)
}

k.net.webSocket.accept(sid, ctx => {
  // ...
})
```

Whether login is required (`k.account.isLogin`) is a business decision: task collaboration usually requires login, while notice boards and online-count demos can use clientId anonymously.

## Disconnect awareness and leave messages

When a connection closes, the framework removes its ID from `k.net.webSocket.list()` but **does not** notify server-side business code. To update online counts or broadcast that a user has left in real time, the client must cooperate:

1. **Client sends `leave` explicitly (recommended, best effort)**: send `{ event: "leave" }` on `pagehide` / `beforeunload` / component `onUnmounted`, then call `ws.close()`.
2. **Handle it on the server in the `receive` callback**: after receiving `leave`, broadcast `online_changed` to the other connections. Handle it inside the `accept` callback or `socket.on('leave')`, not in code that treats `accept()` as a disconnect callback.
3. **Heartbeat timeout fallback**: the client sends `ping` periodically. If the user closes the Tab directly and `leave` is not delivered, the framework disconnects and removes the connection from list after `Timeout * 2` of inactivity, but it **does not** automatically broadcast the changed count. Production systems can tolerate a short-lived count discrepancy or periodically reconcile it.

::: tip Limitations of leave messages
`beforeunload` / `pagehide` are unreliable on mobile and in some browsers, so `leave` is best effort. Do not assume that every disconnect delivers a leave message.
:::

**Handle leave on the server and broadcast the online count:**

```ts
import { useSocket } from "./Utils.useSocket"
import { broadcastOnlineCount } from "./Services.websocket"

const sid = k.request.queryString.get('clientId') || k.account.user.current.id
const socket = useSocket(sid)

socket.on('enter', () => {
  broadcastOnlineCount()
  socket.send({ to: sid, event: 'enter', data: { online_num: k.net.webSocket.list().length } })
})

socket.on('leave', () => {
  // 发送 leave 时连接仍在 list 中，广播时排除自己或计数减 1
  broadcastOnlineCount(sid)
})

socket.accept()
```

```ts
// Services.websocket CodeBlock — 向所有在线连接广播人数
export function broadcastOnlineCount(excludeId?: string) {
  const ids = k.net.webSocket.list().filter(id => id !== excludeId)
  const online_num = ids.length
  const payload = JSON.stringify({ event: 'online_changed', data: { online_num }, time: Date.now() })
  for (const id of ids) {
    k.net.webSocket.get(id)?.sendText(payload)
  }
}
```

**Send leave from the client:**

```js
function sendLeave(ws) {
  if (ws?.readyState === WebSocket.OPEN) {
    ws.send(JSON.stringify({ event: 'leave', data: {} }))
  }
}

window.addEventListener('pagehide', () => sendLeave(ws))
// Vue onUnmounted 中同样调用 sendLeave(ws) 后 ws.close()
```

## Complete examples

### 1. Connection endpoint (authorization + one connection per user)

```ts
// /api/websocket/connect Api

import { useSocket } from "./Utils.useSocket"

if (!k.account.isLogin) {
  return k.api.httpCode(401)
}

const sid = k.account.user.current.id

const socket = useSocket(sid)

socket.on('enter', () => {
  socket.send({
    to: sid,
    event: 'enter',
    data: { online_num: k.net.webSocket.list().length }
  })
})

socket.on('ping', () => {
  socket.send({ to: sid, event: 'pong', data: { timestamp: Date.now() } })
})

// 阻塞直到断开
socket.accept()
```

### 2. useSocket wrapper (recommended event-driven style)

Parse JSON messages as events to simplify branching inside the `accept` callback:

```ts
// Utils.useSocket CodeBlock

export function useSocket(sid: string) {
  const handlers: Record<string, Function[]> = {}

  function on(event: string, fn: (msg: any) => void) {
    (handlers[event] ??= []).push(fn)
  }

  function accept() {
    k.net.webSocket.accept(sid, ctx => {
      try {
        const msg = JSON.parse(ctx.text)
        if (msg?.event && handlers[msg.event]) {
          handlers[msg.event].forEach(fn => fn(msg))
        }
      } catch (e) {
        k.logger.error('WebSocket', `消息解析失败: ${e}`)
      }
    })
  }

  function send({ to, event, data }: { to: string; event: string; data?: any }) {
    const conn = k.net.webSocket.get(to)
    if (!conn) return
    conn.sendText(JSON.stringify({ event, data, time: Date.now() }))
  }

  return { on, accept, send }
}
```

### 3. Push messages from a business API

WebSocket pushes are decoupled from regular HTTP APIs: call the push function from the API that creates or updates the data.

```ts
// /api/task/create Api
import { pushTaskCreated } from "./Services.websocket"

k.api.post(() => {
  const body = JSON.parse(k.request.body)
  const task = createTask(body)

  // 推送失败不应影响主流程
  try {
    pushTaskCreated(task, body.projectId)
  } catch (e) {
    k.logger.warning('WebSocket', `推送失败: ${e}`)
  }

  return { success: true, data: task }
})
```

```ts
// Services.websocket CodeBlock

export function pushTaskCreated(task: any, projectId: string) {
  const message = JSON.stringify({
    event: 'task_created',
    data: { projectId, task },
    time: Date.now()
  })

  for (const id of k.net.webSocket.list()) {
    k.net.webSocket.get(id)?.sendText(message)
  }
}

export function sendToUser(userId: string, event: string, data: any) {
  const conn = k.net.webSocket.get(userId)
  if (!conn) return false
  conn.sendText(JSON.stringify({ event, data, time: Date.now() }))
  return true
}
```

## Message format recommendation

The framework does not enforce a message structure. A consistent JSON format is recommended for frontend/backend agreements:

```ts
{
  event: string,   // 事件名，如 'task_created' | 'ping'
  data: any,       // 业务数据
  time: number     // 时间戳
}
```

Client messages:

```json
{ "event": "enter", "data": {} }
{ "event": "leave", "data": {} }
{ "event": "ping", "data": {} }
```

Server responses:

```json
{ "event": "enter", "data": { "sid": "user_1", "online_num": 3 }, "time": 1710000000000 }
{ "event": "online_changed", "data": { "online_num": 2 }, "time": 1710000000000 }
```

## Notes

| Point | Description |
|------|------|
| Blocking model | `accept()` blocks until disconnect and returns afterward; it is not a disconnect callback. Put timely cleanup logic in the `receive` callback |
| Site isolation | `list()` / `get()` access connections for the **current site** only; they do not cross site boundaries |
| Connection ID | See “Connection ID strategy” above; a new connection with the same ID replaces the old one |
| Disconnect awareness | The framework removes the connection without invoking a callback; real-time counts require a client `leave` or acceptance of heartbeat-timeout delay |
| Authorization timing | Check login state before `accept()`; anonymous scenarios can use clientId from the query |
| Push fault tolerance | `get()` returns empty when the target is offline; check before pushing, and do not let push errors block the HTTP flow |
| Heartbeat | The framework detects idle connections using `Timeout`; clients should send ping/heartbeat periodically |
| Logging | Use `k.logger`, not `console.log` |

## Frontend connection

The frontend of a Kooboo site connects directly to a **same-origin** WebSocket endpoint on the current site using the browser's native `WebSocket` API.

**Connection URL rules:**

- When the page and API belong to the same Kooboo site, build the path with `location.host`.
- Use `wss:` for HTTPS sites and `ws:` for HTTP sites.
- Same-origin requests include cookies automatically, so the server can authorize with `k.account.isLogin` before `accept()`.
- Anonymous scenarios can pass clientId in the query; see “Connection ID strategy” above.

```js
// Reuse getWebSocketUrl() from “Connection ID strategy”, or omit clientId for logged-in endpoints.
const ws = new WebSocket(getWebSocketUrl())

ws.onopen = () => {
  ws.send(JSON.stringify({ event: 'enter', data: {} }))
}

ws.onmessage = (e) => {
  const msg = JSON.parse(e.data)
  // 按 msg.event 更新页面 UI（如 enter、online_changed、notice_published）
}
```

**Use in a client-rendered Vue page (CSR route):** Put the logic above in the Page's `<script type="module">` or an ES module under `/scripts/`; establish the connection in `onMounted`, then send `leave` and call `ws.close()` in `onUnmounted`. Pages with real-time pushes usually use the CSR route; see the [client-rendered Vue guide](../../Frontend/cdn-vue/index.md).

**Reconnect and heartbeat (recommended):**

```js
let ws = null
let pingTimer = null

function connect() {
  ws = new WebSocket(getWebSocketUrl())
  ws.onopen = () => {
    ws.send(JSON.stringify({ event: 'enter', data: {} }))
    pingTimer = setInterval(() => {
      if (ws.readyState === WebSocket.OPEN) {
        ws.send(JSON.stringify({ event: 'ping', data: {} }))
      }
    }, 25000)
  }
  ws.onclose = () => {
    clearInterval(pingTimer)
    setTimeout(connect, 3000)
  }
}
```

## Related documents

- [k.api](../core/api.md) — HTTP API routes (WebSocket endpoints do not need `k.api.get` wrapping)
- [k.logger](../core/logger.md) — logging
- [Client-rendered Vue](../../Frontend/cdn-vue/index.md) — frontend route for real-time interactive pages
- [API Layer Guide](../../Backend/api-layer-guide.md) — API file conventions
