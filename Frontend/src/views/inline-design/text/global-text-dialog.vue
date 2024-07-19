<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getElements } from "@/utils/dom";
import type { TextNode } from "./global-text-dialog";
import { close, list } from "./global-text-dialog";
import { doc, hoverElement } from "../page";
import type { Change } from "../state/change";
import type { Operation } from "../state/operation";
import { createDomOperation } from "../state/operation";
import { getElementByRef } from "../utils/dom";
import TextEditorDialog from "./text-editor-dialog.vue";
import { ensurePlaceholderAttribute, tryGetPureParent } from "../state";
import { getChange } from "../features/edit-dom";
import { Completer } from "@/utils/lang";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const show = ref(true);
const operations: Operation<Change>[] = [];
const success = ref(false);
const currentTextNode = ref<TextNode>();
let completer = ref<Completer<string>>();

const editText = async (row: TextNode) => {
  const elements = getElements(doc.value!);
  const el = getElementByRef(elements, row.id);
  if (!el) return;
  const { element } = tryGetPureParent(el)!;
  const originContent = element.innerHTML;
  currentTextNode.value = row;
  completer.value = new Completer<string>();
  const value = await completer.value.promise;
  row.text.data = value;
  row.content = value;
  const id = ensurePlaceholderAttribute(element);
  const operation = createDomOperation(id, originContent);
  const change = getChange(el);
  if (!change) throw new Error("can not get change");
  operation.changes.push(change);
  operations.push(operation);
};

const hoverRow = (row: TextNode) => {
  const elements = getElements(doc.value!);
  const element = getElementByRef(elements, row.id);
  if (!element) return;
  element.scrollIntoView({ behavior: "smooth" });
  hoverElement.value = element as HTMLElement;
};

const textEditorDialogClosed = (changed: boolean, value: string) => {
  if (changed) {
    completer.value?.resolve(value);
  } else {
    completer.value?.reject(value);
  }
  completer.value = undefined;
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
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.globalTextList')"
    draggable
    :before-close="beforeClose"
    custom-class="el-dialog--zero-padding"
    @closed="close(success, operations)"
  >
    <el-scrollbar v-if="list.length">
      <el-table :data="list" max-height="540px" @cell-mouse-enter="hoverRow">
        <el-table-column width="20px" />
        <el-table-column v-slot="{ row }" :label="t('common.text')">
          <div class="ellipsis" :title="row.content">
            {{ row.content }}
          </div>
        </el-table-column>
        <el-table-column v-slot="{ row }" width="100" align="right">
          <el-button type="primary" round size="small" @click="editText(row)">{{
            t("common.edit")
          }}</el-button>
        </el-table-column>
        <el-table-column width="20" />
      </el-table>
    </el-scrollbar>
    <div v-else>
      <el-empty />
    </div>
    <template v-if="list.length" #footer>
      <el-button round @click="beforeClose(() => (show = false))">{{
        t("common.cancel")
      }}</el-button>
      <el-button type="primary" round @click="save">{{
        t("common.save")
      }}</el-button>
    </template>
  </el-dialog>
  <TextEditorDialog
    v-if="!!completer"
    :value="currentTextNode?.content ?? ''"
    @finish="textEditorDialogClosed"
  />
</template>
