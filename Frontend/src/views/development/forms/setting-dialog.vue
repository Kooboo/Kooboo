<script lang="ts" setup>
import { computed, ref } from "vue";
import type { PostSetting, Setting } from "@/api/form/types";
import { updateSetting, getSetting } from "@/api/form";
import { usePageStore } from "@/store/page";
import type { KeyValue } from "@/global/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();
const { t } = useI18n();

const show = ref(true);
const pageStore = usePageStore();
const setting = ref<Setting>();
const model = ref<PostSetting>({
  id: "",
  formId: "",
  method: "get",
  redirectUrl: "RefreshSelf()",
  setting: {},
  formSubmitter: "",
  enable: false,
});

getSetting(props.id).then((r) => {
  setting.value = r;
  model.value.id = setting.value.id;
  model.value.formId = setting.value.formId;
  model.value.redirectUrl = setting.value.redirectUrl;
  model.value.formSubmitter = setting.value.formSubmitter;
  model.value.enable = setting.value.enable;
  model.value.method = setting.value.method || "get";
  model.value.setting = setting.value.setting;
});

const redirectList = computed(() => {
  const result: KeyValue[] = [];
  result.push({
    key: "RefreshSelf()",
    value: t("common.refreshSelf"),
  });

  for (const i of pageStore.list) {
    result.push({
      key: i.path,
      value: i.name,
    });
  }
  return result;
});

const onSave = async () => {
  await updateSetting(model.value);
  show.value = false;
};

const settings = computed(() => {
  if (!setting.value) return [];
  const found = setting.value.availableSubmitters.find(
    (f) => f.name === model.value.formSubmitter
  );
  return found?.settings || [];
});
pageStore.load();
</script>

<template>
  <div @click.stop>
    <el-dialog
      :model-value="show"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.formSetting')"
      @closed="emit('update:modelValue', false)"
    >
      <el-form
        v-if="setting"
        ref="form"
        label-position="top"
        :model="model"
        @keydown.enter="onSave"
      >
        <el-form-item :label="t('common.enable')">
          <el-switch v-model="model.enable" data-cy="enable" />
        </el-form-item>
        <template v-if="model.enable">
          <el-form-item :label="t('common.method')">
            <el-radio-group
              v-model="model.method"
              class="el-radio-group--rounded"
            >
              <el-radio-button
                v-for="item of ['get', 'post']"
                :key="item"
                :label="item"
                :data-cy="'method-' + `${item}`"
                >{{ item }}</el-radio-button
              >
            </el-radio-group>
          </el-form-item>

          <el-form-item :label="t('common.redirectTo')">
            <el-select
              v-model="model.redirectUrl"
              class="w-full"
              filterable
              allow-create
              default-first-option
              data-cy="redirect-dropdown"
            >
              <el-option
                v-for="item of redirectList"
                :key="item.key"
                :value="item.key"
                :label="item.value"
                data-cy="redirect-opt"
              />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('common.submitter')">
            <el-select
              v-model="model.formSubmitter"
              class="w-full"
              data-cy="submitter-dropdown"
              @change="model.setting = {}"
            >
              <el-option
                v-for="item of setting.availableSubmitters"
                :key="item.name"
                :value="item.name"
                :label="item.name"
                data-cy="submitter-opt"
              />
            </el-select>
          </el-form-item>
          <el-form-item v-if="model.formSubmitter == 'SubmitToExternal'">
            <el-input
              v-model="model.setting['url']"
              placeholder="http://www.domain.com/receive"
              data-cy="url"
            />
          </el-form-item>
          <el-form-item
            v-for="item of settings"
            v-else
            :key="item.name"
            :label="item.name"
          >
            <el-select
              v-model="
                model.setting[
                  item.name.replace(
                    item.name[0],
                    item.name[0].toLocaleLowerCase()
                  )
                ]
              "
              class="w-full"
              filterable
              allow-create
              default-first-option
              data-cy="submitter-object-dropdown"
            >
              <el-option
                v-for="(value, key) of item.selectionValues"
                :key="key"
                :value="key"
                :label="value"
                data-cy="submitter-object-opt"
              />
            </el-select>
          </el-form-item>
        </template>
      </el-form>
      <template #footer>
        <DialogFooterBar
          :permission="{
            feature: 'form',
            action: 'edit',
          }"
          @confirm="onSave"
          @cancel="show = false"
        />
      </template>
    </el-dialog>
  </div>
</template>
