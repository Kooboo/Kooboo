<template>
  <el-form-item
    v-for="(group, index) in embeddedGroup"
    :key="index"
    :label="getLabel(group)"
  >
    <SortableList
      v-if="group.some((s) => s.group) && group.length > 1"
      class="!p-0 min-w-600px"
      :list="groupContents(group)"
      id-prop="id"
      @sort="onSort(groupContents(group), $event)"
      @delete="remove"
    >
      <template #default="{ item }">
        <div class="flex items-center">
          <div class="flex-1 truncate">{{ getTextFromGroup(item, group) }}</div>
          <ElTag round class="flex-shrink-0">{{
            getTypeFromGroup(item, group)
          }}</ElTag>
        </div>
      </template>
      <template #right="{ item }">
        <a
          class="cursor-pointer px-4 hover:text-blue"
          data-cy="edit"
          @click="editFromGroup(group, item)"
        >
          <el-icon class="iconfont icon-a-writein" />
        </a>
      </template>
      <template #bottom>
        <el-dropdown
          trigger="click"
          @command="edit(group.find((f) => f.embeddedFolder == $event)!)"
        >
          <el-button circle data-cy="add">
            <el-icon class="text-blue iconfont icon-a-addto" />
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item
                v-for="item of group"
                :key="item.embeddedFolder"
                :command="item.embeddedFolder"
              >
                <div class="flex gap-8">
                  <span>{{ t("common.create") }}</span>
                  <ElTag round type="info">{{
                    item.display || item.alias
                  }}</ElTag>
                </div>
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </template>
    </SortableList>
    <SortableList
      v-else
      class="!p-0 min-w-600px"
      :list="group[0].contents"
      id-prop="id"
      @sort="onSort(group[0].contents, $event)"
      @delete="remove"
      @add="edit(group[0])"
    >
      <template #default="{ item }">
        {{ getText(item, group[0]) }}
      </template>
      <template #right="{ item }">
        <a
          class="cursor-pointer px-4 hover:text-blue"
          data-cy="edit"
          @click="edit(group[0], item)"
        >
          <el-icon class="iconfont icon-a-writein" />
        </a>
      </template>
    </SortableList>
  </el-form-item>
  <Teleport to="body">
    <EditEmbeddedDialog
      v-model="visibleEdit"
      :current="current"
      :current-content="currentContent"
      :paths="[...(paths ?? []), getText(currentContent!, current)]"
      @save-success="onSaveSuccess"
    />
  </Teleport>
</template>

<script lang="ts" setup>
import type { SortEvent } from "@/global/types";
import { deletes } from "@/api/content/textContent";
import type {
  ContentEmbedded,
  TextContentItem,
} from "@/api/content/textContent";
import { useSync } from "@/hooks/use-sync";
import { computed, ref } from "vue";
import EditEmbeddedDialog from "./edit-embedded-dialog.vue";

import SortableList from "@/components/sortable-list/index.vue";
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: ContentEmbedded[];
  id?: string;
  paths?: string[];
}
const props = defineProps<PropsType>();
const emits = defineEmits<{
  (e: "update:modelValue"): void;
}>();

const { t } = useI18n();
const embeddeds = useSync(props, "modelValue", emits);
const visibleEdit = ref(false);
const current = ref<ContentEmbedded>({} as ContentEmbedded);
const currentContent = ref<TextContentItem>();

const embeddedGroup = computed(() => {
  const result: ContentEmbedded[][] = [];
  for (const i of embeddeds.value) {
    if (i.group) {
      var exist = result.find((f) => f.some((s) => s.group == i.group));
      if (exist) {
        exist.push(i);
      } else {
        result.push([i]);
      }
    } else {
      result.push([i]);
    }
  }

  return result;
});

function getText(content: TextContentItem, embedded: ContentEmbedded) {
  if (!content) return "";
  let key =
    content.summaryField ??
    embedded.columns.find((f: any) => f.isSummaryField)?.name ??
    Object.keys(content.textValues)[0];
  key =
    Object.keys(content.textValues).find(
      (f) => f.toLowerCase() == key.toLowerCase()
    ) ?? "";
  var value = content.textValues[key];

  const column = embedded.columns.find(
    (f: any) => f.name?.toLowerCase() == key?.toLowerCase()
  );
  if (column?.selectionOptions) {
    try {
      const options = JSON.parse(column.selectionOptions);
      let values = [value];
      if (column.controlType == "CheckBox") {
        values = JSON.parse(value);
      }
      const displayValues = [];
      for (const i of values) {
        const option = options.find((f: any) => f.value == i);
        if (option) displayValues.push(option.key);
        else displayValues.push(i);
      }
      value = displayValues.join(",");
    } catch {
      //
    }
  }
  return value;
}
function getTextFromGroup(content: TextContentItem, group: ContentEmbedded[]) {
  const embedded = group.find((f) => f.contents.indexOf(content) > -1);
  return getText(content, embedded!);
}

function getTypeFromGroup(content: TextContentItem, group: ContentEmbedded[]) {
  const embedded = group.find((f) => f.contents.indexOf(content) > -1);
  return embedded?.display || embedded?.alias;
}

async function remove(
  _id: string,
  _item: any,
  context: { list: unknown[]; index: number }
) {
  await showConfirm(t("common.deleteEmbeddedFolderTips"));
  const removedItems = context.list.splice(
    context.index,
    1
  ) as TextContentItem[];
  deletes({
    ids: removedItems.map((m) => m.id),
    parentId: props.id,
  });
  for (const i of embeddeds.value) {
    const index = i.contents.findIndex((f) => f.id == removedItems[0]?.id);
    if (index > -1) {
      i.contents.splice(index, 1);
    }
  }
}

function edit(embedded: ContentEmbedded, content?: TextContentItem) {
  current.value = embedded;
  currentContent.value = content;
  visibleEdit.value = true;
}

function editFromGroup(group: ContentEmbedded[], content: TextContentItem) {
  const embedded = group.find((f) => f.contents.indexOf(content) > -1);
  return edit(embedded!, content);
}

function onSaveSuccess(content: TextContentItem) {
  if (currentContent.value) {
    currentContent.value.textValues = content.textValues;
  } else {
    if (current.value.group) {
      const group = embeddedGroup.value.find((f) =>
        f.some((s) => s.group == current.value.group)
      );
      if (group) {
        let maxOrder = 0;
        for (const i of group) {
          for (const content of i.contents) {
            if (content.order > maxOrder) maxOrder = content.order;
          }
        }
        content.order = maxOrder + 1;
      }
    }
    current.value.contents.push(content);
  }
}

function onSort(list: TextContentItem[], e: SortEvent) {
  const oldItem = list.splice(e.oldIndex, 1)[0];
  list.splice(e.newIndex, 0, oldItem);
  list.forEach((it: TextContentItem, i: number) => {
    it.order = i;
  });
}

function getLabel(group: ContentEmbedded[]) {
  if (group.some((s) => s.group)) {
    var name = group[0].group?.split("##name##")[1];
    if (name) return name;
  }
  return group.map((m) => m.display || m.alias).join(",");
}

function groupContents(group: ContentEmbedded[]) {
  var result = [];
  for (const i of group) {
    result.push(...i.contents);
  }
  return result.sort((left, right) => (left.order > right.order ? 1 : -1));
}
</script>
