<script lang="ts" setup>
import { computed, inject } from "vue";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";
import type { DataSchema } from "./data";
import KInput from "./k-input.vue";
import { useI18n } from "vue-i18n";
import { GetModuleUrl } from "./utils";

interface Props {
  data?: string;
  schemas?: DataSchema[];
  to?: "string";
}

const props = defineProps<Props>();
const pageState = inject<PageState>(PAGE_STATE_KEY);
const { t } = useI18n();

const formItems = computed(() => {
  if (props.schemas) return props.schemas;
  if (!props.data) return;
  return pageState?.getState<DataSchema[]>(props.data, "items").value;
});

async function post() {
  if (props.data) {
    const action = pageState?.getExposedAction(props.data, "post");
    if (action) await action();
    if (props.to) {
      location.href = GetModuleUrl(props.to);
    } else {
      location.reload();
    }
  }
}
</script>

<template>
  <el-form v-if="formItems" label-position="top" @submit.prevent>
    <template v-for="item of formItems" :key="item.name">
      <el-form-item v-if="!item.hidden" :label="item.label ?? item.name">
        <template v-if="item.type == 'object'">
          <el-card shadow="never" class="w-full">
            <KForm :schemas="item.children" />
          </el-card>
        </template>

        <KInput
          v-else
          v-model="item.data.value"
          :type="item.type"
          :options="item.options"
        />
      </el-form-item>
    </template>

    <el-button v-if="data" type="primary" @click="post">{{
      t("common.submit")
    }}</el-button>
  </el-form>
</template>
