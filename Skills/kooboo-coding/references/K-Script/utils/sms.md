# k.utils.sms

> SMS sending (Alibaba Cloud / Tencent Cloud / China Mobile)

## Overview

`k.utils.sms` sends SMS according to the Site admin configuration under “Service integrations → SMS.” Configure the provider credentials and template in the admin before sending.

See [Send Email And SMS Scenario](../../Scenario/send-email-and-sms.md) for complete configuration steps and email scenarios (examples in the Scenario now consistently use **`k.utils.sms.*`**).

## Structure

```ts
interface SMS {
  aliSMS: Ali;
  tencent: Tencent;
  chinaMobile: ChinaMobile;
}
```

## Alibaba Cloud aliSMS

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

Throws `Ali SMS service not configured` when Alibaba Cloud SMS is not configured.
For domestic `send()`, a phone number without a leading `+` is prefixed with `+86`. `sendInternational()` requires the international signature configuration and uses the supplied number as-is. A successful provider response returns `true`; configuration or provider failures throw.

## Tencent Cloud tencent

```ts
k.utils.sms.tencent.send("templateId", "+8613812345678", {
  code: "123456",
});
```

The runtime prefixes a number without a leading `+` with `+86`. It returns `true` only when the first provider status is `Ok`; a missing or non-`Ok` status returns `false`. Missing configuration or client failures can throw.

## China Mobile chinaMobile

```ts
// 单号
k.utils.sms.chinaMobile.send("+8613812345678", "短信正文");

// 多号
k.utils.sms.chinaMobile.send(["+8613812345678", "+8613912345678"], "短信正文");
```

The number strings are passed to the China Mobile client as supplied. A successful send returns `true`; missing configuration or a failed provider result throws.

## Notes

1. **Admin configuration** — Maintain template Code, signature, and credentials in Site settings; do not hard-code credentials in scripts.
2. **Credentials and errors** — Provider credentials, signatures, and template settings are read from the current site's admin configuration. Do not treat a `false` result as equivalent to a thrown configuration or provider error.
