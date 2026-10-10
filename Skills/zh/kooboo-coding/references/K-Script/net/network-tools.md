# k.net 网络工具

> DNS/Whois、IP 地理信息、Ping、SSH、SFTP、Telnet

## DNS 与 Whois

```ts
const addresses = k.net.DNS.getIPs('example.com')
const addressesFromServer = k.net.DNS.getIPs('8.8.8.8', 'example.com')
const reverse = k.net.DNS.getReverseDNS('8.8.8.8')
const mx = k.net.DNS.getMXs('example.com')
const nameservers = k.net.DNS.getNsServers('example.com')
const whois = k.net.DNS.whoisInfo('example.com')
```

必须指定 DNS 服务器时，使用 `getIPs(nameServer, host)` 或 `getMXs(nameServer, host)` 重载。底层查询失败时，DNS 查询方法返回 `null`。

## IP

`IP` 为 PascalCase。使用 `k.net.IP.myIP` 读取当前客户端地址，使用查询方法获取地理信息。

```ts
const ip = k.net.IP.myIP;
const city = k.net.IP.getCity("8.8.8.8");
const country = k.net.IP.getCountry("8.8.8.8");
const either = k.net.IP.getCityOrCountry("8.8.8.8");
```

## ping()

```ts
const result = k.net.ping("kooboo.com");
// { ip: "...", time: 42, success: true }
```

## SSH

```ts
k.net.sshClient.passwordConnect(host, user, password, client => {
  const output = client.runCommand("uname -a", 10);
});
```

使用 PEM 私钥认证时，调用 `privateKeyConnect(host, user, privateKey, callback, timeout?)`。

## SFTP

```ts
k.net.sftpClient.passwordConnect(host, user, password, client => {
  const data = client.download("/remote/report.csv");
  client.upload(data, "/remote/report-copy.csv");
});
```

SFTP 同样支持 `privateKeyConnect(...)`。

## Telnet

```ts
k.net.telnetClient.connect(host, port, client => {
  client.writeLine("help");
  const output = client.read(10);
});
```

协议具有可靠响应终止符时，可使用 `terminatedRead(terminator, timeout?)`。

## 超时与失败

- SSH 和 SFTP 连接方法默认连接超时为 30 秒；连接或回调异常会继续向外抛出。
- SSH 的 `runCommand(command, timeout?)` 默认超时也是 30 秒。
- Telnet 的 `connect(..., timeout?)` 默认超时为 30 秒，但会捕获连接和回调异常；它没有错误返回值，连接失败时可能只是不执行回调。
