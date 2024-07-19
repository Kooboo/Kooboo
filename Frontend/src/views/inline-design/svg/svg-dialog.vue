<script lang="ts" setup>
import { provide, ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import type { DomValueWrapper } from "@/global/types";
import { close, element } from ".";
import SvgEditor from "@/components/svg-editor/index.vue";

const { t } = useI18n();
const show = ref(true);
const values = ref<DomValueWrapper[]>([]);
let success = ref(false);
const showEditor = ref(false);
const editor = ref();

setTimeout(() => {
  showEditor.value = true;
}, 100);

const save = () => {
  success.value = true;
  closed();
};

const closed = () => {
  const result = editor.value.getSvgString();
  close(success.value, result);
};

const svgString = computed(() => {
  if (!element.value) return "";
  const clonedElement = element.value.cloneNode(true) as HTMLElement;
  const xmlns = clonedElement.getAttribute("xmlns");
  if (!xmlns) {
    clonedElement.setAttribute("xmlns", "http://www.w3.org/2000/svg");
  }

  return clonedElement.outerHTML;
});

provide("values", values.value);
</script>
<template>
  <el-dialog
    v-model="show"
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.svgEditor')"
    custom-class="el-dialog--zero-padding"
    draggable
    @closed="closed"
  >
    <div v-if="showEditor" class="h-600px">
      <SvgEditor
        ref="editor"
        :svg="svgString"
        alt=""
        ext="svg"
        name=""
        url=""
        site-url=""
      />
    </div>

    <template #footer>
      <el-button round @click="show = false">{{
        t("common.cancel")
      }}</el-button>
      <el-button type="primary" round @click="save">{{
        t("common.save")
      }}</el-button>
    </template>
  </el-dialog>
</template>
