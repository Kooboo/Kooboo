<script lang="ts" setup>
import { swap } from "@/api/menu";
import type { Menu } from "@/api/menu/types";
import type { SortEvent } from "@/global/types";
import VueDraggable from "vuedraggable";
import IconButton from "@/components/basic/icon-button.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "reload"): void;
  (e: "add", parentId: string): void;
  (e: "edit", value: Menu): void;
  (e: "template", value: Menu): void;
  (e: "delete", id: string): void;
}>();

const props = defineProps<{ menu: Menu; id: string }>();
const { t } = useI18n();

const onSort = async (menu: Menu, e: SortEvent) => {
  const newItem = menu.children[e.newIndex];
  const oldItem = menu.children.splice(e.oldIndex, 1)[0];
  menu.children.splice(e.newIndex, 0, oldItem);
  await swap(props.id, oldItem.id, newItem.id);
  emit("reload");
};
</script>

<template>
  <div
    class="text-666 dark:text-fff/86 pr-8 pl-16 w-full border-line dark:border-444 rounded-normal border border-solid ellipsis bg-fff dark:bg-444"
    data-cy="menu-item"
  >
    <div class="flex items-center text-m">
      <div class="space-x-8 flex">
        <p
          data-cy="menu-item-name"
          class="max-w-300px ellipsis"
          :title="menu.name"
        >
          {{ menu.name }}
        </p>

        <p
          class="text-blue max-w-500px ellipsis"
          data-cy="menu-item-url"
          :title="menu.url"
        >
          {{ menu.url }}
        </p>
      </div>
      <div class="flex-1" />

      <IconButton
        v-if="menu.children?.length"
        icon="icon-code"
        :tip="t('common.editTemplate')"
        data-cy="edit-item-template"
        @click="$emit('template', menu)"
      />

      <IconButton
        v-if="!menu.children?.length"
        :permission="{ feature: 'menu', action: 'edit' }"
        icon="icon-a-addto"
        :tip="t('common.add')"
        data-cy="add-sub-item"
        @click="$emit('add', menu.id)"
      />

      <IconButton
        :permission="{ feature: 'menu', action: 'edit' }"
        icon="icon-a-writein"
        :tip="t('common.edit')"
        data-cy="edit-item"
        @click="$emit('edit', menu)"
      />

      <IconButton
        :permission="{ feature: 'menu', action: 'edit' }"
        class="text-orange hover:text-orange"
        icon="icon-delete"
        :tip="t('common.delete')"
        data-cy="delete-item"
        @click="$emit('delete', menu.id)"
      />
      <div class="p-8 cursor-move menu_move_handler" data-cy="move">
        <el-icon class="iconfont icon-move text-16px" />
      </div>
    </div>
    <div v-if="menu.children?.length" class="pl-32 pr-24 mt-8 mb-16">
      <VueDraggable
        :model-value="menu.children"
        :group="menu.id"
        :item-key="id"
        class="space-y-8 mb-8"
        handle=".menu_move_handler"
        data-cy="sub-menu-item-wrapper"
        @sort="onSort(menu, $event)"
      >
        <template #item="{ element }">
          <MenuItem
            :id="id"
            :menu="element"
            @reload="$emit('reload')"
            @add="$emit('add', $event)"
            @edit="$emit('edit', $event)"
            @template="$emit('template', $event)"
            @delete="$emit('delete', $event)"
          />
        </template>
      </VueDraggable>

      <el-button circle @click="$emit('add', menu.id)">
        <el-icon class="text-blue iconfont icon-a-addto" />
      </el-button>
    </div>
  </div>
</template>
