<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { useAttribute } from "@/utils/dom";
import { close, anchor } from "./link-dialog";
import { pageLinks } from "../page";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const { t } = useI18n();
const show = ref(true);
const element = anchor.value!;
const link = useAttribute(element, "href");
const success = ref(false);

const save = () => {
  success.value = true;
  show.value = false;
};

const closed = () => {
  return close(success.value, [link]);
};
</script>
<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.link')"
    custom-class="el-dialog--zero-padding"
    draggable
    @closed="closed"
  >
    <Alert :content="t('common.linkDialogTip')" />
    <div class="px-32 py-24">
      <el-form ref="form" label-position="top">
        <el-form-item :label="t('common.url')">
          <ElSelect
            v-model="link"
            filterable
            allow-create
            default-first-option
            class="w-full"
          >
            <ElOption
              v-for="item of pageLinks"
              :key="item"
              :label="item"
              :value="item"
            />
          </ElSelect>
        </el-form-item>
      </el-form>
    </div>

    <template #footer>
      <DialogFooterBar @confirm="save" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
