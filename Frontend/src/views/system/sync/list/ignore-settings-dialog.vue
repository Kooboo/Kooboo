<script lang="ts" setup>
import { computed, ref } from "vue";
import type { KeyValue } from "@/global/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import {
  getSyncSetting,
  updateSyncSetting,
  getStoreNames,
} from "@/api/publish";
import type { SyncSetting } from "@/api/publish/types";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; id: string }>();
const { t } = useI18n();
const show = ref(true);
const storeNames = ref<KeyValue[]>([]);
const type = ref("out");
const syncSetting = ref<SyncSetting>();

getStoreNames().then((rsp) => (storeNames.value = rsp));
getSyncSetting(props.id).then((rsp) => (syncSetting.value = rsp));

const toList = ref<KeyValue[]>([
  {
    key: "out",
    value: t("common.push"),
  },
  {
    key: "in",
    value: t("common.pull"),
  },
]);

const onSave = async () => {
  await updateSyncSetting(props.id, syncSetting.value);
  emit("reload");
  show.value = false;
};

const currentStoreNames = computed(() => {
  if (!syncSetting.value) return [];
  return type.value == "in"
    ? syncSetting.value.ignoreInStoreNames
    : syncSetting.value.ignoreOutStoreNames;
});

function isSelectedStore(name: string) {
  return currentStoreNames.value.includes(name);
}

function onStoreClick(name: string) {
  var index = currentStoreNames.value.indexOf(name);
  if (index > -1) {
    currentStoreNames.value.splice(index, 1);
  } else {
    currentStoreNames.value.push(name);
  }
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.ignoreSettings')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form label-width="100px">
      <el-form-item :label="t('common.type')" class="flex--middle">
        <el-radio-group v-model="type" class="el-radio-group--rounded">
          <el-radio-button
            v-for="item of toList"
            :key="item.key"
            :label="item.key"
            @click="type = item.key"
            >{{ item.value }}</el-radio-button
          >
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="t('common.ignoreStore')">
        <div
          class="flex flex-wrap gap-8 bg-card dark:bg-444 p-12 rounded-normal"
        >
          <template v-for="item of storeNames" :key="item.name">
            <el-tag
              round
              class="cursor-pointer"
              :class="{ 'line-through': isSelectedStore(item.key) }"
              :type="isSelectedStore(item.key) ? 'danger' : 'info'"
              @click="onStoreClick(item.key)"
              ><span class="leading-4">{{ item.value }}</span></el-tag
            >
          </template>
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :disabled="!syncSetting"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
