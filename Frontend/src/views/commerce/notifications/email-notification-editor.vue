<script lang="ts" setup>
import type { EmailEvent, EmailNotification } from "@/api/commerce/settings";
import { ref } from "vue";
import EmailNotificationDialog from "./email-notification-dialog.vue";
const props = defineProps<{
  items: EmailNotification[];
  events: EmailEvent[];
}>();

const showDialog = ref(false);
const selected = ref();

function onAdd() {
  selected.value = undefined;
  showDialog.value = true;
}

function onSave(value: EmailNotification) {
  if (selected.value) {
    const index = props.items.indexOf(selected.value);
    props.items.splice(index, 1, value);
  } else {
    props.items.push(value);
  }
}

function onEdit(item: EmailNotification) {
  selected.value = item;
  showDialog.value = true;
}

function onDelete(index: number) {
  props.items.splice(index, 1);
}
</script>

<template>
  <div class="space-y-4 w-full">
    <div v-for="(item, index) of items" :key="index">
      <div
        class="text-666 dark:text-fff/67 flex items-center pl-16 pr-8 w-full border-line dark:border-666 rounded-normal border border-solid ellipsis bg-fff dark:bg-[#333]"
        data-cy="added-item"
      >
        <div
          class="overflow-hidden leading-40px flex-1 mr-8 ellipsis"
          data-cy="text"
        >
          {{ events.find((f) => f.name == item.event)?.description }}
        </div>
        <div>
          <el-tag round>{{
            events.find((f) => f.name == item.event)?.display
          }}</el-tag>
        </div>
        <div class="p-4">
          <el-icon
            class="iconfont icon-a-setup cursor-pointer"
            data-cy="edit"
            @click.stop="onEdit(item)"
          />
        </div>
        <div class="p-4">
          <el-icon
            class="text-orange iconfont icon-delete cursor-pointer"
            color="#fab6b6"
            data-cy="remove"
            @click.stop="onDelete(index)"
          />
        </div>
      </div>
    </div>
    <el-button circle data-cy="add" @click="onAdd">
      <el-icon class="text-blue iconfont icon-a-addto" />
    </el-button>
    <div>
      <EmailNotificationDialog
        v-if="showDialog"
        v-model="showDialog"
        :events="events"
        :model="selected"
        @success="onSave"
      />
    </div>
  </div>
</template>
