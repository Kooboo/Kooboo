<template>
  <div>
    <SubMenu>
      <template #name="{ flag }">
        <div
          v-if="
            (keyword &&
              getDeepestItemName(data).includes(keyword?.toLowerCase())) ||
            !keyword
          "
          class="h-26px border-b-1 border-gray dark:border-b-1 dark:border-999 flex items-center space-x-12 cursor-pointer pr-12 text-12px dark:text-fff/60"
          :style="{
            paddingLeft: 12 + data.floor * 6 + 'px',
          }"
        >
          <el-icon
            class="iconfont icon-pull-down origin-center transform transition duration-200 origin-center"
            :class="flag ? 'rotate-180 ' : 'rotate-0'"
          />
          <span class="ellipsis leading-26px" :title="data.name">{{
            data.name
          }}</span>
          <span class="hidden">{{ flag }}</span>
        </div>
      </template>
      <template v-for="item in data.children">
        <FileItem
          v-if="!item.children"
          :id="item.id"
          :key="item.id"
          :title="item.tabName"
          :name="item.name"
          :remove="!hiddenRemove ? true : false"
          :permission="permission"
          :has-expand-row="true"
          :padding="12 + item.floor * 6"
          @click="emit('click-handle', item)"
          @remove="emit('remove', item.id)"
          @contextmenu.prevent.stop="
            emit('click-contextMenu', { event: $event, item: item })
          "
        >
          <ElIcon
            v-if="showEdit"
            v-hasPermission="{
              feature: route.query.activity,
              action: 'edit',
              effect: 'hiddenIcon',
            }"
            class="iconfont icon-a-writein hover:text-blue"
            @click="emit('edit', item)"
          />
          <el-icon
            v-if="showSetting"
            class="iconfont icon-a-setup hover:text-blue"
            @click="emit('setting', item.id)"
          />
        </FileItem>

        <ReSubMenu
          v-else
          :key="item.folderName"
          :data="item"
          :permission="permission"
          :hidden-remove="hiddenRemove"
          :show-edit="showEdit"
          :show-setting="showSetting"
          @remove="emit('remove', $event)"
          @click-handle="emit('click-handle', $event)"
          @edit="emit('edit', $event)"
          @click-context-menu="emit('click-contextMenu', $event)"
        />
      </template>
    </SubMenu>
  </div>
</template>

<script lang="ts" setup>
import SubMenu from "./sub-menu.vue";
import FileItem from "@/views/dev-mode/components/file-item.vue";
import type { Ref } from "vue";

import { useRoute } from "vue-router";
import { inject } from "vue";

const route = useRoute();

const keyword = inject<Ref<string>>("keyword");
const getDeepestItemName = (obj: {
  name: string;
  folderName: string;
  children: any[];
  floor: number;
}): string => {
  if (!obj?.children || obj?.children.length === 0) {
    return obj.name.toLowerCase();
  } else {
    return getDeepestItemName(obj?.children[0]);
  }
};

defineProps<{
  data?: any;
  showEdit?: boolean;
  showSetting?: boolean;
  permission: string;
  hiddenRemove?: string;
}>();

const emit = defineEmits<{
  (e: "click-handle", value: any): void;
  (e: "remove", value: string): void;
  (e: "click-contextMenu", value: any): void;
  (e: "edit", value: any): void;
  (e: "setting", value: any): void;
}>();
</script>
