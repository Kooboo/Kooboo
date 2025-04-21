<template>
  <el-dialog
    :model-value="show"
    :title="t('common.preferred2FAMethod')"
    width="400px"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top">
      <el-form-item prop="method">
        <ElRadioGroup v-model="method" class="block space-y-8">
          <div
            v-for="item of availableMethods"
            :key="item.key"
            class="flex flex-col"
          >
            <ElRadio :label="item.key">
              <span>{{ item.value }} </span>
              <span v-if="item.key == 'email'"> ({{ email }})</span>
              <span v-if="item.key == 'tel'"> ({{ tel }})</span>
            </ElRadio>
            <div
              class="pl-24 w-full text-s text-999"
              style="word-break: normal"
            >
              {{ item.description }}
            </div>
          </div>
        </ElRadioGroup>
      </el-form-item>
      <el-form-item v-if="method == 'otp'">
        <template #label>
          <span class="text-s">{{ t("common.addOtpCodeTip") }}</span>
        </template>
        <div
          v-if="optUri"
          class="w-[400px] flex flex-col items-center justify-center"
        >
          <VueQr :text="optUri" :size="200" class="transform" />
          <el-button
            size="small"
            type="primary"
            round
            @click="generateNewQRCode"
          >
            {{ t("common.generateNewQRCode") }}
          </el-button>
        </div>
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
    if (method.value == "otp") {
      optUri.value = await getOptUri(false);
    }
  },
  {
    immediate: true,
  }
);

async function generateNewQRCode() {
  optUri.value = await getOptUri(true);
}

const reloadUser = inject("reloadUser") as Load;
</script>
<style scoped>
:deep(.el-dialog__body) {
  word-break: normal;
}
</style>
