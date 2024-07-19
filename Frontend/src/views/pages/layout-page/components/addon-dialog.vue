<script lang="ts" setup>
import { getTagObjects } from "@/api/component";
import type { Component, TagObject } from "@/api/component/types";
import { computed } from "@vue/reactivity";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { AddonSource } from "../source";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: TagObject[]): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  model: Component;
  sources: AddonSource[];
}>();

const { t } = useI18n();

const show = ref(true);
const list = ref<TagObject[]>([]);
const selected = ref<TagObject[]>([]);

const load = async () => {
  list.value = await getTagObjects(props.model.tagName);
};

const displayList = computed(() => {
  const result = [];

  for (const item of list.value) {
    const source = props.sources.find((f) => f.id === item.name);
    if (source?.tag !== "layout") {
      result.push(item);
    }
  }

  return result;
});

const onCheckChange = (checkedNode: TagObject, checked: any) => {
  if (checked) {
    selected.value.push(checkedNode);
  } else {
    selected.value = selected.value.filter((f) => f !== checkedNode);
  }
};

const onSave = async () => {
  show.value = false;
  emit("selected", selected.value);
};

load();
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="600px"
    :close-on-click-modal="false"
    :title="model.displayName"
    @closed="emit('update:modelValue', false)"
  >
    <div>
      <div class="p-32 grid grid-cols-3">
        <el-checkbox
          v-for="item in displayList"
          :key="item.id"
          size="large"
          @change="onCheckChange(item, $event)"
        >
          <p
            class="hover:text-blue w-140px ellipsis text-16px h-17px"
            :title="item.name"
            data-cy="name"
          >
            {{ item.name }}
          </p>
        </el-checkbox>
      </div>
    </div>
    <template #footer>
      <DialogFooterBar
        :disabled="!selected.length"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
