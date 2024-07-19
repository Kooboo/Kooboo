<script lang="ts" setup>
import { site, importAccept, onUploadPackage } from "./settings";
import { ref } from "vue";
import { getTypes } from "@/api/site";
import { useI18n } from "vue-i18n";

const emit = defineEmits<{
  (e: "export"): void;
  (e: "initSite"): void;
}>();

const { t } = useI18n();
const siteTypes = ref<Record<string, string>>();
getTypes().then((rsp) => (siteTypes.value = rsp));
const siteTypesRecord = ref<Record<string, string>>({
  o: "private",
  p: "public",
  m: "member",
});

const uploadPackage = async (f: any) => {
  await onUploadPackage(f);
  emit("initSite");
};

const onAdd = () => {
  if (!Array.isArray(site.value!.specialPath)) {
    site.value!.specialPath = [];
  }

  site.value!.specialPath!.push("");
};

const onDelete = (key: number) => {
  site.value!.specialPath!.splice(key, 1);
};

// includePath
const modeList = ref([
  {
    value: true,
    name: "Contain",
    title: t("common.IncludePath"),
  },
  {
    value: false,
    name: "Exclude",
    title: t("common.ExcludePath"),
  },
]);
</script>

<template>
  <div
    v-if="site"
    class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px"
  >
    <div class="max-w-504px">
      <el-form-item :label="t('common.displayName')">
        <el-input v-model="site.displayName" data-cy="display-name" />
      </el-form-item>
      <el-form-item>
        <template #label>
          <div class="flex items-center">
            {{ t("common.defaultDatabase") }}
            <Tooltip
              :tip="t('common.defaultDataBaseTips')"
              custom-class="ml-4"
            />
          </div>
        </template>
        <el-select v-model="site.defaultDatabase" class="w-full">
          <el-option value="Auto" :label="t('common.auto')" data-cy="auto" />
          <el-option value="Sqlite" label="Sqlite" data-cy="sqlite" />
          <el-option value="Mysql" label="Mysql" data-cy="mysql" />
          <el-option value="SqlServer" label="SqlServer" data-cy="sqlserver" />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('common.siteType')">
        <el-select v-model="site.siteType" class="w-full" data-cy="site-type">
          <el-option
            v-for="(value, key) of siteTypes"
            :key="key"
            :value="key"
            :label="value"
            :data-cy="siteTypesRecord[key]"
          />
        </el-select>
      </el-form-item>
    </div>

    <div
      v-if="site.siteType !== 'p'"
      class="!-ml-56px !-mr-56px !px-56px bg-[#fafafa] dark:bg-[#333]"
    >
      <div class="max-w-504px py-16 mb-18px">
        <el-form-item :label="t('common.PathPattern')">
          <el-select
            v-model="site.includePath"
            class="w-full"
            data-cy="site-type"
          >
            <el-option
              v-for="(item, key) of modeList"
              :key="key"
              :value="item.value"
              :label="item.title"
              :data-cy="item.title"
            />
          </el-select>
        </el-form-item>

        <el-form-item :label="t('common.path')">
          <div class="space-y-8 w-full">
            <div
              v-for="(item, index) of site.specialPath"
              :key="index"
              class="flex items-center space-x-8 w-full"
            >
              <el-input
                v-model="site.specialPath![index]"
                :placeholder="t('common.value')"
              />
              <div>
                <IconButton
                  circle
                  class="hover:text-orange text-orange"
                  icon="icon-delete "
                  :tip="t('common.delete')"
                  @click="onDelete(index)"
                />
              </div>
            </div>
            <IconButton
              circle
              class="text-blue"
              icon="icon-a-addto "
              :tip="t('common.add')"
              @click="onAdd"
            />
          </div>
        </el-form-item>

        <el-form-item :label="t('common.singleSignOn')">
          <el-switch v-model="site.ssoLogin" />
        </el-form-item>
      </div>
    </div>

    <div class="max-w-504px">
      <el-form-item label="Base URL">
        <el-input v-model="site.previewUrl" data-cy="base-url" />
      </el-form-item>
      <el-form-item :label="t('common.import/exportSite')">
        <div class="flex items-center space-x-16">
          <el-upload
            :show-file-list="false"
            :action="''"
            :accept="importAccept.join(',')"
            :auto-upload="false"
            :on-change="uploadPackage"
            data-cy="import-package"
          >
            <el-button round>
              <el-icon class="iconfont icon-a-Pullin" />
              {{ t("common.importPackage") }}
            </el-button>
          </el-upload>

          <el-button
            v-hasPermission="{ feature: 'site', action: 'export' }"
            round
            data-cy="export-site"
            @click="$emit('export')"
          >
            <el-icon class="iconfont icon-share" />
            {{ t("common.exportSite") }}
          </el-button>
        </div>
      </el-form-item>
    </div>
  </div>
</template>
