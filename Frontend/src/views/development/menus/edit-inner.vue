<script lang="ts" setup>
import { getEdit, swap, deleteItem } from "@/api/menu";
import { ref } from "vue";
import type { Menu } from "@/api/menu/types";
import MenuItem from "./components/menu-item.vue";
import VueDraggable from "vuedraggable";
import type { SortEvent } from "@/global/types";
import ItemDialog from "./components/item-dialog.vue";
import TemplateDialog from "./components/template-dialog.vue";
import GuidInfo from "@/components/guid-info/index.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const props = defineProps<{ id: string }>();

const emit = defineEmits<{
  (e: "update:model", value: Menu): void;
}>();
const { t } = useI18n();

const siteStore = useSiteStore();
const model = ref<Menu>();
const showItemDialog = ref(false);
const showTemplateDialog = ref(false);
const selected = ref<Menu>();

const load = async () => {
  model.value = await getEdit(props.id, props.id);
  emit("update:model", model.value);
};

const onSort = async (menu: Menu, e: SortEvent) => {
  const newItem = menu.children[e.newIndex];
  const oldItem = menu.children.splice(e.oldIndex, 1)[0];
  menu.children.splice(e.newIndex, 0, oldItem);
  await swap(model.value!.id, oldItem.id, newItem.id);
  load();
};

const onAdd = (id: string) => {
  if (!siteStore.hasAccess("menu", "edit")) return;
  (selected.value as Partial<Menu>) = {
    parentId: id,
    rootId: model.value!.id,
    name: "",
    url: "",
    values: {},
    urls: {},
  };
  showItemDialog.value = true;
};

const onEdit = (value: Menu) => {
  if (!siteStore.hasAccess("menu", "edit")) return;
  selected.value = value;
  showItemDialog.value = true;
};

const onDelete = async (id: string) => {
  if (!siteStore.hasAccess("menu", "edit")) return;
  await showDeleteConfirm();
  await deleteItem(model.value!.id, id);
  load();
};

const onTemplateEdit = (value: Menu) => {
  selected.value = value;
  showTemplateDialog.value = true;
};

defineExpose({ onTemplateEdit });

load();
</script>

<template>
  <div v-if="model">
    <VueDraggable
      :model-value="model.children"
      :group="model.id"
      :item-key="model.id"
      class="space-y-8 mb-8"
      handle=".menu_move_handler"
      data-cy="menu-item-wrapper"
      @sort="onSort(model!, $event)"
    >
      <template #item="{ element }">
        <MenuItem
          :id="model.id"
          :menu="element"
          @reload="load"
          @add="onAdd"
          @delete="onDelete"
          @edit="onEdit"
          @template="onTemplateEdit"
        />
      </template>
    </VueDraggable>
    <div>
      <GuidInfo v-if="!model.children.length">
        {{ t(`common.createItemNowToCreateAItem`) }}
        <template #button>
          <el-button round type="primary" @click="onAdd(model!.id)">
            <el-icon class="iconfont icon-a-addto" />{{
              t("common.createItemNow")
            }}
          </el-button>
        </template>
      </GuidInfo>
      <IconButton
        v-else
        :permission="{ feature: 'menu', action: 'edit' }"
        circle
        icon="icon-a-addto"
        :tip="t('common.addMenuItem')"
        data-cy="add-item"
        @click="onAdd(model!.id)"
      />
    </div>

    <teleport to="body">
      <ItemDialog
        v-if="showItemDialog"
        v-model="showItemDialog"
        :menu="selected!"
        @reload="load"
      />
      <TemplateDialog
        v-if="showTemplateDialog"
        v-model="showTemplateDialog"
        :menu="selected!"
        @reload="load"
      />
    </teleport>
  </div>
</template>
