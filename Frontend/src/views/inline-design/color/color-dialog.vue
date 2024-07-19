<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Color } from "@/global/color";
import { getColorSelections } from "@/global/color";
import ColorPicker from "@/components/basic/color-picker.vue";
import { hoverElement, doc, win } from "../page";
import { close } from ".";
import { createStyleOperation } from "../state/operation/style-operation";
import type { Change, StyleChange } from "../state/change";
import type { Operation } from "../state/operation";
import { createDomOperation } from "../state/operation";
import { colors } from ".";
import { ensurePlaceholderAttribute, tryGetPureParent } from "../state";
import {
  domSources,
  getBinding,
  getKoobooBindings,
  getKoobooId,
} from "../binding";
import { active, getChange } from "../features/edit-style";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const show = ref(true);
const operations: Operation<Change>[] = [];
const success = ref(false);
const selections = getColorSelections(doc.value, win.value);

const getFileName = (url?: string) => {
  if (!url) return t("common.page");
  const span = url.split("/");
  return span[span.length - 1];
};

const getPath = (color: Color) => {
  return [getFileName(color.file), color.media, color.selectorText]
    .filter((f) => !!f)
    .join(" > ");
};
// 模态框修改颜色
const change = (color: Color) => {
  if (color.type == "inline") {
    const { element, isSelf } = tryGetPureParent(color.owner)!;
    const targetElement = isSelf ? color.owner.parentElement! : element;
    const originContent = targetElement.innerHTML;
    const priority = color.owner.style.getPropertyPriority(color.property);
    color.owner.style.setProperty(color.property, color.value, priority);
    const id = ensurePlaceholderAttribute(targetElement);
    const operation = createDomOperation(id, originContent);
    const change = getInlineChange(color);
    if (!change) throw new Error("can not get change");
    operation.changes.push(change);
    operations.push(operation);
  } else if (color.rule) {
    const operation = createStyleOperation(color.rule, color.property);

    const priority = color.rule.style.getPropertyPriority(color.property);
    color.rule.style.setProperty(color.property, color.value, priority);
    const bindings = getKoobooBindings(color.owner);
    const binding = getBinding(bindings, domSources);

    operation.changes.push({
      source: binding?.source,
      id: binding?.id,
      action: "styleSheet",
      mediarulelist: color.media,
      property: color.property,
      selector: color.selectorText,
      url: color.file,
      koobooId: getKoobooId(color.owner),
      important: priority,
      value: color.value,
    } as StyleChange);
    operations.push(operation);
  }
};

function getInlineChange(color: Color) {
  if (!active(color.owner)) return;
  return getChange(color.owner);
}

const focusElement = (el: HTMLElement) => {
  el.scrollIntoView({ behavior: "smooth" });
  hoverElement.value = el;
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
    :title="t('common.colorList')"
    draggable
    custom-class="el-dialog--zero-padding"
    :before-close="beforeClose"
    @closed="close(success, operations)"
  >
    <el-scrollbar v-if="colors.length">
      <el-table :data="colors" max-height="540px">
        <el-table-column width="20px" />
        <el-table-column v-slot="{ row }" :label="t('common.path')">
          <div class="ellipsis" :title="getPath(row)">{{ getPath(row) }}</div>
        </el-table-column>

        <el-table-column
          v-slot="{ row }"
          :label="t('common.references')"
          width="100"
          align="center"
        >
          <el-popover
            placement="auto"
            trigger="hover"
            :width="240"
            @before-enter="row.renderReferences = true"
            @after-leave="row.renderReferences = false"
          >
            <template #reference>
              <el-tag round class="cursor-pointer">{{
                row.references.length
              }}</el-tag>
            </template>
            <el-scrollbar v-if="row.renderReferences" height="180px">
              <div class="space-x-4 space-y-4">
                <el-tag
                  v-for="(tag, index) of row.references"
                  :key="index"
                  class="cursor-pointer"
                  size="small"
                  round
                  @pointerenter="focusElement(tag)"
                  >{{ tag.tagName }}</el-tag
                >
              </div>
            </el-scrollbar>
          </el-popover>
        </el-table-column>

        <el-table-column
          v-slot="{ row }"
          :label="t('common.property')"
          width="150"
        >
          <div class="flex">
            <el-tag type="success" round :title="row.property">{{
              row.property
            }}</el-tag>
          </div>
        </el-table-column>

        <el-table-column
          v-slot="{ row }"
          :label="t('common.value')"
          width="230"
        >
          <ColorPicker
            v-slot="{ color }"
            v-model="row.value"
            show-alpha
            :predefine="selections"
            @update:model-value="change(row)"
          >
            <div
              class="flex items-center space-x-8 p-4 max-w-230px w-full justify-end"
            >
              <p class="flex-1 ellipsis">{{ row.value }}</p>
              <div
                :style="{ backgroundColor: color }"
                class="w-24 h-24 rounded-4px shadow-s-10"
              />
            </div>
          </ColorPicker>
        </el-table-column>
        <el-table-column width="20px" />
      </el-table>
    </el-scrollbar>
    <el-empty v-else />
    <template v-if="colors.length" #footer>
      <el-button round @click="beforeClose(() => (show = false))">{{
        t("common.cancel")
      }}</el-button>
      <el-button type="primary" round @click="save">{{
        t("common.save")
      }}</el-button>
    </template>
  </el-dialog>
</template>
