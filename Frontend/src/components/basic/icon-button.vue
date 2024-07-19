<script lang="ts">
export default {
  inheritAttrs: false,
};
</script>

<script lang="ts" setup>
defineProps<{
  circle?: boolean;
  icon: string;
  tip: string;
  permission?: {
    feature: string;
    action?: string;
  };
}>();
</script>

<template>
  <el-tooltip v-if="circle" placement="top" :content="tip">
    <el-button
      v-hasPermission="{
        feature: permission?.feature,
        action: permission?.action,
        effect: 'circle',
      }"
      circle
      v-bind="$attrs"
    >
      <el-icon class="iconfont" :class="icon" />
    </el-button>
  </el-tooltip>
  <span v-else class="px-8">
    <el-popconfirm
      v-if="$attrs['onConfirm']"
      :title="tip"
      @confirm="$attrs['onConfirm']"
    >
      <template #reference>
        <el-icon
          v-hasPermission="{
            feature: permission?.feature,
            action: permission?.action,
            effect: 'icon',
          }"
          class="iconfont text-l cursor-pointer hover:text-blue"
          :class="icon"
          v-bind="$attrs"
        />
      </template>
    </el-popconfirm>
    <el-tooltip v-else placement="top" :content="tip">
      <el-icon
        v-hasPermission="{
          feature: permission?.feature,
          action: permission?.action,
          effect: 'icon',
        }"
        class="iconfont text-l cursor-pointer hover:text-blue"
        :class="icon"
        v-bind="$attrs"
      />
    </el-tooltip>
  </span>
</template>
