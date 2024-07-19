<script lang="ts" setup>
import { ref } from "vue";

const payload = ref();
const visible = ref(false);
const left = ref(0);
const top = ref(0);
const trigger = ref();

const openMenu = (e: { clientY: number; clientX: number }, item: any) => {
  var needTrigger = visible.value;
  visible.value = true;
  top.value = e.clientY;
  left.value = e.clientX;
  payload.value = item;
  setTimeout(() => {
    trigger.value.click();
    if (needTrigger) {
      setTimeout(() => {
        visible.value = true;
        setTimeout(() => {
          trigger.value.click();
        });
      }, 0);
    }
  }, 0);
};

defineProps<{
  actions: { name: string; invoke(item: any): Promise<void> }[];
}>();

defineExpose({ openMenu, payload });
</script>

<template>
  <div
    v-if="visible"
    :style="{ top: top - 2 + 'px', left: left - 32 + 'px' }"
    class="fixed"
  >
    <el-dropdown
      trigger="click"
      @command="$event.invoke(payload)"
      @visible-change="visible = $event"
    >
      <div ref="trigger" class="w-64 h-8" @contextmenu.stop.prevent="" />
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item
            v-for="item of actions"
            :key="item.name"
            :command="item"
            >{{ item.name }}
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>
