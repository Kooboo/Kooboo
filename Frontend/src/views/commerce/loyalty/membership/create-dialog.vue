<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { createMembership } from "@/api/commerce/loyalty";
import type { MembershipCreate } from "@/api/commerce/loyalty";
import EditForm from "./edit-form.vue";
import { errorMessage } from "@/components/basic/message";

const { t } = useI18n();
const show = ref(true);
const form = ref();
defineProps<{ modelValue: boolean }>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const model = ref<MembershipCreate>({
  name: "",
  description: "",
  duration: 1,
  durationUnit: "Year",
  price: 10,
  priority: 0,
  condition: {
    isAny: false,
    items: [],
  },
  allowPurchase: false,
  allowAutoUpgrade: false,
});

async function save() {
  if (model.value?.allowAutoUpgrade && !model.value.condition.items.length) {
    errorMessage(t("common.autoUpgradeMembershipNonCondition"));
    return;
  }
  await form.value.validate();
  await createMembership(model.value);
  show.value = false;
  emit("reload");
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('common.createMembership')"
    :close-on-click-modal="false"
    @closed="$emit('update:modelValue', false)"
  >
    <EditForm ref="form" :model="model" />

    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'loyalty',
          action: 'edit',
        }"
        @confirm="save"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
