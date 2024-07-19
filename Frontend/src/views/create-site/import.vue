<script lang="ts" setup>
import { getAvailableDomain } from "@/api/console";
import {
  importSite,
  importChunkPackage,
  isUniqueName,
  setSitesFolder,
  importSiteByUrl,
} from "@/api/site";
import type { AvailableDomain } from "@/api/console/types";
import { ref, watch } from "vue";
import {
  domainBindingIsUniqueNameRule,
  DomainRule,
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { ElLoading, ElMessage } from "element-plus";
import { getQueryString } from "@/utils/url";
import type { KeyValue } from "@/global/types";
import { newGuid } from "@/utils/guid";
const { t } = useI18n();
const domains = ref<AvailableDomain[]>();
const router = useRouter();
const progress = ref(0);

const model = ref({
  rootDomain: "",
  subDomain: "",
  siteName: "",
  url: "",
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  file: null as any,
});
const isSubDomainEdit = ref(false);
const isSiteNameEdit = ref(false);
const siteId = ref();

const rules = {
  subDomain: [
    requiredRule(t("common.subDomainRequiredTips")),
    rangeRule(1, 63),
    DomainRule,
    domainBindingIsUniqueNameRule(model.value),
  ],
  rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
  file: requiredRule(t("common.selectSitePackageTips")),
  siteName: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    simpleNameRule(),
    isUniqueNameRule(isUniqueName, t("common.siteNameExistsTips")),
  ],
  url: requiredRule(t("common.urlRequiredTips")),
} as Rules;

const form = ref();

getAvailableDomain().then((rsp) => {
  domains.value = rsp;

  if (domains.value.length) {
    model.value.rootDomain = domains.value[0].domainName;
  }
});

const types: KeyValue[] = [
  {
    key: "file",
    value: t("common.file"),
  },
  {
    key: "URL",
    value: "URL",
  },
];
const currentType = ref(types[0].key);

const startImport = async () => {
  await form.value.validate();

  const loading = ElLoading.service({
    lock: true,
    text: t("common.siteParsing"),
    background: "rgba(0, 0, 0, 0.7)",
  });

  try {
    if (currentType.value === "file") {
      var data = new FormData();
      data.append("siteName", model.value.siteName);
      data.append("subDomain", model.value.subDomain);
      data.append("rootDomain", model.value.rootDomain);
      const packageName = await multiChunkUploadPackage();
      data.append("packageName", packageName);
      siteId.value = await importSite(data, (p) => {
        //
      }); // eslint-disable-next-line no-undef
    } else {
      siteId.value = await importSiteByUrl({
        rootDomain: model.value.rootDomain,
        subDomain: model.value.subDomain,
        siteName: model.value.siteName,
        url: model.value.url,
      });
    }

    const folder = getQueryString("currentFolder");
    if (folder) {
      await setSitesFolder({
        siteIds: [siteId.value],
        folderName: folder,
      });
    }
    loading.close();
    router.push({
      name: "home",
      query: {
        currentFolder: folder,
      },
    });
  } catch (error) {
    loading.close();
  }
};

var chunkSize = 1024 * 1024;
async function multiChunkUploadPackage() {
  const id = newGuid();
  const file = model.value.file;
  let offset = 0;
  let index = 0;
  while (offset < file.size) {
    index++;
    const endOffset = offset + chunkSize;
    const blob = file.raw.slice(offset, endOffset);
    const data = new FormData();
    data.append("id", id);
    data.append("index", index.toString());
    data.append("file", blob);
    await importChunkPackage(data, (p) => {
      progress.value = parseFloat(
        (((p.loaded + offset) / file.size) * 100).toFixed(2)
      );
    });
    offset = endOffset;
  }

  const fileName = `${id}.zip`;
  const finishData = new FormData();
  finishData.append("id", id);
  finishData.append("name", fileName);

  await importChunkPackage(finishData, () => {
    //
  });

  return fileName;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const onFileSelect = (value: any) => {
  var name = value.name;
  if (name.indexOf(".") > -1 && name.split(".").reverse()[0] === "zip") {
    model.value.file = value;
    if (!model.value.subDomain) {
      var arr = name.split(".");
      arr.pop();
      model.value.subDomain = arr.join(".");
    }
  } else {
    ElMessage.error(t("common.fileFormatIsIncorrect"));
  }
};
const removeFile = () => {
  model.value.file = null;
  model.value.subDomain = "";
};
watch(
  () => model.value.file,
  () => {
    if (model.value.file) {
      form.value.clearValidate("file");
    }
  }
);

watch(
  () => model.value.subDomain,
  () => {
    if (isSubDomainEdit.value) return;
    model.value.siteName = model.value.subDomain;
  }
);

watch(
  () => model.value.siteName,
  () => {
    if (isSiteNameEdit.value) return;
    model.value.subDomain = model.value.siteName;
  }
);
</script>

<template>
  <div class="text-444 dark:text-fff/86">
    <p class="text-3l mb-8">{{ t("common.importSite") }}</p>
    <p class="text-s">
      {{ t("common.importSiteTips") }}
    </p>
    <div
      class="border-t border-b border-line dark:border-[#555] border-solid my-24 py-24"
    >
      <el-form ref="form" label-position="top" :model="model" :rules="rules">
        <el-form-item :label="t('common.siteName')" prop="siteName">
          <el-input
            v-model="model.siteName"
            class="w-394px"
            data-cy="siteName"
            @input="isSubDomainEdit = true"
          />
        </el-form-item>
        <div class="flex items-center space-x-8">
          <el-form-item :label="t('common.domain')" prop="subDomain">
            <el-input
              v-model="model.subDomain"
              class="w-394px"
              data-cy="subdomain"
              @input="isSiteNameEdit = true"
            />
          </el-form-item>
          <el-form-item prop="rootDomain" class="mt-30px">
            <el-select v-model="model.rootDomain" data-cy="root-domain">
              <el-option
                v-for="item of domains"
                :key="item.domainName"
                :value="item.domainName"
                :label="'.' + item.domainName"
                data-cy="root-domain-opt"
              />
            </el-select>
          </el-form-item>
        </div>

        <el-radio-group
          v-model="currentType"
          class="el-radio-group--rounded mb-12"
        >
          <el-radio-button
            v-for="item of types"
            :key="item.key"
            :label="item.key"
            :data-cy="item.key"
            >{{ item.value }}</el-radio-button
          >
        </el-radio-group>

        <el-form-item
          v-if="currentType === 'file'"
          :label="t('common.file')"
          prop="file"
        >
          <el-upload
            :show-file-list="false"
            :action="''"
            drag
            accept=".zip"
            :auto-upload="false"
            :on-change="onFileSelect"
            class="w-full"
          >
            <div class="flex items-center justify-center h-full flex-col">
              <p class="text-m flex items-center relative">
                <el-icon
                  class="iconfont icon-a-Cloudupload mr-8 text-2l leading-none absolute left-[-24px]"
                />
                <span>{{ t("common.selectOrDrag") }}</span>
              </p>
              <span
                v-if="!model.file"
                class="h-16 text-666 font-normal text-[12px] leading-relaxed"
                >{{ t("common.compressedPackageFileFormat") }}</span
              >

              <p v-if="model.file" class="text-s mt-8 text-666 pr-8 pl-8">
                <span>{{ model.file.name }}</span>
                <el-icon
                  class="iconfont icon-delete text-orange ml-8"
                  @click.stop="removeFile"
                />
              </p>
            </div>
          </el-upload>
        </el-form-item>
        <el-form-item v-else prop="url" label="URL">
          <el-input v-model="model.url" data-cy="url" />
        </el-form-item>
        <el-progress
          v-if="progress > 0"
          :text-inside="true"
          :stroke-width="26"
          :percentage="progress > 100 ? 100 : progress"
        />
      </el-form>
    </div>
    <el-button
      round
      type="primary"
      data-cy="start-import"
      @click="startImport"
      >{{ t("common.startImport") }}</el-button
    >
  </div>
</template>
<style>
.el-upload-dragger {
  border: 1px dashed #2296f3;
  width: 660px;
  height: 140px;
}
</style>
