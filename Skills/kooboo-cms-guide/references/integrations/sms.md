# Service Integrations - SMS

## Page Purpose

- The SMS group stores account, application, signature, and region configuration for Alibaba Cloud, Tencent Cloud, and China Mobile SMS services, for use by the KScript SMS entry.
- This page does not create SMS templates, approve signatures, purchase quota, automatically select a provider, or send test messages. Templates, signatures, account status, and quota remain managed by the corresponding SMS platform.

## Provider Differences

- Alibaba Cloud requires an access key, region, and domestic signature; international sending uses a separate international signature. A domestic number without `+` receives `+86` as a China number.
- Tencent Cloud requires SecretId, SecretKey, application ID, signature, and region; a number without `+` also receives `+86`, and template parameters are sent in the order supplied at invocation.
- China Mobile uses enterprise name, interface account, key, and signature and supports sending content to one or multiple numbers; its template-call mode differs from Alibaba Cloud and Tencent Cloud.
- The three providers differ in success checks and error responses; fields, template IDs, or return semantics from one provider must not be applied to another.

## Saving, Sending, and Results

- Saving writes site configuration only and does not request the provider. Required fields are checked and the external service is called only during a real send.
- Alibaba Cloud is successful only when it returns a success code; current Tencent Cloud handling checks whether the first send status is successful; China Mobile throws an error when the service returns failure. A successful method return proves only that the provider accepted the call, not that the phone received the message.
- The current configuration page is not a unified send log, retry queue, or delivery report. Send frequency, failed retries, idempotency, abuse prevention, and business-verification-code expiration require both calling code and provider policy.

## Risks and Verification

- Changing a key, signature, or application ID affects later sends and does not retract sent messages. An unapproved signature or template, invalid number format, insufficient quota, wrong region, or provider risk control can cause failure.
- Before release, use controlled numbers to verify domestic and international number formats, template parameters, signatures, error responses, and actual delivery separately; a successful configuration save is not evidence that SMS is usable.
