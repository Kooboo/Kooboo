<script lang="ts" setup>
import { provide, ref } from "vue";
import { useI18n } from "vue-i18n";
import BackgroundGroup from "./background-group.vue";
import SizeGroup from "./size-group.vue";
import FontGroup from "./font-group.vue";
import type { DomValueWrapper } from "@/global/types";
import { close } from ".";

const { t } = useI18n();
const show = ref(true);
const values = ref<DomValueWrapper[]>([]);
let success = ref(false);

const save = () => {
  success.value = true;
  closed();
};

const closed = () => {
  const result = values.value.filter((f) => f.origin != f.value);
  close(success.value, result);
};

provide("values", values.value);
</script>
<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.inlineStyle')"
    custom-class="el-dialog--zero-padding"
    draggable
    @closed="closed"
  >
    <div class="px-32 py-24">
      <el-form ref="form" label-position="top">
        <el-form-item :label="t('common.background')">
          <BackgroundGroup />
        </el-form-item>
        <el-form-item :label="t('common.size')">
          <SizeGroup />
        </el-form-item>
        <el-form-item :label="t('common.font')">
          <FontGroup />
        </el-form-item>
      </el-form>
    </div>

    <template #footer>
      <el-button round @click="show = false">{{
        t("common.cancel")
      }}</el-button>
      <el-button type="primary" round @click="save">{{
        t("common.save")
      }}</el-button>
    </template>
  </el-dialog>
</template>
