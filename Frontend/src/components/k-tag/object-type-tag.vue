<script lang="ts" setup>
import { computed } from "vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{ type: string }>();
const { t } = useI18n();

const defines = [
  {
    name: "Page",
    display: t("common.page"),
    elType: "success",
  },
  {
    name: "Script",
    display: t("common.script"),
    elType: "warning",
  },
  {
    name: "Style",
    display: t("common.style"),
    elType: "",
  },
  {
    name: "Layout",
    display: t("common.layout"),
    elType: "",
  },
  {
    name: "Label",
    display: t("common.label"),
    elType: "",
  },
  {
    name: "View",
    display: t("common.view"),
    elType: "success",
  },
  {
    name: "HtmlBlock",
    display: t("common.htmlBlock"),
    elType: "error",
  },
  {
    name: "CssRule",
    display: t("common.cssRule"),
    elType: "",
  },
  {
    name: "CmsFile",
    display: t("common.file"),
    elType: "",
  },
  {
    name: "Route",
    display: t("common.route"),
    elType: "",
  },
  {
    name: "ObjectRelation",
    display: "ObjectRelation",
    elType: "",
  },
  {
    name: "Image",
    display: t("common.image"),
    elType: "",
  },
  {
    name: "TextContent",
    display: t("common.textContent"),
    elType: "",
  },
  {
    name: "Binding",
    display: "Binding",
    elType: "",
  },
  {
    name: "Menu",
    display: t("common.menu"),
    elType: "warning",
  },
  {
    name: "WebSite",
    display: t("common.webSite"),
    elType: "",
  },
  {
    name: "User",
    display: t("common.user"),
    elType: "",
  },
  {
    name: "ContentFolder",
    display: t("common.contentFolder"),
    elType: "",
  },
  {
    name: "Domain",
    display: t("common.domain"),
    elType: "",
  },
  {
    name: "UserGroup",
    display: t("common.userGroup"),
    elType: "",
  },
  {
    name: "ExternalResource",
    display: t("common.externalResource"),
    elType: "",
  },
  {
    name: "Thumbnail",
    display: "Thumbnail",
    elType: "",
  },
  {
    name: "Folder",
    display: t("common.folder"),
    elType: "",
  },
  {
    name: "CssDeclaration",
    display: "CssDeclaration",
    elType: "",
  },
  {
    name: "Notification",
    display: "Notification",
    elType: "",
  },
  {
    name: "DataMethodSetting",
    display: t("common.dataMethodSetting"),
    elType: "",
  },
  {
    name: "Link",
    display: t("common.link"),
    elType: "",
  },
  {
    name: "Embedded",
    display: t("common.embedded"),
    elType: "",
  },
  {
    name: "Form",
    display: t("common.form"),
    elType: "",
  },
  {
    name: "SyncSetting",
    display: "SyncSetting",
    elType: "",
  },
  {
    name: "ContentCategory",
    display: t("common.contentCategory"),
    elType: "",
  },
  {
    name: "ContentType",
    display: t("common.contentType"),
    elType: "",
  },
  {
    name: "FormValue",
    display: "FormValue",
    elType: "",
  },
  {
    name: "ViewDataMethod",
    display: t("common.viewDataMethod"),
    elType: "",
  },
  {
    name: "Unknown",
    display: t("common.unknown"),
    elType: "",
  },
  {
    name: "KoobooSystem",
    display: "KoobooSystem",
    elType: "",
  },
  {
    name: "ResourceGroup",
    display: t("type.resourceGroup"),
    elType: "",
  },
  {
    name: "Synchronization",
    display: "Synchronization",
    elType: "",
  },
  {
    name: "DiskSync",
    display: "DiskSync",
    elType: "",
  },
  {
    name: "SiteCluster",
    display: "SiteCluster",
    elType: "",
  },
  {
    name: "ContinueConverter",
    display: "ContinueConverter",
    elType: "",
  },
  {
    name: "FormSetting",
    display: t("type.formSetting"),
    elType: "",
  },
  {
    name: "RolePermission",
    display: t("common.role"),
    elType: "",
  },
  {
    name: "TransferTask",
    display: t("common.transferTask"),
    elType: "",
  },
  {
    name: "TransferPage",
    display: t("common.transferPage"),
    elType: "",
  },
  {
    name: "SiteUser",
    display: t("common.siteUser"),
    elType: "",
  },
  {
    name: "Code",
    display: t("common.code"),
    elType: "",
  },
  {
    name: "CoreSetting",
    display: t("common.systemConfig"),
    elType: "",
  },
  {
    name: "BusinessRule",
    display: t("type.frontEvent"),
    elType: "",
  },
  {
    name: "Order",
    display: "Order",
    elType: "",
  },
  {
    name: "OrderDetail",
    display: "OrderDetail",
    elType: "",
  },
  {
    name: "Product",
    display: "Product",
    elType: "",
  },
  {
    name: "Category",
    display: "Category",
    elType: "",
  },
  {
    name: "Cateogry",
    display: "Cateogry",
    elType: "",
  },
  {
    name: "ProductCategory",
    display: "ProductCategory",
    elType: "",
  },
  {
    name: "ProductVariants",
    display: "ProductVariants",
    elType: "",
  },
  {
    name: "ProductType",
    display: "ProductType",
    elType: "",
  },
  {
    name: "PaymentCallback",
    display: t("common.paymentCallback"),
    elType: "",
  },
  {
    name: "PaymentRequest",
    display: "PaymentRequest",
    elType: "",
  },
  {
    name: "Customer",
    display: "Customer",
    elType: "",
  },
  {
    name: "ShippingAddress",
    display: "ShippingAddress",
    elType: "",
  },
  {
    name: "kfile",
    display: "kfile",
    elType: "",
  },
  {
    name: "Kconfig",
    display: t("common.kConfig"),
    elType: "",
  },
  {
    name: "TableRelation",
    display: t("common.tableRelation"),
    elType: "",
  },
  {
    name: "DatabaseTable",
    display: t("common.databaseTable"),
    elType: "",
  },
  {
    name: "TableSchemaMapping",
    display: t("common.tableSchemaMapping"),
    elType: "",
  },
  {
    name: "Table",
    display: t("common.tableData"),
    elType: "",
  },
  {
    name: "ScriptModule",
    display: t("common.scriptModule"),
    elType: "",
  },
  {
    name: "SiteJob",
    display: t("common.job"),
    elType: "",
  },
  {
    name: "SpaMultilingual",
    display: t("common.spaMultilingual"),
    elType: "",
  },
  {
    name: "OpenApiAuthorize",
    display: t("common.openApiAuthorize"),
    elType: "",
  },
];

const define = computed(() => {
  const found = defines.find(
    (f) => f.name?.toLowerCase() === props.type?.toLowerCase()
  );
  if (found) return found;
  return {
    name: props.type,
    display: props.type,
    elType: "",
  };
});
</script>

<template>
  <el-tag
    class="rounded-full"
    :type="(define.elType as any)"
    size="small"
    data-cy="resource-type"
  >
    {{ define.display }}
  </el-tag>
</template>
