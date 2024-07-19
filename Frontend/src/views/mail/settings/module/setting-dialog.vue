<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  getMailModuleSetting,
  updateMailModuleSetting,
} from "@/api/mail/mail-module";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { getMailModuleText } from "@/api/mail/mail-module";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; id: string }>();
const { t } = useI18n();
const show = ref(true);
const config = ref();
const model = ref<any>({});

(async () => {
  var rsp = await getMailModuleText("root", props.id, "module.config");
  config.value = JSON.parse(rsp);
  const oldSetting = await getMailModuleSetting(props.id);
  if (oldSetting) {
    model.value = JSON.parse(oldSetting);
  } else if (config.value.settingDefines) {
    for (const item of config.value.settingDefines) {
      model.value[item.name] = item.defaultValue;
    }
  }
})();

const onSave = async () => {
  config.value.setting = model.value;
  await updateMailModuleSetting(props.id, JSON.stringify(model.value, null, 2));
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.setting')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form label-position="top">
      <div v-if="config?.settingDefines">
        <el-form-item v-for="item of config.settingDefines" :key="item.name">
          <template #label>
            <p class="text-m font-bold ellipsis">
              {{ item.display || item.name
              }}<span
                class="font-normal text-s px-4"
                :title="item.description"
                >{{ item.description }}</span
              >
            </p>
          </template>
          <el-switch v-if="item.type === 'switch'" v-model="model[item.name]" />
          <el-input-number
            v-else-if="item.type === 'number'"
            v-model.number="model[item.name]"
          />
          <el-select
            v-else-if="item.type === 'select'"
            v-model.number="model[item.name]"
            class="w-full"
          >
            <el-option
              v-for="(i, index) of item.options"
              :key="index"
              :label="i"
              :value="i"
            />
          </el-select>
          <el-input
            v-else
            v-model.number="model[item.name]"
            :type="item.type == 'textarea' ? 'textarea' : ''"
          />
        </el-form-item>
      </div>
    </el-form>

    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
