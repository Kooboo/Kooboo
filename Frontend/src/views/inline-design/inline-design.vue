<script lang="ts" setup>
import { onMounted, ref, provide } from "vue";
import { getQueryString, updateQueryString } from "@/utils/url";
import { api } from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { ContextMenu as InlineContextMenu, GlobalMenu } from "./menus";
import { InlineEditor } from "./inline-editor";
import { PageEditor, culture, DomBorder } from "./page";
import { show as showLabelDiag, label, close as closeLabelDiag } from "./label";
import { HtmlblockDialog, show as showHtmlblockDialog } from "./htmlblock";
import { RepeaterDialog, show as showRepeaterDialog } from "./repeater";
import { LinkDialog, show as showLinkDialog } from "./link/link-dialog";
import { ContentDialog, show as showContentDialog } from "./content";
import { ProductDialog, show as showProductDialog } from "./commerce";
import { ConfigDialog } from "./config";
import { StyleDialog, show as showStyleDialog } from "./style";
import { SvgDialog, show as showSvgDialog } from "./svg";
import { ColorDialog, show as showColorDialog } from "./color";
import { MenuDialog, show as showMenuDialog } from "./menu";
import { WidgetDialog, show as showWidgetDialog } from "./widget";
import {
  show as showImageDialog,
  close as closeImageDialog,
  multipleSelect,
} from "./image/image-dialog";
import { DatabaseDialog, show as showDatabaseDialog } from "./database";
import { useI18n } from "vue-i18n";
import LabelDialog from "@/views/content/labels/edit-dialog.vue";
import KeyValueDialog from "@/views/database/key-value/edit-value-dialog.vue";
import KMediaDialog from "@/components/k-media-dialog";
import {
  show as showKeyValueDiag,
  keyValueId,
  close as closeKeyValueDiag,
} from "./key-value";

import {
  GlobalImageDialog,
  show as showGlobalImageDialog,
} from "./image/global-image-dialog";

import {
  GlobalTextDialog,
  show as showGlobalTextDialog,
} from "./text/global-text-dialog";

import {
  ImageEditor as ImgEditor,
  show as showImageEditor,
} from "./image/image-editor";

import {
  GlobalLinkDialog,
  show as showGlobalLinkDialog,
} from "./link/global-link-dialog";
import { useAppStore } from "@/store/app";
import { useBindingStore } from "@/store/binding";
import { usePageDesigner } from "@/components/visual-editor/main";
import type { PostPage } from "@/api/pages/types";

const pageType = getQueryString("type");
const pageId = getQueryString("id");
let path = getQueryString("path")!;
if (!path?.startsWith("/")) path = "/" + path;
const html = ref<string>();
const { t } = useI18n();
const appStore = useAppStore();
appStore.load();
const bindingStore = useBindingStore();
bindingStore.loadBindings();
localStorage.setItem("vueuse-color-scheme", getQueryString("scheme")!);

const { page, allPages, initPageDesigner, onSave } = usePageDesigner();

async function onGlobalMenuSave(page: PostPage) {
  await onSave(page);
}

provide("ve-get-pages", () => {
  return allPages.value;
});
onMounted(async () => {
  if (pageId && pageType === "Designer") {
    await initPageDesigner(pageId);
  }
  const url = useUrlSiteId(updateQueryString(path, { koobooInline: "design" }));
  const { data, headers } = await api.get(url, { baseURL: "" });
  html.value = data;
  if (headers["k-culture"]) culture.value = headers["k-culture"];
});
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <div v-if="html" class="h-full relative bg-gray">
      <PageEditor :content="html" class="bg-fff" />
      <DomBorder />
      <InlineEditor />
      <InlineContextMenu :page="page" />
      <GlobalMenu :page="page" @save="onGlobalMenuSave" />
      <ImgEditor v-if="showImageEditor" />
      <GlobalImageDialog v-if="showGlobalImageDialog" />
      <HtmlblockDialog v-if="showHtmlblockDialog" />
      <LinkDialog v-if="showLinkDialog" />
      <GlobalLinkDialog v-if="showGlobalLinkDialog" />
      <GlobalTextDialog v-if="showGlobalTextDialog" />
      <ContentDialog v-if="showContentDialog" />
      <ProductDialog v-if="showProductDialog" />
      <ConfigDialog />
      <StyleDialog v-if="showStyleDialog" />
      <SvgDialog v-if="showSvgDialog" />
      <ColorDialog v-if="showColorDialog" />
      <MenuDialog v-if="showMenuDialog" />
      <DatabaseDialog v-if="showDatabaseDialog" />
      <LabelDialog
        v-if="showLabelDiag"
        v-model="showLabelDiag"
        :current="label!"
        :alert="t('common.immediatelyChange')"
        @success="closeLabelDiag"
      />
      <KeyValueDialog
        v-if="showKeyValueDiag"
        v-model="showKeyValueDiag"
        :current="keyValueId!"
        :alert="t('common.immediatelyChange')"
        @success="closeKeyValueDiag"
      />
      <RepeaterDialog v-if="showRepeaterDialog" />
      <KMediaDialog
        v-if="showImageDialog"
        v-model="showImageDialog"
        :multiple="multipleSelect"
        @choose="closeImageDialog"
      />
      <WidgetDialog
        v-if="showWidgetDialog && page"
        v-model="showWidgetDialog"
        :page="page"
      />
      <div />
    </div>
  </el-config-provider>
</template>
