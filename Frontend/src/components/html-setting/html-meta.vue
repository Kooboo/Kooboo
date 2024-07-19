<script lang="ts" setup>
import type { Meta } from "@/api/pages/types";
import { ref } from "vue";
import HtmlMetaDialog from "./html-meta-dialog.vue";

defineProps<{
  list: Meta[];
  metaBindings?: string[];
}>();

const emit = defineEmits<{
  (e: "delete", value: Meta): void;
  (e: "addOrUpdate", value: Meta): void;
}>();
const showDialog = ref(false);
const editMeta = ref();
</script>

<template>
  <div class="px-24 py-16 space-y-4">
    <div
      v-for="item of list"
      :key="item.name! + item.httpequiv + item.charset+item.property"
      class="text-666 dark:text-fff/67 flex items-center px-16 w-full border-line rounded-normal border border-solid dark:border-666 ellipsis bg-fff dark:bg-[#333]"
    >
      <div
        class="overflow-hidden leading-40px flex-1 mr-8 ellipsis"
        :title="item.el!.outerHTML"
        data-cy="meta-preview"
      >
        {{ item.el!.outerHTML }}
      </div>
      <el-icon
        class="text-blue iconfont icon-a-writein cursor-pointer mr-8"
        data-cy="edit"
        @click="
          editMeta = item;
          showDialog = true;
        "
      />
      <el-icon
        class="text-orange iconfont icon-delete cursor-pointer"
        data-cy="remove"
        @click="emit('delete', item)"
      />
    </div>
    <el-button
      circle
      @click="
        editMeta = undefined;
        showDialog = true;
      "
    >
      <el-icon class="text-blue iconfont icon-a-addto" data-cy="add" />
    </el-button>
    <HtmlMetaDialog
      v-if="showDialog"
      v-model="showDialog"
      :meta="editMeta"
      :meta-bindings="metaBindings"
      @add-or-update="$emit('addOrUpdate', $event)"
    />
  </div>
</template>
