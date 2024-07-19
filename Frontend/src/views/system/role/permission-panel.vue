<script lang="ts" setup>
import type { Permission } from "@/api/role/types";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{ list: Permission[] }>();
const { t } = useI18n();

const features = computed(() => {
  if (!props.list) return [];
  return new Set(props.list.map((m) => m.feature));
});

const featureItems = (feature: string) => {
  const set = new Set<Permission>();
  const list = props.list.filter((f) => f.feature === feature);
  set.add(list.find((f) => f.action === "view")!);
  set.add(list.find((f) => f.action === "edit")!);
  set.add(list.find((f) => f.action === "delete")!);

  for (const i of list) {
    set.add(i);
  }

  set.delete(undefined!);
  return Array.from(set);
};

const isSelectAll = (list: Permission[]) => list.every((e) => e.access);

const isSelectSome = (list: Permission[]) =>
  list.some((e) => e.access) && !isSelectAll(list);

const onSelectAll = (list: Permission[]) => {
  if (isSelectAll(list)) {
    list.forEach((m) => (m.access = false));
  } else {
    list.forEach((m) => (m.access = true));
  }
};

const featureDisplay = (name: string) => {
  return t(`common.${name}`, name);
};

const actionDisplay = (name: string) => {
  if (name === "view") return t("permission.view");
  return t(`common.${name}`, name);
};

const dependViewActions = ["edit", "delete", "debug"];

const onSelectFeatureItem = (permission: Permission, checked: any) => {
  if (checked && dependViewActions.includes(permission.action)) {
    selectViewItem(permission.feature);
  } else if (permission.action == "view") {
    const cantCancel = featureItems(permission.feature).some(
      (f) => dependViewActions.includes(f.action) && f.access
    );

    if (cantCancel) selectViewItem(permission.feature);
  }
};

const selectViewItem = (feature: string) => {
  const viewItem = featureItems(feature).find((f) => f.action == "view");
  if (viewItem) viewItem.access = true;
};
</script>

<template>
  <el-row>
    <el-col :span="6">
      <el-checkbox
        size="large"
        :model-value="isSelectAll(list)"
        :indeterminate="isSelectSome(list)"
        @update:model-value="onSelectAll(list)"
      >
        <span class="text-blue">{{ t("common.selectAll") }}</span>
      </el-checkbox>
    </el-col>
    <el-col v-for="feature of features" :key="feature" :span="6">
      <el-checkbox
        v-if="featureItems(feature).length === 1"
        v-model="featureItems(feature)[0].access"
        size="large"
        :label="featureDisplay(feature)"
      />
      <el-popover v-else placement="right" trigger="click">
        <div>
          <el-checkbox
            size="large"
            :model-value="isSelectAll(featureItems(feature))"
            :indeterminate="isSelectSome(featureItems(feature))"
            @update:model-value="onSelectAll(featureItems(feature))"
          >
            <span class="text-blue">{{ t("common.selectAll") }}</span>
          </el-checkbox>
          <el-checkbox
            v-for="item of featureItems(feature)"
            :key="feature + item"
            v-model="item.access"
            size="large"
            :label="actionDisplay(item.action)"
            @update:model-value="onSelectFeatureItem(item, $event)"
          />
        </div>
        <template #reference>
          <el-checkbox
            size="large"
            :label="featureDisplay(feature)"
            :model-value="isSelectAll(featureItems(feature))"
            :indeterminate="isSelectSome(featureItems(feature))"
          />
        </template>
      </el-popover>
    </el-col>
  </el-row>
</template>
