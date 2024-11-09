<script lang="ts" setup>
defineProps<{
  options?: string[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onSelected(value: string) {
  emit("update:model-value", value);
}
</script>

<template>
  <ElInput
    v-bind="$attrs"
    @update:model-value="emit('update:model-value', $event)"
  >
    <template #suffix>
      <el-dropdown
        v-if="options?.length"
        trigger="click"
        class="ml-10px"
        @command="onSelected"
      >
        <div class="w-16 h-16 flex items-center justify-center">
          <el-icon class="iconfont icon-pull-down text-12px" />
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              v-for="item of options"
              :key="item"
              :command="item"
            >
              <span>{{ item }}</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </template>
  </ElInput>
</template>
