# Integrations · Storage

> Group: **Storage**  
> Deep link: `/_Admin/system/config?SiteId={siteGuid}&group=Storage`

The **Storage** group stores **media library**, **content files**, and other binaries on third-party object storage (or enables remote storage). Common fields: **Endpoint**, **AccessKeyId**, **AccessKeySecret**, **Bucket**, **MediaLibraryFolder**, **CmsFileFolder**, **BindingDomain**, **Enable** / **EnableSSL** (per edit dialog).

<DocImage src="/cms/settings/settings-integrations-storage.png" alt="Storage group" width="1120" />

## Integration Items

| Name | Description |
|------|-------------|
| **AmazonS3** | AWS S3 |
| **AliyunOSS** | Alibaba Cloud OSS |
| **AzureBlob** | Azure Blob (Alert includes Endpoint / container examples) |

When enabled, upload and URL generation use the provider; otherwise site local / default Kooboo storage.

## Admin Steps

1. Create **Bucket/container** and access keys at cloud provider.  
2. **Edit** Storage item: Endpoint, keys, Bucket, folder prefixes.  
3. Optional **BindingDomain** for custom asset domain.  
4. **Save**, upload [Media library](../../site/media.md) file and verify URL.

## Related

- [Integrations overview](./index.md)  
- [Media library](../../site/media.md)
