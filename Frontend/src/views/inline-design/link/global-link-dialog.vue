<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getElements } from "@/utils/dom";
import type { Link } from "./global-link-dialog";
import { close, list } from "./global-link-dialog";
import { doc, hoverElement } from "../page";
import { editAnchor } from "../features/edit-link";
import type { Change } from "../state/change";
import type { Operation } from "../state/operation";
import { getElementByRef } from "../utils/dom";
import { showConfirm } from "@/components/basic/confirm";

const { t } = useI18n();
const show = ref(true);

const operations: Operation<Change>[] = [];
const success = ref(false);

const editLink = async (row: Link) => {
  const elements = getElements(doc.value!);
  const element = getElementByRef(elements, row.id);
  if (!element) return;
  const operation = await editAnchor(element);

  if (operation) {
    row.url = element.getAttribute("href")!;
    operations.push(operation);
  }
};

const hoverRow = (row: Link) => {
  const elements = getElements(doc.value!);
  const element = getElementByRef(elements, row.id);
  if (!element) return;
  element.scrollIntoView({ behavior: "smooth" });
  hoverElement.value = element as HTMLElement;
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
    :title="t('common.globalLinkList')"
    draggable
    :before-close="beforeClose"
    custom-class="el-dialog--zero-padding"
    @closed="close(success, operations)"
  >
    <el-scrollbar v-if="list.length">
      <el-table :data="list" max-height="540px" @cell-mouse-enter="hoverRow">
        <el-table-column width="20px" />
        <el-table-column v-slot="{ row }" :label="t('common.url')">
          <div class="ellipsis" :title="row.url">{{ row.url }}</div>
        </el-table-column>

        <el-table-column v-slot="{ row }" :label="t('common.content')">
          <div class="ellipsis" :title="row.content">
            {{ row.content }}
          </div>
        </el-table-column>
        <el-table-column v-slot="{ row }" width="100" align="right">
          <el-button type="primary" round size="small" @click="editLink(row)">{{
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
</template>
