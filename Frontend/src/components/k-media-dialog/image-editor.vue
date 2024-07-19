<template>
  <div class="edit-image p-24 h-min-750px">
    <div class="text-2l font-bold mb-24 dark:text-gray">
      {{ t("common.editImage") }}
    </div>
    <div class="flex mb-32">
      <div class="w-390px mr-21px">
        <div
          class="py-24 pt-28px px-24px bg-fff dark:bg-444 rounded-normal shadow-s-10"
        >
          <div
            class="min-h-80px max-h-261px h-[calc(100vh-600px)] overflow-hidden shadow-l-20 flex items-center justify-center rounded-6px"
          >
            <img
              ref="img"
              :src="base64 || randomUrl(siteUrl ?? url)"
              class="rounded-6px w-full h-full object-contain"
              data-cy="preview-image"
            />
          </div>
          <div class="mt-24 space-y-12 file-info">
            <el-form
              ref="form"
              class="el-form--label-normal"
              label-position="left"
              label-width="120px"
              :model="{ name, alt, url }"
            >
              <el-form-item
                :label="t('common.fileName')"
                class="pb-16"
                prop="name"
                :rules="nameRules"
              >
                <el-input
                  :model-value="name"
                  :placeholder="t('common.fileName')"
                  data-cy="fileName"
                  @input="$emit('update:name', $event)"
                >
                  <template v-if="ext" #append>{{ ext }}</template>
                </el-input>
              </el-form-item>
              <el-form-item
                :label="t('common.altText')"
                prop="alt"
                class="pb-16"
              >
                <el-input
                  :model-value="alt"
                  :placeholder="t('common.altText')"
                  data-cy="alt"
                  @input="$emit('update:alt', $event)"
                />
              </el-form-item>
              <el-form-item class="mb-16" :label="t('common.url')">
                <a
                  class="text-[#999] break-all hover:underline"
                  :href="url"
                  target="_blank"
                  :title="t('common.preview')"
                  data-cy="url"
                >
                  {{ url }}
                </a>
              </el-form-item>
            </el-form>
          </div>
        </div>
        <div class="p-24 bg-fff dark:bg-444 rounded-normal shadow-s-10 mt-16">
          <div class="flex justify-between">
            <div
              v-for="item in ratios"
              :key="item.text"
              class="k-button dark:text-gray"
              :class="{ 'k-button--checked': currentRatio === item.ratio }"
              :data-cy="'radio-' + `${item.text}`"
              @click="handleChangeRatio(item)"
            >
              {{ item.text }}
            </div>
          </div>
          <div
            class="edit-image__tool2 mt-16 flex justify-between items-center"
          >
            <div class="flex items-center">
              <el-input
                v-model.number="size.width"
                :placeholder="t('common.width')"
                class="mr-4"
                data-cy="width"
                @change="handleChangeWidth(size.width)"
              >
                <template #append><span class="px-12">X</span></template>
              </el-input>
              <div
                class="iconfont icon-lock text-m mx-6px cursor-pointer text-444"
                :class="currentRatio ? 'icon-lock' : 'icon-unlock'"
                @click="handleSetRatio"
              />

              <el-input
                v-model.number="size.height"
                :placeholder="t('common.height')"
                class="ml-4"
                data-cy="height"
                @change="handleChangeHeight"
              >
                <template #append><span class="px-12">Y</span></template>
              </el-input>
            </div>
            <div>
              <el-input
                v-model.number="angle"
                :placeholder="t('common.angle')"
                @change="handleChangeAngle"
              >
                <template #append>
                  <div class="px-12 relative">
                    <span class="absolute bottom-[-12px] left-4">。</span>
                  </div>
                </template>
              </el-input>
            </div>
          </div>
          <div class="space-x-24 text-right mt-18px">
            <el-button
              type="primary"
              round
              class="min-w-134px"
              data-cy="crop"
              @click="handleCrop"
              >{{ t("common.crop") }}</el-button
            >
          </div>
        </div>
      </div>
      <div
        class="rounded-normal px-24 w-720px flex items-center justify-center h-auto shadow-s-10"
      >
        <div class="flex justify-center min-h-300px min-w-300px max-h-562px">
          <Cropper
            ref="cropper"
            class="cropper"
            :src="base64 || (siteUrl ?? url)"
            :stencil-props="{
              aspectRatio: currentRatio,
            }"
            @change="handleChangeCrop"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { updateQueryString } from "@/utils/url";
import { requiredRule, simpleNameRule } from "@/utils/validate";
import type { FormItemRule, FormValidateCallback } from "element-plus";
import { reactive, ref } from "vue";
import type { Cropper as CropperType, Transform } from "vue-advanced-cropper";
import { Cropper } from "vue-advanced-cropper";
import "vue-advanced-cropper/dist/style.css";
import { useI18n } from "vue-i18n";
import type { ElForm } from "element-plus";

const { t } = useI18n();
const form = ref<InstanceType<typeof ElForm>>();
const cropper = ref<InstanceType<typeof CropperType>>();
const angle = ref(0);
const ratios = [
  {
    text: t("common.free"),
    ratio: 0,
  },
  {
    text: "16:9",
    ratio: 16 / 9,
  },
  {
    text: "4:3",
    ratio: 4 / 5,
  },
  {
    text: "1:1",
    ratio: 1 / 1,
  },
  {
    text: "2:3",
    ratio: 2 / 3,
  },
] as const;

const currentRatio = ref<number>(0);
const size = reactive({
  width: 0,
  height: 0,
});
interface Props {
  name: string;
  url: string;
  siteUrl?: string;
  alt?: string;
  base64: string;
  ext: string;
}
defineProps<Props>();
interface EmitType {
  (e: "update:alt", params: string): void;
  (e: "update:name", params: string): void;
  (e: "update:base64", params: string): void;
}
const img = ref();
const emit = defineEmits<EmitType>();

const nameRules: FormItemRule[] = [
  requiredRule(t("common.nameRequiredTips")),
  simpleNameRule(),
];

const randomUrl = (url: string) => {
  return updateQueryString(url, { t: Date.now() });
};

function handleCrop() {
  if (!cropper.value) {
    return;
  }
  const { canvas } = cropper.value.getResult();
  if (canvas) {
    emit("update:base64", canvas.toDataURL());
  }
}

function setSize() {
  setTimeout(() => {
    if (cropper.value) {
      const { coordinates } = cropper.value.getResult();
      size.width = coordinates.width;
      size.height = coordinates.height;
    }
  }, 100);
}

function handleChangeRatio(item: typeof ratios[number]) {
  cropper.value?.reset();
  currentRatio.value = item.ratio;
  setSize();
}

function handleChangeWidth(width: number) {
  const transform: Transform = {
    width,
  };

  if (currentRatio.value > 0) {
    transform.height = size.width * currentRatio.value;
  }
  cropper.value?.setCoordinates(transform);
}
function handleChangeHeight() {
  if (currentRatio.value > 0) {
    const width = size.height * currentRatio.value;
    handleChangeWidth(width);
  } else {
    cropper.value?.setCoordinates({
      height: size.height,
    });
  }
}

function handleChangeCrop() {
  setSize();
}
let oldAngle = angle.value;
function handleChangeAngle() {
  if (isNaN(angle.value)) {
    angle.value = oldAngle;
    return;
  }
  cropper.value?.rotate(angle.value - oldAngle);
  oldAngle = angle.value;
}
function handleSetRatio() {
  if (currentRatio.value) {
    currentRatio.value = 0;
  } else if (size.height > 0 && size.width > 0) {
    currentRatio.value = size.width / size.height;
  }
}

defineExpose({
  validate(cb?: FormValidateCallback) {
    form.value?.validate(cb);
  },
  reset: form.value?.resetFields,
});
</script>

<style lang="scss" scoped>
:deep(.el-form--label-left) {
  .el-form-item__label {
    @apply font-bold justify-start items-center;
  }
}

:deep(.label-center) {
  .el-form-item__label {
    @apply items-center;
  }
}

:deep(.el-form-item) {
  margin-bottom: 12px;

  &:last-child {
    margin-bottom: 0;
  }
}

:deep(.el-form-item__content) {
  line-height: initial;
}

.edit-image {
  &__tool2 {
    .el-input {
      width: 96px;
    }
  }
}

:deep(.edit-image__tool2) {
  .el-input__inner {
    height: 36px;
  }
}

.el-button {
  min-width: 120px;
  height: 34px;
  min-height: auto;
}

:deep(.file-info) {
  .el-input {
    .el-input-group__append {
      background-color: var(--el-fill-color-light);
      padding: 0 20px;
    }
  }
}
</style>
