<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import ImagePreview from "@/components/basic/image-preview.vue";
import { close, images } from "./global-image-dialog";
import type { ImageBinding } from "./global-image-dialog";
import { editImage } from "../features/edit-image";
import type { Operation } from "../state/operation";
import type { Change, StyleChange } from "../state/change";
import { editStyle } from "../features/edit-style";
import { getElementByRef } from "../utils/dom";
import { getElementParents, getElements } from "@/utils/dom";
import { doc, hoverElement } from "../page";
import { createStyleOperation } from "../state/operation/style-operation";
import { chooseImage } from "./image-dialog";
import {
  domSources,
  getBinding,
  getKoobooBindings,
  getKoobooId,
} from "../binding";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const show = ref(true);
const operations: Operation<Change>[] = [];
const success = ref(false);
const changeImage = async (image: ImageBinding) => {
  switch (image.type) {
    case "src":
      changeSrcImage(image);
      break;

    case "inline":
      changeInlineStyleImage(image);
      break;

    case "style":
      changeStyleImage(image);
      break;
    default:
      break;
  }
};

async function changeSrcImage(image: ImageBinding) {
  const elements = getElements(doc.value!);
  const element = getElementByRef(elements, image.id!);
  if (!element) return;
  const operation = await editImage(element);

  if (operation) {
    image.url = element.getAttribute("src")!;
    operations.push(operation);
  }
}

async function changeInlineStyleImage(image: ImageBinding) {
  const elements = getElements(doc.value!);
  const element = getElementByRef(elements, image.id!);
  if (!element) return;
  const operation = await editStyle(element);
  if (operation) {
    image.url = element.style.backgroundImage;
    operations.push(operation);
  }
}

async function changeStyleImage(image: ImageBinding) {
  if (!image.element || !image.rule) return;
  const property = "background-image";
  const priority = image.rule.style.getPropertyPriority(property);
  const result = await chooseImage();
  const value = `url('${result[0].url}')`;
  const operation = createStyleOperation(image.rule, property);
  image.rule.style.setProperty(property, value, priority);
  const bindings = getKoobooBindings(image.element);
  const binding = getBinding(bindings, domSources);

  operation.changes.push({
    source: binding?.source,
    id: binding?.id,
    action: "styleSheet",
    mediarulelist: image.media,
    property: property,
    selector: image.rule.selectorText,
    url: image.file,
    koobooId: getKoobooId(image.element!),
    important: priority,
    value: value,
  } as StyleChange);
  operations.push(operation);
  image.url = value;
}

function hoverImage(image: ImageBinding) {
  if (!image.id) return;
  const elements = getElements(doc.value!);
  const element = getElementByRef(elements, image.id);
  if (!element) return;
  element.scrollIntoView({ behavior: "smooth" });
  hoverElement.value = element;
}

const getFileName = (url?: string) => {
  if (!url) return t("common.page");
  const span = url.split("/");
  return span[span.length - 1];
};

const getPath = (image: ImageBinding) => {
  const path = [];

  path.push(getFileName(image.file));

  if (image.id) {
    const elements = getElements(doc.value!);
    const element = getElementByRef(elements, image.id);

    if (element) {
      path.push(
        ...getElementParents(element, true)
          .map((m) => m.nodeName?.toLowerCase())
          .reverse()
          .filter((f) => !["html", "body"].includes(f))
      );
    }
  }

  if (image.rule?.selectorText) {
    path.push(image.rule.selectorText);
  }

  return path.filter((f) => !!f).join(" > ");
};

const getType = (image: ImageBinding) => {
  switch (image.type) {
    case "style":
      return t("common.style");
    case "inline":
      return t("common.inlineStyle");
    case "src":
      return t("common.image");
    default:
      return "";
  }
};

const save = () => {
  success.value = true;
  show.value = false;
};

async function beforeClose(done: any) {
  if (!success.value && operations.length) {
    await showConfirm(t("common.unsavedChangesLeaveTips"));
  }
  done();
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.globalImageList')"
    draggable
    custom-class="el-dialog--zero-padding"
    :before-close="beforeClose"
    @closed="close(success, operations)"
  >
    <el-scrollbar v-if="images?.length">
      <el-table
        :data="images"
        max-height="540px"
        @cell-mouse-enter="hoverImage"
        @cell-click="changeImage"
      >
        <el-table-column width="20" />
        <el-table-column :label="t('common.path')">
          <template #default="{ row }">
            <div class="ellipsis" :title="getPath(row)">{{ getPath(row) }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.type')">
          <template #default="{ row }">
            <el-tag type="success" round>{{ getType(row) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column align="right" width="120">
          <template #default="{ row }">
            <div class="inline-flex">
              <ImagePreview
                :src="row.url"
                :has-url-wrap="row.type != 'src'"
                class="h-80px w-80px overflow-hidden"
              />
            </div>
          </template>
        </el-table-column>
        <el-table-column width="20" />
      </el-table>
    </el-scrollbar>
    <div v-else>
      <el-empty />
    </div>
    <template v-if="images?.length" #footer>
      <el-button round @click="beforeClose(() => (show = false))">{{
        t("common.cancel")
      }}</el-button>
      <el-button type="primary" round @click="save">{{
        t("common.save")
      }}</el-button>
    </template>
  </el-dialog>
</template>
