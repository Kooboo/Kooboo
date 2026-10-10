# 服务集成 · Storage

> Group：**Storage**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}&group=Storage`

**Storage** 分组将 **媒体库**、**内容文件** 等二进制资源存到第三方对象存储（或启用远程存储）。常见字段包括 **Endpoint**、**AccessKeyId**、**AccessKeySecret**、**Bucket**、**MediaLibraryFolder**、**CmsFileFolder**、**BindingDomain**、**Enable** / **EnableSSL** 等（以编辑弹窗为准）。

<DocImage src="/cms/settings/settings-integrations-storage.png" alt="Storage 分组" width="1120" />

## 集成项

| 名称 | 说明 |
|------|------|
| **AmazonS3** | AWS S3 |
| **AliyunOSS** | 阿里云 OSS |
| **AzureBlob** | Azure Blob（Alert 含 Endpoint / 容器示例） |

启用后，上传与 URL 生成走对应 Provider；未启用或未配置时通常使用站点本地/Kooboo 默认存储。

## 后台操作

1. 在云厂商创建 **Bucket/容器** 与访问密钥。  
2. **编辑** 对应 Storage 项，填写 Endpoint、密钥、Bucket、文件夹前缀。  
3. 可选 **BindingDomain** 自定义资源访问域名。  
4. **保存** 后上传 [媒体库](../../site/media.md) 文件验证 URL 可访问。

## 相关

- [服务集成概述](./index.md)  
- [媒体库](../../site/media.md)
