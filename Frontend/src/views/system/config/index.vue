<script lang="ts" setup>
import { getCoreSetting } from "@/api/core-setting";
import type { CoreSetting } from "@/api/core-setting/types";
import { computed, ref } from "vue";
import EditDialog from "./edit-dialog.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { getQueryString } from "@/utils/url";

import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const list = ref<CoreSetting[]>([]);
const showDialog = ref(false);
const current = ref<CoreSetting>();
const activeGroup = ref<string[]>([]);
const group = getQueryString("group");
const siteStore = useSiteStore();
if (group) {
  activeGroup.value.push(group);
}

const load = async () => {
  list.value = await getCoreSetting();
};

const groups = computed(() => {
  const result: Record<string, CoreSetting[]> = {};

  for (const i of list.value) {
    if (!result[i.group]) {
      result[i.group] = [];
    }
    result[i.group].push(i);
  }

  return result;
});

const onEdit = (item: CoreSetting) => {
  if (siteStore.hasAccess("config", "edit")) {
    current.value = item;
    showDialog.value = true;
  }
};

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.config')" />
    <el-collapse
      v-model="activeGroup"
      class="rounded-normal bg-fff my-32 overflow-hidden el-collapse--no-border dark:bg-[#333]"
    >
      <el-collapse-item
        v-for="(value, key) in groups"
        :key="key"
        :title="key"
        :name="key"
      >
        <el-row
          v-for="item of value"
          :key="item.name"
          class="px-44px pt-16 flex items-center"
        >
          <el-col :span="4">{{ item.name }}</el-col>
          <el-col :span="14" class="ellipsis" :title="item.value">{{
            item.value
          }}</el-col>
          <el-col :span="6">
            <IconButton
              :permission="{ feature: 'config', action: 'edit' }"
              circle
              class="text-blue ml-12"
              icon="icon-a-writein "
              :tip="t('common.edit')"
              :data-cy="item.name"
              @click="onEdit(item)"
            />
          </el-col>
        </el-row>
      </el-collapse-item>
    </el-collapse>
  </div>
  <EditDialog
    v-if="showDialog"
    v-model="showDialog"
    :config="current!"
    @reload="load"
  />
</template>
