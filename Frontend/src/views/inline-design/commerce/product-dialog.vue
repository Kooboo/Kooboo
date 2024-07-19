<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import EditForm from "@/views/commerce/products-management/edit-form.vue";
import { close, productId } from ".";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const { t } = useI18n();
const form = ref();
const success = ref(false);
const show = ref(true);

const save = async () => {
  await form.value.onSave();
  success.value = true;
  show.value = false;
};
</script>
<template>
  <el-dialog
    v-model="show"
    width="1100px"
    :close-on-click-modal="false"
    :title="t('commerce.product')"
    custom-class="el-dialog--zero-padding"
    @closed="close(success)"
  >
    <Alert :content="t('common.immediatelyChange')" />
    <div class="px-32 py-24">
      <EditForm :id="productId!" ref="form" />
    </div>
    <template #footer>
      <DialogFooterBar @confirm="save" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
