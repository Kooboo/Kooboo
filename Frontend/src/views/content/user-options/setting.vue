<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Schema } from "./user-options";
import ObjectSchema from "./object-schema.vue";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { saveUserOptions, getUserOptions } from "@/api/content/user-options";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const schema = ref<Schema[]>([]);
const router = useRouter();
const name = ref("");
const display = ref("");
const id = getQueryString("id")!;

getUserOptions(id).then((rsp) => {
  name.value = rsp.name;
  display.value = rsp.display;
  schema.value = JSON.parse(rsp.schema);
});

function goBack() {
  router.goBackOrTo(useRouteSiteId({ name: "useroptions" }));
}

async function save() {
  await saveUserOptions({
    name: name.value,
    display: display.value,
    data: JSON.stringify({}),
    schema: JSON.stringify(schema.value),
  });
  goBack();
}
</script>

<template>
  <div class="p-24 mb-64">
    <el-form label-position="top">
      <div class="rounded-normal bg-fff dark:bg-[#252526] mt-16 py-24 px-56px">
        <div class="w-504px">
          <el-form-item :label="t('common.name')">
            <el-input v-model="name" disabled />
          </el-form-item>
          <el-form-item :label="t('common.displayName')">
            <el-input v-model="display" />
          </el-form-item>
        </div>
        <el-form-item>
          <template #label>
            <div class="flex items-center">
              {{ t("common.optionSchema") }}
              <Tooltip
                :tip="t('common.settingSchemaTips', { name: name })"
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
