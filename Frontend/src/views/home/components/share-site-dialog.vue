<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    :title="t('common.shareSite')"
    @close="handleClose"
  >
    <el-alert
      :closable="false"
      type="info"
      class="mb-24 dark:bg-666 text-666 dark:text-gray"
      :title="t('common.ShareSitesTips')"
    />

    <el-scrollbar class="w-full pr-12" max-height="350px">
      <el-form
        ref="form"
        class="el-form--label-normal dark:text-fff/86"
        :model="model"
        :rules="rules"
        label-position="top"
      >
        <el-form-item label="Share Method">
          <el-radio-group
            v-model="model.shareMethod"
            class="el-radio-group--rounded"
          >
            <el-radio-button label="add" size="large">{{
              t("common.add")
            }}</el-radio-button>
            <el-radio-button label="update" size="large">{{
              t("common.update")
            }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="model.shareMethod === 'update'" prop="templateId">
          <el-select v-model="model.templateId" class="w-full">
            <el-option
              v-for="item in siteList"
              :key="item.templateId"
              :value="item.templateId"
              :label="item.name"
            />
          </el-select>

          <el-radio-group v-model="model.updateItem" class="ml-4 h-40px mt-4">
            <el-radio label="onlyScreen" size="large">{{
              t("common.onlyScreen")
            }}</el-radio>
            <el-radio label="onlyBinary" size="large">{{
              t("common.onlyBinary")
            }}</el-radio>
          </el-radio-group>
        </el-form-item>
        <div
          :class="
            model.updateItem === 'onlyBinary' && model.shareMethod === 'update'
              ? 'hidden'
              : ''
          "
        >
          <el-form-item
            :label="t('common.category')"
            data-cy="category"
            :class="
              model.updateItem === 'onlyScreen' &&
              model.shareMethod === 'update'
                ? '!hidden'
                : ''
            "
          >
            <el-select v-model="model.typeName" class="w-full">
              <el-option
                v-for="(item, index) of typeList"
                :key="index"
                :label="item.value"
                :value="item.key"
                :data-cy="`category-${item.name}`"
              />
            </el-select>
          </el-form-item>

          <div class="flex space-x-64">
            <el-form-item prop="cover">
              <div>
                <div>
                  {{ t("common.cover") }}
                </div>
                <div class="relative group">
                  <el-image
                    class="!w-150px !h-full rounded-normal shadow-s-10 border-666 dark:border-1 border-solid"
                    :src="model.coverImage"
                    :preview-src-list="[model.coverImage]"
                    :initial-index="0"
                    fit="cover"
                  />
                  <div
                    class="hidden group-hover:block w-full h-28px !cursor-pointer absolute leading-7 text-l text-center bg-000 dark:bg-666 bg-opacity-20 text-fff bottom-10px rounded-b-normal"
                  >
                    <input
                      class="absolute inset-0 !opacity-0"
                      type="file"
                      title=" "
                      style="cursor: pointer"
                      :accept="imageAccepts.join(',')"
                      @change="getFile('model.coverImage', $event)"
                    />
                    {{ t("common.changeCover") }}
                  </div>
                </div>
              </div>
            </el-form-item>
            <el-form-item v-if="shareValidator?.showScreenShot">
              <div>
                {{ t("common.screenshot") }}
                <span class="text-s text-999">(1072px * 500px)</span>
              </div>
              <div class="relative group">
                <el-image
                  class="!w-150px !h-full rounded-normal shadow-s-10 border-666 dark:border-1 border-solid"
                  :src="model.screenshot"
                  :preview-src-list="[model.screenshot]"
                  :initial-index="0"
                  fit="cover"
                />

                <div
                  class="hidden group-hover:block w-full h-28px !cursor-pointer absolute leading-7 text-l text-center bg-000 dark:bg-666 bg-opacity-20 text-fff bottom-10px rounded-b-normal"
                >
                  <input
                    class="absolute inset-0 !opacity-0"
                    type="file"
                    title=" "
                    style="cursor: pointer"
                    :accept="imageAccepts.join(',')"
                    @change="getFile('model.screenshot', $event)"
                  />
                  {{ t("common.changeScreenshot") }}
                </div>
              </div>
            </el-form-item>
          </div>

          <el-form-item prop="siteName" :label="t('common.templateName')">
            <el-input v-model="model.siteName" data-cy="template-name" />
          </el-form-item>
          <ElButton
            v-if="!showChinese"
            size="small"
            @click="showChinese = true"
            >{{ t("common.addChinese") }}</ElButton
          >
          <template v-else>
            <div class="flex space-x-64">
              <el-form-item prop="cover">
                <div>
                  <div>
                    {{ t("common.zhCover") }}
                  </div>
                  <div class="relative group">
                    <el-image
                      class="!w-150px !h-full rounded-normal shadow-s-10 border-666 dark:border-1 border-solid"
                      :src="model.zhCoverImage"
                      :preview-src-list="[model.zhCoverImage]"
                      :initial-index="0"
                      fit="cover"
                    />
                    <div
                      class="hidden group-hover:block w-full h-28px !cursor-pointer absolute leading-7 text-l text-center bg-000 dark:bg-666 bg-opacity-20 text-fff bottom-10px rounded-b-normal"
                    >
                      <input
                        class="absolute inset-0 !opacity-0"
                        type="file"
                        title=" "
                        style="cursor: pointer"
                        :accept="imageAccepts.join(',')"
                        @change="getFile('model.zhCoverImage', $event)"
                      />
                      {{ t("common.changeCover") }}
                    </div>
                  </div>
                </div>
              </el-form-item>

              <el-form-item v-if="shareValidator?.showScreenShot">
                <div>
                  {{ t("common.zhScreenshot") }}
                  <span class="text-s text-999">(1072px * 500px)</span>
                </div>
                <div class="relative group">
                  <el-image
                    class="!w-150px !h-full rounded-normal shadow-s-10 border-666 dark:border-1 border-solid"
                    :src="model.zhScreenshot"
                    :preview-src-list="[model.zhScreenshot]"
                    :initial-index="0"
                    fit="cover"
                  />
                  <div
                    class="hidden group-hover:block w-full h-28px !cursor-pointer absolute leading-7 text-l text-center bg-000 dark:bg-666 bg-opacity-20 text-fff bottom-10px rounded-b-normal"
                  >
                    <input
                      class="absolute inset-0 !opacity-0"
                      type="file"
                      title=" "
                      style="cursor: pointer"
                      :accept="imageAccepts.join(',')"
                      @change="getFile('model.zhScreenshot', $event)"
                    />
                    {{ t("common.changeScreenshot") }}
                  </div>
                </div>
              </el-form-item>
            </div>

            <el-form-item prop="zhSiteName" :label="t('common.zhTemplateName')">
              <el-input v-model="model.zhSiteName" data-cy="template-name" />
              <!-- <template #error="{ error }">
          <el-tooltip :model-value="true" :content="error" placement="top">
            <div></div>
          </el-tooltip>
        </template> -->
            </el-form-item>
          </template>
        </div>
      </el-form>
    </el-scrollbar>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.share')"
        @confirm="handleShare"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { reactive, ref, watch } from "vue";
import type { ElForm } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type { SiteItem } from "../type";
import { getType, share, shareValidate, getPersonalList } from "@/api/template";
import { rangeRule, requiredRule } from "@/utils/validate";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
import { useFileUpload } from "@/hooks/use-file-upload";
import type { ShareValidator } from "@/api/template/types";
import { showConfirm } from "@/components/basic/confirm";
interface PropsType {
  modelValue: boolean;
  site: SiteItem;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
}
const emptyScreenshot =
  "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/4+goAAAAVklEQVR42mJ89wPTAwMDAwMDAwMABXAAcLCAFEAAAAASUVORK5CYII=";

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const { getAccepts } = useFileUpload();
const imageAccepts = getAccepts("image");
const typeList = ref();
const shareValidator = ref<ShareValidator>();
const showChinese = ref(false);

const form = ref<InstanceType<typeof ElForm>>();
const model = reactive({
  siteName: "",
  coverImage: "",
  screenshot: "",
  zhSiteName: "",
  zhCoverImage: "",
  zhScreenshot: "",
  typeName: "",
  shareMethod: "add",
  templateId: "",
  updateItem: "",
});

const siteList = ref([] as any);
const rules = {
  siteName: [
    requiredRule(t("common.pleaseEnterTemplateName")),
    rangeRule(1, 50),
  ],
  zhSiteName: [
    requiredRule(t("common.pleaseEnterTemplateName")),
    rangeRule(1, 50),
  ],

  templateId: requiredRule(t("common.pleaseEnterTemplateName")),
};

function getFile(image: string, event: any) {
  var file = event.target.files[0];
  //上传的图片文件转为base64
  var reader = new FileReader();
  reader.readAsDataURL(file);
  reader.onload = function (e: any) {
    var b64 = e.target.result;
    if ("model.coverImage" === image) {
      model.coverImage = b64;
    } else if ("model.screenshot" == image) {
      model.screenshot = b64;
    } else if ("model.zhCoverImage" === image) {
      model.zhCoverImage = b64;
    } else if ("model.zhScreenshot" == image) {
      model.zhScreenshot = b64;
    }
  };
  event.target.value = "";
}

//初始化封面和截图。并把路径转为base64
function getBase64(imgUrl: any) {
  window.URL = window.URL || window.webkitURL;
  var xhr = new XMLHttpRequest();
  xhr.open("get", imgUrl, true);
  xhr.responseType = "blob";
  xhr.onload = function () {
    if (this.status == 200) {
      var blob = this.response;
      let fileReader = new FileReader();
      fileReader.onloadend = function (e: any) {
        model.coverImage = e.target.result;
        model.zhCoverImage = e.target.result;
        if (shareValidator.value?.showScreenShot) {
          model.screenshot = emptyScreenshot;
          model.zhScreenshot = emptyScreenshot;
        }
      };
      fileReader.readAsDataURL(blob);
    }
  };
  xhr.send();
}

watch(
  () => visible.value,
  async (val) => {
    if (val) {
      typeList.value = await getType();
      shareValidator.value = await shareValidate(props.site.siteId);
      model.siteName = props.site.siteDisplayName;
      model.zhSiteName = props.site.siteDisplayName;
      getBase64(` __cover?siteId=${props.site.siteId}`);
    }
  }
);

watch(
  () => model.shareMethod,
  async () => {
    if (model.shareMethod === "update" && !siteList.value.length) {
      siteList.value = await getPersonalList();
    }
  }
);

function handleShare() {
  form.value?.validate(async (valid) => {
    if (valid) {
      if (!shareValidator.value?.isSecure) {
        await showConfirm(t("common.shareSiteConnectionStringWarning"));
      }
      await share({
        siteId: props.site.siteId,
        typeName: model.typeName,
        siteName: model.siteName,
        coverImage: model.coverImage,
        screenshot:
          model.screenshot === emptyScreenshot ? "" : model.screenshot,
        zhSiteName: model.zhSiteName,
        zhCoverImage: model.zhCoverImage,
        zhScreenshot:
          model.zhScreenshot === emptyScreenshot ? "" : model.zhScreenshot,
        shareMethod: model.shareMethod,
        templateId: model.templateId,
        updateItem: model.updateItem,
      });
      // TODO: 成功后跳转到 模板站点页面
      handleClose();
    }
  });
}

watch(
  () => visible.value,
  () => {
    if (!visible.value) {
      model.typeName = "";
      model.screenshot = "";
      model.zhScreenshot = "";
      model.shareMethod = "add";
      model.siteName = "";
      model.updateItem = "";
      model.templateId = "";
      showChinese.value = false;
      siteList.value = [];
    }
  }
);
</script>
<style scoped>
:deep(img.el-image__inner.el-image__preview) {
  height: 100px;
}

input[type="file"]::-webkit-file-upload-button {
  /* chromes and blink button */
  cursor: pointer;
}
:deep(.el-radio__input.is-checked .el-radio__inner) {
  border-color: #409eff;
  background: #fff;
}
:deep(.el-radio__inner::after) {
  width: 7px;
  height: 7px;
  background-color: #409eff;
}
</style>
