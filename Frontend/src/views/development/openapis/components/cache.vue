<script lang="ts" setup>
import type { Cache } from "@/api/openapi/types";
import { expiresInRule } from "@/utils/validate";
import type { Rules } from "async-validator";

import { useI18n } from "vue-i18n";
defineProps<{ list: Cache[] }>();
const { t } = useI18n();

const rules = {
  expiresIn: [expiresInRule(t("common.integerRuleTips"))],
} as Rules;
</script>

<template>
  <div class="space-y-4">
    <div
      v-for="(item, index) of list"
      :key="index"
      class="flex items-center space-x-4"
    >
      <el-select v-model="item.method" class="w-138px">
        <el-option value="Get" label="GET" data-cy="method-get" />
        <el-option value="Put" label="PUT" data-cy="method-put" />
        <el-option value="Post" label="POST" data-cy="method-post" />
        <el-option value="Delete" label="DELETE" data-cy="method-delete" />
        <el-option value="Options" label="OPTIONS" data-cy="method-options" />
        <el-option value="Head" label="HEAD" data-cy="method-head" />
        <el-option value="Patch" label="PATCH" data-cy="method-patch" />
        <el-option value="Trace" label="TRACE" data-cy="method-trace" />
        <el-option value="All" label="ALL" data-cy="method-all" />
      </el-select>
      <el-input
        v-model="item.pattern"
        :placeholder="t('common.inputCachePathTips')"
        class="w-400px"
        data-cy="cache-path"
      />
      <el-form :model="item" :rules="rules" @submit.prevent>
        <el-form-item prop="expiresIn">
          <el-input v-model.number="item.expiresIn" class="w-138px">
            <template #append>
              <div class="px-12 text-blue">
                {{ t("common.minutes") }}
              </div>
            </template>
          </el-input>
        </el-form-item>
      </el-form>

      <div>
        <el-button
          circle
          class="text-orange"
          data-cy="delete"
          @click="list.splice(index, 1)"
        >
          <el-icon class="iconfont icon-delete" />
        </el-button>
      </div>
    </div>
    <el-button
      circle
      class="text-blue"
      data-cy="add"
      @click="list.push({ method: 'Get', expiresIn: 1, pattern: '' })"
    >
      <el-icon class="iconfont icon-a-addto" />
    </el-button>
  </div>
</template>
