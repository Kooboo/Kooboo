<template>
  <el-dialog
    :model-value="show"
    :title="t('common.2fa')"
    width="400px"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top">
      <el-form-item prop="method" :label="t('common.preferred2FAMethod')">
        <el-select v-model="method" class="w-full" data-cy="servers">
          <el-option
            v-for="item of availableMethods"
            :key="item.key"
            :label="item.value"
            :value="item.key"
          />
        </el-select>
      </el-form-item>
      <el-form-item
        v-if="method == 'otp' && optUri"
        :label="t('common.addOtpCodeTip')"
      >
        <VueQr :text="optUri" :size="400" class="transform" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="change" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref, inject, computed, watch } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { updateTwoFAMethod, getOptUri } from "@/api/user";
import type { Load } from "../profile.vue";
import { methods } from "../constants";
import VueQr from "vue-qr/src/packages/vue-qr.vue";

const props = defineProps<{
  modelValue: boolean;
  method: string;
  tel: string;
  email: string;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const method = ref(props.method || " ");
const optUri = ref();

const availableMethods = computed(() => {
  let result = [...methods];
  if (!props.tel) result = result.filter((f) => f.key != "tel");
  if (!props.email) result = result.filter((f) => f.key != "email");
  return result;
});

const change = async () => {
  await updateTwoFAMethod(method.value);
  reloadUser();
  show.value = false;
};

watch(
  () => method.value,
  async () => {
    console.log(method.value);
    if (method.value == "otp") {
      optUri.value = await getOptUri();
    }
  }
);

const reloadUser = inject("reloadUser") as Load;
</script>
