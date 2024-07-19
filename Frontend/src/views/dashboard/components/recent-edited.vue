<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";
import ObjectTypeTag from "@/components/k-tag/object-type-tag.vue";

const { t } = useI18n();

defineProps<{ list: any }>();
</script>

<template>
  <el-card shadow="always">
    <div class="mb-16 text-18px">{{ t("common.editLog") }}</div>
    <KTable :data="list">
      <el-table-column :label="t('common.name')" min-width="150">
        <template #default="{ row }">{{ row.displayName }}</template>
      </el-table-column>

      <el-table-column
        :label="t('common.objectType')"
        width="110"
        align="center"
      >
        <template #default="{ row }">
          <ObjectTypeTag :type="row.storeName" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.action')" width="90" align="center">
        <template #default="{ row }">
          <ActionTag :type="row.actionType" />
        </template>
      </el-table-column>

      <el-table-column :label="t('common.user')">
        <template #default="{ row }"
          ><span class="ellipsis">{{ row.userName }}</span></template
        >
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" width="180">
        <template #default="{ row }">{{ useTime(row.lastModify) }}</template>
      </el-table-column>
    </KTable>
  </el-card>
</template>
