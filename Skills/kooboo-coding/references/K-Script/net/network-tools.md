# k.net Network Utilities

> DNS/Whois, IP geolocation, Ping, SSH, SFTP, and Telnet

## DNS and Whois

```ts
const addresses = k.net.DNS.getIPs('example.com')
const addressesFromServer = k.net.DNS.getIPs('8.8.8.8', 'example.com')
const reverse = k.net.DNS.getReverseDNS('8.8.8.8')
const mx = k.net.DNS.getMXs('example.com')
const nameservers = k.net.DNS.getNsServers('example.com')
const whois = k.net.DNS.whoisInfo('example.com')
```

Use the two-argument `getIPs(nameServer, host)` or `getMXs(nameServer, host)` overload when the query must use a specific DNS server. DNS lookup methods return `null` when the underlying lookup fails.

## IP

`IP` is PascalCase. Use `k.net.IP.myIP` for the current client address and the lookup methods for geolocation.

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

Use `privateKeyConnect(host, user, privateKey, callback, timeout?)` for PEM private-key authentication.

## SFTP

```ts
k.net.sftpClient.passwordConnect(host, user, password, client => {
  const data = client.download("/remote/report.csv");
  client.upload(data, "/remote/report-copy.csv");
});
```

SFTP also supports `privateKeyConnect(...)`.

## Telnet

```ts
k.net.telnetClient.connect(host, port, client => {
  client.writeLine("help");
  const output = client.read(10);
});
```

Use `terminatedRead(terminator, timeout?)` when the protocol has a reliable response terminator.

## Timeouts and failures

- SSH and SFTP connection methods default to a 30-second connection timeout and propagate connection or callback errors.
- SSH `runCommand(command, timeout?)` also defaults to 30 seconds.
- Telnet `connect(..., timeout?)` defaults to 30 seconds but catches connection and callback errors; it has no error return, so a failed connection may simply skip the callback.
