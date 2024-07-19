<template>
  <el-form-item
    :label="field.displayName ?? field.name"
    :prop="field.prop"
    :required="field.required"
    :class="cssClass"
  >
    <VueDraggable
      class="w-full"
      item-key="value"
      :list="list"
      :disabled="list.length < 2"
      :group="{
        name: 've-items-control',
        pull: 'clone',
        put: false,
      }"
      handle=".icon-move"
    >
      <template #item="{ element, index }">
        <div
          class="mb-8 text-666 dark:text-fff/67 flex items-center pl-16 pr-8 w-full border-line dark:border-666 rounded-normal border border-solid ellipsis bg-fff dark:bg-[#333]"
          data-cy="added-item"
        >
          <template v-if="list.length > 1">
            <div class="pr-8"># {{ index + 1 }}</div>
          </template>
          <div
            class="ve-list-item-preview overflow-hidden leading-40px flex-1 mr-8 ellipsis"
            data-cy="text"
          >
            <div>{{ summaryText(element.value) }}</div>
          </div>
          <div class="p-4 cursor-pointer">
            <el-icon
              class="iconfont icon-a-writein"
              data-cy="edit"
              @click="onEdit(element, index)"
            />
          </div>
          <div class="p-4 cursor-pointer">
            <el-icon
              class="text-orange iconfont icon-delete"
              data-cy="remove"
              @click="onDelete(index)"
            />
          </div>
          <div class="p-4">
            <el-icon class="iconfont icon-move cursor-move" data-cy="move" />
          </div>
        </div>
      </template>
      <template #footer>
        <el-button circle data-cy="add" @click="onAdd">
          <el-icon class="text-blue iconfont icon-a-addto" />
        </el-button>
      </template>
    </VueDraggable>
    <el-dialog
      v-if="editingItem"
      v-model="visible"
      :close-on-click-modal="false"
      :title="activeIndex > -1 ? t('common.edit') : t('common.create')"
      destroy-on-close
      append-to-body
      @opened="setOverlay"
      @closed="restoreOverlay"
      @close="handleClose"
    >
      <el-form
        ref="form"
        class="el-form--label-normal"
        :model="editingItem"
        label-position="top"
        @submit.prevent
      >
        <el-form-item prop="content" :label="t('common.content')">
          <KEditor
            v-model="editingItem.value"
            min_height="200px"
            v-bind="editorAttrs"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <DialogFooterBar @confirm="onSaveItem" @cancel="handleClose" />
      </template>
    </el-dialog>
  </el-form-item>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import { watch, ref, computed } from "vue";
import { newGuid } from "@/utils/guid";
import { useI18n } from "vue-i18n";
import VueDraggable from "vuedraggable";
import KEditor from "@/components/k-editor/index.vue";
import { setOverlay, restoreOverlay } from "@/utils/dialog";
import { cloneDeep } from "lodash-es";
import type { KeyValue } from "@/global/types";
import { isClassic } from "../../../utils";
import { extendEmailEditorButtons } from "@/components/visual-editor/utils/editor";

const { t } = useI18n();
const props = defineProps<{
  model: Record<string, any>;
  field: Field;
  cssClass?: any;
}>();

const list = ref<KeyValue[]>([]);
const visible = ref(false);
const activeIndex = ref<number>(-1);
const editingItem = ref<KeyValue>();

const defaultToolbar =
  "undo redo | fontselect fontsizeselect bold italic forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | image link";
const editorAttrs = computed<Record<string, any>>(() => {
  if (!isClassic()) {
    return {
      toolbar: defaultToolbar,
    };
  }

  const external = extendEmailEditorButtons();
  return {
    toolbar: defaultToolbar + " " + external["additional-toolbar-buttons"],
    forced_root_block: "",
    force_br_newlines: true,
    force_p_newlines: false,
    ...external,
  };
});

function init() {
  const json: string = props.model[props.field.name] || "[]";
  if (typeof json === "string") {
    list.value = JSON.parse(json);
  }
}
init();

watch(
  () => list.value,
  (data) => {
    props.model[props.field.name] = JSON.stringify(data);
  },
  {
    deep: true,
  }
);

function onAdd() {
  editingItem.value = {
    key: newGuid(),
    value: "",
  };
  activeIndex.value = -1;
  visible.value = true;
}

function onDelete(index: number) {
  list.value.splice(index, 1);
}

function onEdit(item: KeyValue, index: number) {
  editingItem.value = cloneDeep(item);
  visible.value = true;
  activeIndex.value = index;
}

function onSaveItem() {
  if (!editingItem.value) {
    return;
  }
  if (activeIndex.value > -1) {
    list.value.splice(activeIndex.value, 1, editingItem.value);
  } else {
    list.value.push(editingItem.value);
  }
  visible.value = false;
  editingItem.value = undefined;
}

function handleClose() {
  visible.value = false;
}

function summaryText(html: string) {
  const el = document.createElement("div");
  el.innerHTML = html;
  return el.innerText;
}
</script>
<style scoped lang="scss">
.ve-list-item-preview {
  pointer-events: none;
}
</style>
