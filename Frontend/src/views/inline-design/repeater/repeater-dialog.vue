<script lang="ts" setup>
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { close, element } from ".";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { getChildElements, stringToDom } from "@/utils/dom";
import { newGuid } from "@/utils/guid";

const { t } = useI18n();
const success = ref(false);
const show = ref(true);
const tinymceAuxNewContainer = ref<HTMLDialogElement>();
const list = ref<HTMLElement[]>([]);

const load = () => {
  const children = getChildElements(element.value!);

  for (const i of children) {
    if (!(i as any).__edit_repeater_id) {
      (i as any).__edit_repeater_id = newGuid();
    }
  }

  list.value = children;
};

onMounted(load);

// fixed element dialog and tinymce dialog conflict
setTimeout(() => {
  const tinymceContainer = document.querySelectorAll(".tox-tinymce-aux");
  tinymceContainer.forEach((f) => {
    tinymceAuxNewContainer.value?.appendChild(f);
  });
}, 500);

const onUpdateItem = (el: HTMLElement, value: string) => {
  el.innerHTML = value;
};

const onCopyItem = (item: HTMLElement) => {
  const copiedElement = stringToDom(item.outerHTML) as Element;
  item.parentElement!.insertBefore(copiedElement!, item.nextSibling);
  load();
};

const onDeleteItem = (item: HTMLElement) => {
  element.value?.removeChild(item);
  load();
};

const save = async () => {
  success.value = true;
  show.value = false;
};
</script>
<template>
  <el-dialog
    v-model="show"
    width="1000px"
    :close-on-click-modal="false"
    :title="t('common.editRepeater')"
    append-to-body
    custom-class="el-dialog--zero-padding"
    @closed="close(success)"
  >
    <div class="px-32 py-24 space-y-12">
      <div
        v-for="item in list"
        :key="(item as any).__edit_repeater_id"
        class="space-x-8 flex"
      >
        <div class="flex-1">
          <KEditor
            :model-value="item.innerHTML"
            :min_height="180"
            @update:model-value="onUpdateItem(item, $event)"
          />
        </div>
        <div class="flex flex-col items-center justify-center space-y-8 ml-8">
          <div>
            <IconButton
              class="text-blue"
              icon="icon-copy"
              :tip="t('common.copy')"
              circle
              @click="onCopyItem(item)"
            />
          </div>
          <div>
            <IconButton
              v-if="list.length > 1"
              class="text-orange"
              icon="icon-delete"
              :tip="t('common.delete')"
              circle
              @click="onDeleteItem(item)"
            />
          </div>
        </div>
      </div>
    </div>
    <div ref="tinymceAuxNewContainer" />
    <template #footer>
      <DialogFooterBar @confirm="save" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
