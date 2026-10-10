# k.net.webSocket

> WebSocket 实时通信 — 接受连接、收发消息、跨 API 推送

## 概述

KScript 通过 `k.net.webSocket` 提供 WebSocket 支持。连接按**站点**隔离管理，每个连接以字符串 `id` 标识（登录场景常用用户 ID，匿名场景常用 clientId，见「连接 id 策略」）。

典型用法分两类：

1. **连接端点**：客户端发起 WebSocket 握手时，在 KScript 中调用 `accept()` 接受连接并监听消息。
2. **服务端推送**：在普通 HTTP API 中，通过 `get(id)` 获取已建立的连接并 `sendText()` 推送消息。

::: warning 与 HTTP API 的区别
WebSocket 连接端点**不需要**用 `k.api.get()` 包裹, 文件内直接写连接逻辑即可（CLI 同步站点常见写法）。
:::

## TypeScript 定义

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

## 路由定义

WebSocket 端点通过 API 文件的 URL 绑定路由（Kooboo AI Chat 用 `create_api` 的 `url` 参数）。客户端使用 `ws://` 或 `wss://` 协议连接该路径。

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

::: tip 连接 URL
若站点域名为 `example.com`，端点为 `/api/websocket/connect`，则客户端连接地址为 `wss://example.com/api/websocket/connect`（HTTPS 站点用 `wss`，HTTP 用 `ws`）。
:::

## 方法

### accept(id, receive)

将**当前 HTTP 请求**升级为 WebSocket 连接。

```ts
k.net.webSocket.accept('user_1', ctx => {
  // ctx.text   — 文本帧内容
  // ctx.binary — 二进制帧（number[]）
  // ctx.type   — 'Text' | 'Binary'
})
```

**行为说明：**

- 仅当请求头包含 WebSocket 升级信息时可用；否则抛出 `Invalid WebSocket request url`。
- `accept()` **会阻塞当前请求线程**，直到连接关闭。方法返回后才会继续执行后续代码，因此后续代码不是断开回调，不能用于及时清理。
- 客户端消息（如 `leave`）应在 `receive` 回调中处理；也可以依赖框架按 `Timeout` 自动移除空闲连接。
- 同一 `id` 再次 `accept` 时，框架会先关闭旧连接再注册新连接。
- 连接关闭后自动从连接池移除；框架**不会**触发任何服务端回调通知「某连接已断开」。

### list()

返回当前站点所有在线连接的 id 数组。

```ts
const ids = k.net.webSocket.list()
// ['user_1', 'user_2']
```

### get(id)

按 id 获取连接对象，用于在其他 API 中向指定客户端推送消息。

```ts
const connection = k.net.webSocket.get('user_1')
if (connection) {
  connection.sendText(JSON.stringify({ event: 'notify', data: { msg: 'hello' } }))
}
```

## WebSocketConnection

### sendText(text, callback?)

发送 UTF-8 文本帧。

```ts
connection.sendText('hello', () => {
  k.logger.debug('WebSocket', '消息已发送')
})
```

### sendBinary(binary, callback?)

发送二进制帧。

```ts
connection.sendBinary([0x48, 0x65, 0x6c, 0x6c, 0x6f])
```

### close(status?, statusDescription?)

主动关闭连接。

```ts
connection.close()
```

## 连接 id 策略

`accept(id, ...)` 的第一个参数 `id` 决定连接在 `list()` / `get()` 中的标识。同一 `id` 新连接会**替换**旧连接。按业务场景选择策略：

| 场景 | 推荐 id | 行为 |
|------|---------|------|
| 已登录、一人一连接 | 用户 ID（如 `k.account.user.current.id`） | 刷新或新设备登录会踢掉旧连接 |
| 匿名、同 Tab 刷新后 id 不变 | `sessionStorage` 持久化的 clientId | 刷新后 id 相同 → 替换旧连接，在线计数不增加 |
| 匿名、多 Tab 各自独立 | 每个 Tab 独立 clientId（如 `sessionStorage` 按 Tab 生成） | 各 Tab 独立连接，可同时在线 |

**通过 URL 传递 clientId（匿名场景常用）：**

客户端把 clientId 放在 WebSocket URL 的 query 中，服务端在 `accept()` 前读取：

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

是否要求登录（`k.account.isLogin`）由业务决定：task 协作类通常需登录；公告板、在线人数演示等匿名场景可用 clientId。

## 断开感知与 leave 消息

框架在连接断开时会从 `k.net.webSocket.list()` 移除该 id，但**不会**通知服务端业务代码。若需实时更新在线人数或广播「某用户离开」，需客户端配合：

1. **客户端主动发送 `leave`（推荐，best effort）**：在 `pagehide` / `beforeunload` / 组件 `onUnmounted` 时发送 `{ event: "leave" }`，再 `ws.close()`。
2. **服务端在 `receive` 回调中处理**：收到 `leave` 后向其余连接广播 `online_changed`；此逻辑必须写在 `accept` 的回调或 `socket.on('leave')` 内，不能写在 `accept()` 之后。
3. **心跳超时兜底**：客户端定期发 `ping`；若用户直接关 Tab 且 `leave` 未送达，框架会在 `Timeout * 2` 无活动后断开并从 list 移除，但**不会**自动广播人数变化。生产环境可接受短暂计数偏差，或配合定时校正。

::: tip leave 消息的局限性
`beforeunload` / `pagehide` 在移动端和部分浏览器下并不可靠，`leave` 属于尽力而为。不要假设每次断开都能收到 leave 消息。
:::

**服务端处理 leave 并广播在线人数：**

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

**客户端发送 leave：**

```js
function sendLeave(ws) {
  if (ws?.readyState === WebSocket.OPEN) {
    ws.send(JSON.stringify({ event: 'leave', data: {} }))
  }
}

window.addEventListener('pagehide', () => sendLeave(ws))
// Vue onUnmounted 中同样调用 sendLeave(ws) 后 ws.close()
```

## 完整示例

### 1. 连接端点（鉴权 + 单用户单连接）

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

### 2. useSocket 封装（推荐的事件驱动写法）

将 JSON 消息解析为事件，简化 `accept` 回调内的分支逻辑：

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

### 3. 业务 API 中推送消息

WebSocket 推送与普通 HTTP API 解耦：在创建/更新数据的 API 里调用推送函数即可。

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

## 消息格式建议

框架不强制消息结构，推荐统一 JSON 格式便于前后端约定：

```ts
{
  event: string,   // 事件名，如 'task_created' | 'ping'
  data: any,       // 业务数据
  time: number     // 时间戳
}
```

客户端发送：

```json
{ "event": "enter", "data": {} }
{ "event": "leave", "data": {} }
{ "event": "ping", "data": {} }
```

服务端响应：

```json
{ "event": "enter", "data": { "sid": "user_1", "online_num": 3 }, "time": 1710000000000 }
{ "event": "online_changed", "data": { "online_num": 2 }, "time": 1710000000000 }
```

## 注意事项

| 要点 | 说明 |
|------|------|
| 阻塞模型 | `accept()` 阻塞直到断开，之后才返回；它不是断开回调，及时清理逻辑写在 `receive` 回调内 |
| 站点隔离 | `list()` / `get()` 仅访问**当前站点**的连接，不会跨站 |
| 连接 id | 见上文「连接 id 策略」；同一 id 新连接会替换旧连接 |
| 断开感知 | 框架移除连接但不触发回调；实时人数需客户端发 `leave` 或接受心跳超时带来的延迟 |
| 鉴权时机 | 在 `accept()` 之前检查登录态；匿名场景可用 query 中的 clientId |
| 推送容错 | 目标不在线时 `get()` 返回空，推送前应判断；推送异常不应阻断 HTTP 主流程 |
| 心跳 | 框架按 `Timeout` 检测空闲连接；客户端建议定期发送 ping/heartbeat |
| 日志 | 使用 `k.logger`，不要使用 `console.log` |

## 前端连接

Kooboo 站点前端直接连接**当前站点同源**的 WebSocket 端点，使用浏览器原生 `WebSocket` API

**连接 URL 规则：**

- 页面与 API 同属一个 Kooboo 站点时，用 `location.host` 拼接路径
- HTTPS 站点用 `wss:`，HTTP 站点用 `ws:`
- 同域请求会自动携带 Cookie，服务端可在 `accept()` 前用 `k.account.isLogin` 鉴权
- 匿名场景可通过 query 传递 clientId，见上文「连接 id 策略」

```js
// 复用「连接 id 策略」中的 getWebSocketUrl()；登录端点也可以不传 clientId。
const ws = new WebSocket(getWebSocketUrl())

ws.onopen = () => {
  ws.send(JSON.stringify({ event: 'enter', data: {} }))
}

ws.onmessage = (e) => {
  const msg = JSON.parse(e.data)
  // 按 msg.event 更新页面 UI（如 enter、online_changed、notice_published）
}
```

**在客户端渲染 Vue 页面中使用（CSR 路线）：** 将上述逻辑放入 Page 的 `<script type="module">` 或 `/scripts/` 下的 ES Module，在 `onMounted` 中建立连接，在 `onUnmounted` 中发送 `leave` 并 `ws.close()`。实时推送类页面通常走 CSR 路线，参见 [客户端渲染 Vue 指南](../../Frontend/cdn-vue/index.md)。

**断线重连与心跳（建议）：**

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

## 相关文档

- [k.api](../core/api.md) — HTTP API 路由（WebSocket 端点不用 `k.api.get` 包裹）
- [k.logger](../core/logger.md) — 日志记录
- [客户端渲染 Vue](../../Frontend/cdn-vue/index.md) — 实时交互页面的前端路线
- [Api Layer Guide](../../Backend/api-layer-guide.md) — API 文件规范
