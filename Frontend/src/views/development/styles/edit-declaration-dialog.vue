<script lang="ts" setup>
import { ref, nextTick } from "vue";
import { getEdit, updateInline } from "@/api/css-rule";
import type { InlineStyle, Declaration } from "@/api/css-rule/types";
import { emptyGuid, newGuid } from "@/utils/guid";
import {
  cssProperties,
  cssColors,
  cssColorAndImage,
  cssImages,
} from "@/constants/development";
import KMediaDialog from "@/components/k-media-dialog";
import type { MediaFileItem } from "@/components/k-media-dialog";
import type { ElInput } from "element-plus";
import { getColorWordValue } from "@/utils/colorWord";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; id: string }>();
const { t } = useI18n();
const show = ref(true);
const selected = ref<Declaration>();
const showMediaDialog = ref(false);

const model = ref<InlineStyle>({
  id: props.id || emptyGuid,
  selector: "",
  declarations: [],
});

type Input = typeof ElInput;
const inputs = ref<Record<string, Input>>({});

getEdit(props.id).then((r) => {
  model.value = r;
  for (const i of model.value.declarations) {
    if (i.important) i.value = i.value + " !important";
  }
});

const isColor = (value: string) => {
  return cssColors.concat(cssColorAndImage).some((s) => s === value);
};

const isImage = (value: string) => {
  return cssImages.concat(cssColorAndImage).some((s) => s === value);
};

const getColor = (value: string) => {
  // 空格处理 去除左右空格 删除多余空格
  value = value?.trim().replace(/[ ]+/g, " ");
  if (!value) {
    return "";
  }
  // 切割属性, 查找颜色
  let attribute = [];
  attribute = value.replace(/([^,]) /g, "$1#$#").split("#$#");
  for (let i = 0; i < attribute.length; i++) {
    // hex rgb[a] hsl[a]
    if (/^(#|rgb|hsl)/i.test(attribute[i])) {
      return attribute[i];
    }
    // 颜色单词
    if (getColorWordValue(attribute[i])) {
      return getColorWordValue(attribute[i]);
    }
  }
  return "";
};

const setColor = (value: string, color: string) => {
  // 空格处理 去除左右空格 删除多余空格
  value = value?.trim().replace(/[ ]+/g, " ");
  if (!value) {
    return color;
  }
  // 切割属性, 查找颜色并替换
  let attribute = [];
  attribute = value.replace(/([^,]) /g, "$1#$#").split("#$#");
  for (let i = 0; i < attribute.length; i++) {
    // hex// hex rgb[a] hsl[a]
    if (/^(#|rgb|hsl)/i.test(attribute[i])) {
      attribute[i] = color;
      return attribute.join(" ");
    }
    // 颜色单词
    if (getColorWordValue(attribute[i])) {
      attribute[i] = color;
      return attribute.join(" ");
    }
  }

  // 没匹配到颜色 向前添加颜色
  return `${color} ${value}`;
};

const onSave = async () => {
  const ruleText = model.value.declarations
    .map((m) => `${m.propertyName}:${m.value};`)
    .join("");
  await updateInline({ id: model.value.id, ruleText });
  show.value = false;
  emit("reload");
};

const onAdd = () => {
  const declarations = model.value.declarations as Partial<Declaration>[];
  declarations.push({
    id: newGuid(),
    propertyName: "",
    value: "",
  });
};

const onSelectedLog = (items: MediaFileItem[]) => {
  if (items.length === 1 && selected.value) {
    var _oldValue = selected.value.value,
      regex = _oldValue.match(/url\((\S+)\)/);
    if (regex && regex.length) {
      var newValue = _oldValue.split(regex[1]).join("'" + items[0].url + "'");
      selected.value.value = newValue;
    } else {
      selected.value.value = _oldValue + " url('" + items[0].url + "')";
    }
  }
  showMediaDialog.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="680px"
    :close-on-click-modal="false"
    :title="t('common.inlineStyle')"
    @closed="emit('update:modelValue', false)"
  >
    <div>{</div>
    <div class="ml-24 space-y-8">
      <div
        v-for="item of model.declarations"
        :key="item.id"
        class="flex items-center space-x-8"
        data-cy="style-rule-item"
      >
        <el-select
          v-model="item.propertyName"
          class="w-220px"
          filterable
          allow-create
          default-first-option
          :placeholder="t('common.selectProperty')"
          data-cy="style-rule-key-dropdown"
          @change="
            item.value = '';
            nextTick(() => inputs[item.id]?.focus());
          "
        >
          <el-option
            v-for="i of cssProperties"
            :key="i"
            :value="i"
            :label="i"
            data-cy="style-rule-key-opt"
          />
        </el-select>
        <span class="font-bold">:</span>
        <el-input
          :ref="(el:any) => (inputs[item.id] = el)"
          v-model="item.value"
          :placeholder="t('common.value')"
          class="w-200px"
          data-cy="style-rule-value"
        />
        <span class="font-bold">;</span>
        <el-button
          circle
          @click="
            model.declarations = model.declarations.filter((f) => f !== item)
          "
        >
          <el-icon
            class="iconfont icon-delete text-orange cursor-pointer"
            data-cy="remove"
          />
        </el-button>
        <el-color-picker
          v-if="isColor(item.propertyName)"
          :model-value="getColor(item.value)"
          show-alpha
          @change="(color: string) => {item.value = setColor(item.value, color)}"
        />
        <el-button
          v-if="isImage(item.propertyName)"
          circle
          data-cy="background-image-picker"
          @click="
            selected = item;
            showMediaDialog = true;
          "
        >
          <el-icon class="iconfont icon-photo text-blue cursor-pointer" />
        </el-button>
      </div>
      <el-button circle @click="onAdd">
        <el-icon
          class="iconfont icon-a-addto text-blue cursor-pointer"
          data-cy="add"
        />
      </el-button>
    </div>
    <div>}</div>

    <template #footer>
      <DialogFooterBar
        :permission="{ feature: 'style', action: 'edit' }"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
  <KMediaDialog
    v-if="showMediaDialog"
    v-model="showMediaDialog"
    @choose="onSelectedLog"
  />
</template>

<style lang="scss" scoped>
:deep(.el-select .el-input__inner) {
  padding-right: 54px;
}
// 修复颜色选择器中有空白的问题
:deep(.el-color-picker__trigger .el-color-picker__color) {
  border: none;
  outline: 1px solid var(--el-text-color-secondary);
}
</style>
