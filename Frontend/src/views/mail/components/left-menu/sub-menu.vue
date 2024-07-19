<template>
  <el-sub-menu
    v-if="menu.items?.length"
    :index="'folder' + menu.name"
    :class="{
      dark: dark,
      active:
        (route.name !== 'compose' &&
          route.query.folderName &&
          menu.name === route.query.folderName) ||
        emailStore.activeName === 'folder' + menu.name,
    }"
  >
    <template #title>
      <div
        class="w-full flex items-center justify-between group"
        :style="{
          paddingLeft: 26 + menu.floor! * 10 + 'px',
        }"
        @mouseenter="mouseHover"
        @mouseleave="mouseHover"
        @click.stop="
          emits('changeMenu', {
            name: 'folder',
            folderName: menu.name,
          })
        "
      >
        <div class="flex items-center w-full justify-between pl-20px">
          <div class="flex items-center w-full">
            <span
              class="ellipsis"
              :title="menu.name"
              :style="calculateMaxWidth(menu.floor!)"
              >{{ menu.displayName }}
            </span>
            <span
              v-if="menu.unRead !== 0"
              class="font-bold mx-4 group-hover:hidden"
              >({{ menu.unRead }})</span
            >
          </div>

          <div
            class="hidden items-center font-normal space-x-8 group-hover:flex"
          >
            <el-icon
              class="iconfont icon-a-writein !hover:text-blue !text-s !w-12 !text-444 !dark:text-999 !mr-0"
              @click.stop="emits('editFolder', menu.name)"
            />
            <el-icon
              class="iconfont icon-delete !hover:text-orange !text-s !w-12 !text-444 !dark:text-999 !mr-0"
              @click.stop="emits('deleteFolder', menu)"
            />
          </div>
        </div>
      </div>
    </template>
    <!-- 多级嵌套菜单渲染 -->
    <SubMenu
      v-for="menuItem in menu.items"
      :key="menuItem.id"
      :menu="menuItem"
      @change-menu="
        emits('changeMenu', {
          name: 'folder',
          folderName: $event.folderName,
        })
      "
      @edit-folder="emits('editFolder', $event)"
      @delete-folder="emits('deleteFolder', $event)"
      @expand-sub-menu="emits('expandSubMenu', $event)"
    />
  </el-sub-menu>
  <el-menu-item v-else :index="'folder' + menu.name">
    <template #title>
      <div
        class="w-full flex items-center justify-between group"
        :style="{
          paddingLeft: 26 + menu.floor! * 10 + 'px',
        }"
        @mouseenter="mouseHover"
        @mouseleave="mouseHover"
        @click.stop="
          emits('changeMenu', {
            name: 'folder',
            folderName: menu.name,
          })
        "
      >
        <div class="flex items-center w-full justify-between">
          <div class="flex items-center w-full">
            <span
              class="ellipsis"
              :title="menu.name"
              :style="calculateMaxWidth(menu.floor!)"
              >{{ menu.displayName }}
            </span>
            <span
              v-if="menu.unRead !== 0"
              class="font-bold mx-4 group-hover:hidden"
              >({{ menu.unRead }})</span
            >
          </div>

          <div
            class="hidden items-center font-normal space-x-8 group-hover:flex"
          >
            <el-icon
              class="iconfont icon-a-writein !hover:text-blue !text-s !w-12 !text-444 !dark:text-999 !mr-0"
              @click.stop="emits('editFolder', menu.name)"
            />
            <el-icon
              class="iconfont icon-delete !hover:text-orange !text-s !w-12 !text-444 !dark:text-999 !mr-0"
              @click.stop="emits('deleteFolder', menu)"
            />
          </div>
        </div></div
    ></template>
  </el-menu-item>
</template>

<script lang="ts" setup>
import type { EmailFolder } from "@/api/mail/types";
import { useEmailStore } from "@/store/email";
import { useRoute } from "vue-router";
import { dark } from "@/composables/dark";
import { onMounted, ref, watch } from "vue";

const route = useRoute();
const emailStore = useEmailStore();

const props = defineProps<{
  menu: EmailFolder;
}>();

const emits = defineEmits<{
  (e: "changeMenu", value: { name: string; folderName: string }): void;
  (e: "editFolder", value: string): void;
  (e: "deleteFolder", value: EmailFolder): void;
  (e: "expandSubMenu", value: string): void;
}>();
const isHover = ref(false);

onMounted(async () => {
  // 如果当前激活的菜单是带有子文件夹的菜单,则手动展开其父菜单
  if (
    route.query.folderName &&
    route.query.folderName === props.menu.name &&
    props.menu.items?.length
  ) {
    emits("expandSubMenu", props.menu.name);
  }
});
const mouseHover = () => {
  isHover.value = !isHover.value;
};
const calculateMaxWidth = (floor: number) => {
  // 根据鼠标是否hover来设置不同的最大宽度(不用“:class”是因为会出现动态类名会未编译的问题)
  return {
    "max-width": isHover.value
      ? 80 - floor * 10 + "px"
      : 120 - floor * 10 + "px",
  };
};
watch(
  () => route.name,
  (newValue, oldValue) => {
    // 从搜索列表到文件夹列表时，展开对应的子文件夹
    if (route.query.folderName && oldValue === "searchEmail") {
      emits("expandSubMenu", props.menu.name);
    }
  }
);
</script>
