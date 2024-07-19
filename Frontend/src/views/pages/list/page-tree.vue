<template>
  <div ref="containerDom" class="h-full">
    <el-empty v-if="!treeData?.children" />
    <el-tree-v2
      v-if="treeData?.children"
      ref="tree"
      icon=""
      :height="containerDom?.offsetHeight || 400"
      highlight-current
      :indent="24"
      :data="treeData.children"
      :props="treeProps"
      node-key="id"
      :default-expanded-keys="[
        treeData.children.find((i) => i.title === 'body')?.id || '',
      ]"
      :filter-method="onFilterMethod"
      @current-change="onCurrentChange"
    >
      <template #default="{ data, node }">
        <div
          class="node-content w-full overflow-hidden flex items-center truncate text-l font-bold pr-15px"
        >
          <div
            class="flex-shrink-0 h-26px w-26px pl-12px flex items-center justify-center"
          >
            <template v-if="!node.isLeaf">
              <el-icon class="expanded-icon iconfont icon-reduce" />
              <el-icon class="contracted-icon iconfont icon-a-addto" />
            </template>
          </div>

          <div class="flex-shrink-0 pl-12px flex items-center">
            <el-checkbox :model-value="selectedKey === node.key" />
          </div>

          <el-tooltip
            placement="top"
            :show-after="1000"
            :content="data.alt || data.title"
          >
            <div class="pl-8px truncate">
              {{ data.title }}
            </div>
          </el-tooltip>

          <div class="flex-1 pl-12px" />

          <div class="flex items-center gap-8px">
            <el-tag
              v-if="data.sameSub"
              effect="plain"
              type="success"
              round
              size="small"
            >
              {{ t("common.same") }}
            </el-tag>

            <el-tag
              effect="plain"
              round
              size="small"
              @click.stop="showDetail(data)"
            >
              {{ data.pageCount }} {{ t("common.pages") }}
            </el-tag>
          </div>
        </div>
      </template>
    </el-tree-v2>
  </div>

  <ConvertToViewDialog
    v-if="showConvertToViewDialog"
    v-model="showConvertToViewDialog"
    :data="currentData!"
    @add-task="addTask"
  />

  <ConvertToHtmlDialog
    v-if="showConvertToHtmlDialog"
    v-model="showConvertToHtmlDialog"
    :data="currentData!"
    @add-task="addTask"
  />

  <EditHtmlDialog
    v-if="showEditHtmlDialog"
    v-model="showEditHtmlDialog"
    :edit-type="editType"
    :data="currentData!"
    @add-task="addTask"
  />

  <PageRelationDialog
    v-if="showPageRelationDialog"
    v-model="showPageRelationDialog"
    :data="relation!"
  />
</template>

<script lang="ts" setup>
import type { NodeGroup, ConvertToTypes, PageName } from "@/api/pages/types";
import { deleteDom, getPageNames } from "@/api/pages";
import ConvertToViewDialog from "./convert-to-view-dialog.vue";
import ConvertToHtmlDialog from "./convert-to-html-dialog.vue";
import EditHtmlDialog from "./edit-html-dialog.vue";
import PageRelationDialog from "./page-relation-dialog.vue";
import type { TreeNode } from "element-plus/es/components/tree-v2/src/types";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import type { TreeNodeData } from "element-plus/es/components/tree/src/tree.type";
import { ElMessage } from "element-plus";
import { showDeleteConfirm, showConfirm } from "@/components/basic/confirm";

const showConvertToViewDialog = ref(false);
const showConvertToHtmlDialog = ref(false);
const showEditHtmlDialog = ref(false);
const currentData = ref<NodeGroup>();
interface PropsType {
  treeData: NodeGroup;
}

const { t } = useI18n();
const tree = ref();
const containerDom = ref();
const treeProps: any = {
  label: "title",
  children: "children",
  disabled(data: TreeNodeData) {
    return !data.sameSub ? "true" : "";
  },
};

defineProps<PropsType>();
const emit = defineEmits<{
  (e: "reload"): void;
  (e: "addTask", value: string): void;
}>();

const selectedKey = ref<string | number>("");

async function onCurrentChange(data: TreeNodeData, node: TreeNode) {
  currentData.value = data as NodeGroup;
  selectedKey.value = node.key;
}

async function onConvertTo(type: ConvertToTypes, data: NodeGroup) {
  if (!data.sameSub) {
    await showConfirm(t("page.pageTreeDifferentTips"));
  }
  if (type === "View") {
    showConvertToViewDialog.value = true;
  } else {
    showConvertToHtmlDialog.value = true;
  }
}
const editType = ref<"innerHtml" | "outerHtml">("innerHtml");
async function edit(type: "innerHtml" | "outerHtml") {
  // 设置类型
  if (type === "innerHtml") {
    editType.value = "innerHtml";
  } else {
    editType.value = "outerHtml";
  }

  if (currentData.value) {
    if (!currentData.value.sameSub) {
      await showConfirm(t("page.pageTreeDifferentTips"));
    }
    showEditHtmlDialog.value = true;
  }
}

async function deleteSelected() {
  if (selectedKey.value) {
    await showDeleteConfirm();
    const taskId = await deleteDom(currentData.value!.pages!);

    ElMessage.warning(t("common.jobStartRunning"));

    addTask(taskId);
  }
}

const showPageRelationDialog = ref(false);
const relation = ref<PageName[]>([]);
async function showDetail(data: NodeGroup) {
  const pageNames = await getPageNames();
  const pages = data.pages.map((i) => i.pageId);
  relation.value = pageNames.filter((page) => pages.includes(page.pageId));
  showPageRelationDialog.value = true;
}

async function addTask(taskId: string) {
  emit("addTask", taskId);
}

function onFilterMethod(value: string, data: TreeNodeData) {
  if (data.alt.includes(value)) {
    return true;
  } else {
    return false;
  }
}

async function filter(keyword: string) {
  tree.value?.filter(keyword);
}

defineExpose({
  selectedKey,
  convertTo(type: ConvertToTypes) {
    onConvertTo(type, currentData.value!);
  },
  edit,
  deleteSelected,
  filter,
});
</script>

<style>
.el-tree-node > .el-tree-node__content > .node-content > div > .expanded-icon {
  display: none;
}

.el-tree-node
  > .el-tree-node__content
  > .node-content
  > div
  > .contracted-icon {
  display: block;
}

.el-tree-node.is-expanded
  > .el-tree-node__content
  > .node-content
  > div
  > .expanded-icon {
  display: block;
}

.el-tree-node.is-expanded
  > .el-tree-node__content
  > .node-content
  > div
  > .contracted-icon {
  display: none;
}
</style>
