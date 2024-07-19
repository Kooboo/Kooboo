<script lang="ts" setup>
import { ref, toRefs } from "vue";
import type { Rules } from "async-validator";
import type { Page, StructureNode } from "@/api/pages/types";
import {
  rangeRule,
  isUniqueNameRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import {
  isUniqueName,
  copy,
  pageUrlIsUniqueName,
  getPageStructure,
} from "@/api/pages";
import { usePageStore } from "@/store/page";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import Schema from "async-validator";
import { newGuid } from "@/utils/guid";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; page?: Page }>();
const { t } = useI18n();
const pageStore = usePageStore();
const rules = {
  name: [
    requiredRule(t("common.nameRequiredTips")),
    rangeRule(1, 50),
    isUniqueNameRule(isUniqueName, t("common.pageExists")),
  ],
  url: [
    requiredRule(t("common.urlRequiredTips")),
    isUniqueNameRule(pageUrlIsUniqueName, t("common.urlOccupied")),
  ],
} as Rules;

const schema = new Schema({
  name: [
    requiredRule(t("common.nameRequiredTips")),
    rangeRule(1, 50),
    simpleNameRule(),
  ],
});

const show = ref(true);
const form = ref();
const tree = ref();

const model = ref({
  id: "",
  name: "",
  url: "",
  pageStructure: undefined as StructureNode[] | undefined,
});

function initTreeData(nodes: StructureNode[]) {
  for (const i of nodes) {
    i.newName = i.name + "_copy";
    i.invalidMessage = "";
    i.selected = false;
    i.guid = newGuid();
    if (i.children) initTreeData(i.children);
  }
}

if (props.page) {
  model.value.id = toRefs(props).page?.value?.id || "";
  let pageName = props.page.name || "";
  if (pageName.indexOf(".") > -1) {
    var _temp = pageName.split(".");
    _temp[0] = _temp[0] + "_copy";
    pageName = _temp.join(".");
  } else {
    pageName += "_copy";
  }
  model.value.name = pageName;
  model.value.url = "/" + pageName;
  getPageStructure(model.value.id).then((rsp) => {
    initTreeData(rsp);
    model.value.pageStructure = rsp;
  });
}

// 判断时候含有invalidMessage不为空的对象
function hasNonEmptyInvalidMessage(obj: any): boolean {
  if (Array.isArray(obj)) {
    for (const item of obj) {
      if (hasNonEmptyInvalidMessage(item)) {
        return true;
      }
    }
  } else if (typeof obj === "object" && obj !== null) {
    if ("invalidMessage" in obj && obj.invalidMessage !== "") {
      return true;
    }
    if (Array.isArray(obj.children)) {
      if (hasNonEmptyInvalidMessage(obj.children)) {
        return true;
      }
    }
    for (const key in obj) {
      if (typeof obj[key] === "object") {
        if (hasNonEmptyInvalidMessage(obj[key])) {
          return true;
        }
      }
    }
  }
  return false;
}

const onSave = async () => {
  const hasInvalidMessage: boolean = hasNonEmptyInvalidMessage(
    model.value.pageStructure
  );

  if (hasInvalidMessage) return;

  await form.value.validate();
  await copy(model.value);
  show.value = false;
  pageStore.load();
};

const checkChange = (
  data: StructureNode,
  checked: boolean,
  indeterminate: boolean
) => {
  data.selected = checked || indeterminate;

  if (data.selected) {
    let node = tree.value.getNode(data)?.parent;
    if (node) tree.value.setChecked(node.data, true);
  }
};

// 初始化数据的invalidMessage和newName
const initTreeStatusCombined = (data: any, isChildren: boolean) => {
  if ((isChildren && data.selected) || (!isChildren && !data.selected)) {
    if (data.invalidMessage !== "") {
      data.invalidMessage = "";
      data.newName = data.name + "_copy";
    }
  }
};

const onNodeCheck = (data: StructureNode) => {
  initTreeStatusCombined(data, false);
  if (!data.children?.length) return;

  function checkNodes(d: any, checked: boolean) {
    let node = tree.value.getNode(d);
    for (const i of node.childNodes) {
      tree.value.setChecked(i.data, checked);
      checkNodes(i.data, checked);
      initTreeStatusCombined(i.data, true);
    }
  }

  if (data.children?.some((f) => !f.selected)) {
    checkNodes(data, true);
  } else {
    checkNodes(data, false);
  }
};

const validName = async (data: StructureNode) => {
  schema
    .validate({ name: data.newName })
    .then(() => {
      data.invalidMessage = "";
    })
    .catch(({ errors, fields }) => {
      data.invalidMessage = errors[0].message;
    });
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.copyPage')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @keydown.enter="onSave"
    >
      <div class="flex space-x-8">
        <el-form-item :label="t('common.name')" prop="name" class="flex-1">
          <el-input v-model="model.name" data-cy="name" />
        </el-form-item>
        <el-form-item label="URL" prop="url" class="flex-1">
          <el-input
            v-model="model.url"
            data-cy="url"
            @input="model.url = model.url.replace(/\s+/g, '')"
          />
        </el-form-item>
      </div>
      <div v-if="model.pageStructure?.length">
        <div class="mb-8">
          <span>
            {{ t("common.references") }}
          </span>
          <el-tooltip
            class="box-item"
            :content="t('common.copyPageWithRelationsTips')"
            placement="top"
            popper-class="w-800px"
          >
            <el-icon class="iconfont icon-problem ml-8" />
          </el-tooltip>
        </div>
        <ElScrollbar :max-height="400">
          <div class="w-full truncate">
            <el-tree
              ref="tree"
              :data="model.pageStructure"
              show-checkbox
              node-key="guid"
              check-strictly
              default-expand-all
              :expand-on-click-node="false"
              @check-change="checkChange"
              @check="onNodeCheck"
            >
              <template #default="{ node, data }">
                <div class="flex items-center space-x-8 truncate">
                  <ElInput
                    v-if="node.checked || node.indeterminate"
                    v-model="data.newName"
                    class="w-180px flex-shrink-0"
                    size="small"
                    :placeholder="t('common.name')"
                    @input="validName(data)"
                  />
                  <div v-else>{{ data.name }}</div>
                  <div class="text-[#f56c6c] text-12px">
                    {{ data.invalidMessage }}
                  </div>
                  <ObjectTypeTag :type="data.type" />
                  <div :title="data.summary" class="text-s text-999 truncate">
                    {{ data.summary }}
                  </div>
                </div>
              </template>
            </el-tree>
          </div>
        </ElScrollbar>
      </div>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
