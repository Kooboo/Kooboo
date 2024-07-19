<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Schema } from "./user-options";
import ObjectSchema from "./object-schema.vue";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { saveUserOptions, isUniqueName } from "@/api/content/user-options";
import {
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  letterAndDigitStartRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";

const { t } = useI18n();
const schema = ref<Schema[]>([]);
const router = useRouter();
const name = ref("");
const display = ref("");
const form = ref();

const model = computed(() => {
  return {
    name: name.value,
  };
});

function goBack() {
  router.goBackOrTo(useRouteSiteId({ name: "useroptions" }));
}

const rules = {
  name: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    letterAndDigitStartRule(),
    isUniqueNameRule(isUniqueName, t("common.nameExistsTips")),
  ],
} as Rules;

async function save() {
  await form.value?.validate();
  await saveUserOptions({
    name: name.value,
    display: display.value,
    schema: JSON.stringify(schema.value),
  });
  goBack();
}
</script>

<template>
  <div class="p-24 mb-64">
    <el-form ref="form" :model="model" label-position="top" :rules="rules">
      <div class="rounded-normal bg-fff dark:bg-[#252526] mt-16 py-24 px-56px">
        <div class="w-504px">
          <el-form-item :label="t('common.name')" prop="name">
            <el-input v-model="name" />
          </el-form-item>
          <el-form-item :label="t('common.displayName')">
            <el-input v-model="display" />
          </el-form-item>
        </div>

        <el-form-item>
          <template #label>
            <div class="flex items-center">
              {{ t("common.customSettings") }}
              <Tooltip
                :tip="t('common.settingSchemaTips', { name: name || '{Name}' })"
                custom-class="ml-4"
              />
            </div>
          </template>
          <ObjectSchema :schema="schema" />
        </el-form-item>
      </div>
    </el-form>

    <KBottomBar
      :permission="{
        feature: 'userOptions',
        action: 'setting',
      }"
      @cancel="goBack"
      @save="save"
    />
  </div>
</template>
