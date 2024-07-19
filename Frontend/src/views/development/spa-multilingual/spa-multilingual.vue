<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.spaMultilingual')" />

    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'spaMultilingual', action: 'edit' }"
        round
        data-cy="upload"
        @click="dialog.open('upload')"
      >
        <el-icon class="iconfont icon-a-Cloudupload" />
        {{ t("common.import") }}
      </el-button>

      <el-button round data-cy="export" @click="exportJson()">
        <el-icon class="iconfont icon-share" />
        {{ t("common.exportAll") }}
      </el-button>

      <el-button
        v-hasPermission="{ feature: 'spaMultilingual', action: 'edit' }"
        round
        data-cy="setting"
        @click="dialog.open('setting')"
      >
        <el-icon class="iconfont icon-a-setup" />
        {{ t("common.setting") }}
      </el-button>
    </div>

    <KTable
      :data="list"
      show-check
      table-layout="auto"
      :permission="{ feature: 'spaMultilingual', action: 'delete' }"
      @delete="onDelete"
    >
      <template #bar="{ selected }">
        <IconButton
          v-if="selected.length"
          class="text-blue"
          icon="icon-share"
          :tip="t('common.exportSelectedItems')"
          circle
          data-cy="export-seleted"
          @click="exportJson(selected)"
        />
      </template>

      <template v-if="list?.length">
        <el-table-column :label="t('common.name')" width="150">
          <template #default="{ row }">
            <span class="ellipsis" :title="row.name" data-cy="name">{{
              row.name
            }}</span>
          </template></el-table-column
        >
        <el-table-column
          v-for="(key, index) in keys"
          :key="key"
          min-width="160"
          :label="index === 0 ? key + `(${t('common.default')})` : key"
        >
          <template #default="{ row }">
            {{ row.value?.[key] }}
          </template>
        </el-table-column>
        <el-table-column fixed="right" width="70" align="right">
          <template #default="{ row }">
            <IconButton
              :permission="{ feature: 'spaMultilingual', action: 'edit' }"
              icon="icon-a-writein"
              :tip="t('common.edit')"
              data-cy="edit"
              @click="dialog.open('edit', row)"
            />
          </template>
        </el-table-column>
      </template>
    </KTable>

    <SettingDialogVue
      :model-value="dialog.current === 'setting'"
      :data="keys"
      @close="dialog.close"
      @save-success="load"
    />
    <UploadDialogVue
      :model-value="dialog.current === 'upload'"
      :is-null="!(list.length > 0)"
      @close="dialog.close"
      @save-success="load"
    />
    <EditDialogVue
      :data="dialog.row!"
      :keys="keys"
      :model-value="dialog.current === 'edit'"
      @close="dialog.close"
      @save-success="load"
    />
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import EditDialogVue from "./components/edit-dialog.vue";
import SettingDialogVue from "./components/setting-dialog.vue";
import UploadDialogVue from "./components/upload-dialog.vue";
import { getList, deletes } from "@/api/development/spa-multilingual";
import type { Multilingual, Langs } from "@/api/development/spa-multilingual";
import { useDate } from "@/hooks/use-date";
import IconButton from "@/components/basic/icon-button.vue";
import { useSiteStore } from "@/store/site";
import { showDeleteConfirm } from "@/components/basic/confirm";

const { t } = useI18n();

const list = ref<Multilingual[]>([]);
const keys = ref<Langs[]>([]);
const siteStore = useSiteStore();
const dialog = reactive({
  current: "" as undefined | "setting" | "edit" | "upload",
  row: undefined as undefined | Multilingual,
  open(key: "setting" | "edit" | "upload", row?: Multilingual) {
    if (key === "edit" && !siteStore.hasAccess("spaMultilingual", "edit")) {
      return;
    }
    dialog.current = key;
    if (key === "edit" && row) {
      dialog.row = row;
    }
  },
  close() {
    dialog.current = undefined;
    dialog.row = undefined;
  },
});

const load = async () => {
  list.value = await getList();
  if (list.value?.length) {
    // 找到key-value最多的一组翻译, 计算出覆盖的语言
    let valueLen = 1;
    let maxIndex = 0;
    list.value.forEach((item, index) => {
      const len = Object.keys(item.value || {}).length;
      if (len > valueLen) {
        valueLen = len;
        maxIndex = index;
      }
    });
    const { value, defaultLang } = list.value[maxIndex];
    keys.value = Object.keys(value || {}) as Langs[];
    // 默认语言 置顶
    const index = keys.value.findIndex((i) => i === defaultLang);
    index !== -1 && keys.value.splice(index, 1);
    keys.value.unshift(defaultLang);
  }
};

load();

const onDelete = async (params: Multilingual[]) => {
  await showDeleteConfirm(params.length);
  await deletes(params.map((i) => i.id));
  load();
};

const exportJson = (data?: Multilingual[]) => {
  const obj: any = {};
  // 导出JSON格式
  if (data?.length) {
    data.forEach((item) => {
      obj[item.name] = item.value;
    });
  } else {
    list.value.forEach((item) => {
      obj[item.name] = item.value;
    });
  }

  // 下载文件
  const str = JSON.stringify(obj, null, 2);
  const blob = new Blob([str]);
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `translate-${useDate(new Date().toString(), "YYYYMMDD")}.json`;
  a.click();
};
</script>
