<script lang="ts" setup>
import { ref } from "vue";
import type { CoreSetting } from "@/api/core-setting/types";
import { update } from "@/api/core-setting";
import type { Field } from "@/api/core-setting/types";
import { ElMessage } from "element-plus";
import { getFields } from "@/api/core-setting";
import { toObject } from "@/utils/lang";
import type { KeyValue } from "@/global/types";
import Alert from "@/components/basic/alert.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; config: CoreSetting }>();
const { t } = useI18n();
const show = ref(true);
const fields = ref<Field[]>([]);

getFields(props.config.name).then((rsp) => (fields.value = rsp));

function getFileName(nameAndBase64: string) {
  if (!nameAndBase64) {
    return nameAndBase64;
  }
  var idx = nameAndBase64.indexOf("|");
  idx = idx < 0 ? 0 : idx;
  return nameAndBase64.substr(0, idx);
}

function getFile(event: any, field: Field) {
  var file = event.target.files[0];
  if (!file) {
    return;
  }

  if (file.size > 10 * 1024) {
    ElMessage.error(t("common.fileSizeLessTips"));
    event.target.value = "";
    return;
  }

  var reader = new FileReader();
  reader.readAsDataURL(file);
  reader.onload = function (e: any) {
    var b64 = e.target.result;
    field.value = file.name + "|" + b64.substr(b64.indexOf("base64,") + 7);
  };
}

const onSave = async () => {
  if (!fields.value.length) return;
  show.value = false;
  const kvList = fields.value.map<KeyValue>((m) => ({
    key: m.name,
    value: m.value,
  }));
  const model = toObject(kvList);
  await update({
    name: props.config.name,
    model: model,
  });
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.edit')"
    @closed="emit('update:modelValue', false)"
  >
    <Alert v-if="config.alert" :content="config.alert" />
    <div class="p-32">
      <el-form label-position="top" @submit.prevent @keydown.enter="onSave">
        <el-form-item
          v-for="item of fields"
          :key="item.name"
          :label="item.name"
        >
          <el-switch v-if="item.type === 'checkbox'" v-model="item.value" />
          <template v-else-if="item.type === 'file'">
            <div class="flex items-center space-x-12">
              <el-button round class="relative">
                {{ t("common.selectFile") }}
                <input
                  class="cursor-pointer absolute inset-0 opacity-0"
                  type="file"
                  @change="getFile($event, item)"
                />
              </el-button>
              <template v-if="item.value">
                <div class="ellipsis max-w-200px">
                  <span>{{ getFileName(item.value) }}</span>
                </div>

                <el-icon
                  class="iconfont icon-delete text-orange cursor-pointer"
                  @click="item.value = ''"
                />
              </template>
            </div>
          </template>

          <el-input v-else v-model="item.value" />
        </el-form-item>
      </el-form>
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
