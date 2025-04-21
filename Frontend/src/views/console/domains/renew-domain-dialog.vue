<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { renewDomainOrder } from "@/api/market-order";
import { useRouter } from "vue-router";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; domain: string }>();
const { t } = useI18n();
const router = useRouter();
const show = ref(true);
const form = ref();
const model = ref({
  domainName: props.domain,
  year: 1,
});

const yearList = ref<{ value: number; label: string }[]>([]);
for (var i = 1; i <= 10; i++) {
  yearList.value.push({
    value: i,
    label: i + " " + (i === 1 ? t("common.year") : t("common.years")),
  });
}

const onSave = async () => {
  const orderId = await renewDomainOrder({
    domain: {
      domainName: model.value.domainName,
      year: model.value.year,
    },
  });
  router.push({
    name: "checkOrder",
    query: {
      orderId: orderId as string,
    },
  });
};
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.createDomainRenewOrder')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      @keydown.enter="onSave"
    >
      <el-form-item :label="t('common.domain')">
        <el-input v-model="model.domainName" disabled />
      </el-form-item>
      <el-form-item
        :label="`${t('common.period')} (${t('common.year')})`"
        prop="year"
      >
        <el-select v-model="model.year" class="w-30" default-first-option>
          <el-option
            v-for="item in yearList"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.ok')"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
