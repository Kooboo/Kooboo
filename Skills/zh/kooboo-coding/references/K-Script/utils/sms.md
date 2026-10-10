# k.utils.sms

> 短信发送（阿里云 / 腾讯云 / 中国移动）

## 概述

`k.utils.sms` 按站点后台「服务集成 → SMS」配置发送短信。发送前须在后台配置对应厂商密钥与模板。

完整配置步骤与邮件场景见 [Send Email And SMS Scenario](../../Scenario/send-email-and-sms.md)（Scenario 内示例已统一为 **`k.utils.sms.*`**）。

## 结构

```ts
interface SMS {
  aliSMS: Ali;
  tencent: Tencent;
  chinaMobile: ChinaMobile;
}
```

## 阿里云 aliSMS

```ts
// 单键值模板参数
k.utils.sms.aliSMS.send("SMS_489735268", "+8613812345678", "code", "123456");

// 多参数对象
k.utils.sms.aliSMS.send("SMS_489735268", "+8613812345678", {
  code: "123456",
});

// 国际号码
k.utils.sms.aliSMS.sendInternational(
  "SMS_489735268",
  "+8115312345678",
  "code",
  "12345"
);
```

未配置阿里云短信时抛出 `Ali SMS service not configured`。
国内 `send()` 的号码如果不以 `+` 开头，会自动添加 `+86`。`sendInternational()` 要求配置国际签名，并按原样使用传入号码。服务商返回成功时返回 `true`；配置或服务商失败时抛出异常。

## 腾讯云 tencent

```ts
k.utils.sms.tencent.send("templateId", "+8613812345678", {
  code: "123456",
});
```

号码如果不以 `+` 开头，运行时会自动添加 `+86`。只有第一个服务商状态为 `Ok` 时返回 `true`；状态缺失或不是 `Ok` 时返回 `false`。缺少配置或客户端调用失败时可能抛出异常。

## 中国移动 chinaMobile

```ts
// 单号
k.utils.sms.chinaMobile.send("+8613812345678", "短信正文");

// 多号
k.utils.sms.chinaMobile.send(["+8613812345678", "+8613912345678"], "短信正文");
```

号码字符串会按传入内容交给中国移动客户端。发送成功返回 `true`；缺少配置或服务商返回失败时抛出异常。

## 注意事项

1. **后台配置** — 模板 Code、签名、密钥均在站点设置中维护，脚本内不写死密钥。
2. **凭据与错误** — 服务商凭据、签名和模板设置从当前站点后台配置读取。不要把返回 `false` 与配置或服务商异常等同处理。
