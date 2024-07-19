<template>
  <el-dialog
    :model-value="show"
    :title="t('common.changeCurrency')"
    width="400px"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top">
      <el-form-item prop="phone" :label="t('common.currency')">
        <el-select v-model="model.currency" class="w-full" data-cy="servers">
          <el-option
            v-for="item of currencyList"
            :key="item"
            :label="item"
            :value="item"
            data-cy="server-opt"
          />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar @confirm="change" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref, inject } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { changeCurrencyApi, getAvailableCurrency } from "@/api/user";
import type { Load } from "../profile.vue";

const props = defineProps<{
  modelValue: boolean;
  currency: string;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const model = ref({
  currency: "",
});
const currencyList = ref();

const load = async () => {
  currencyList.value = await getAvailableCurrency();
  model.value.currency = JSON.parse(JSON.stringify(props.currency));
};

const change = async () => {
  currencyList.value = await changeCurrencyApi(model.value.currency);
  reloadUser();
  show.value = false;
};
load();
const reloadUser = inject("reloadUser") as Load;
</script>
