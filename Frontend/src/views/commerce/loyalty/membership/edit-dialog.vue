<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { editMembership, getMembershipEdit } from "@/api/commerce/loyalty";
import type { MembershipEdit } from "@/api/commerce/loyalty";
import EditForm from "./edit-form.vue";
import { errorMessage } from "@/components/basic/message";

const { t } = useI18n();
const model = ref<MembershipEdit>();
const show = ref(true);
const props = defineProps<{ modelValue: boolean; id: string }>();
const form = ref();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

getMembershipEdit(props.id).then((rsp) => {
  model.value = rsp;
});

async function save() {
  if (model.value?.allowAutoUpgrade && !model.value.condition.items.length) {
    errorMessage(t("common.autoUpgradeMembershipNonCondition"));
    return;
  }
  await form.value.validate();
  await editMembership(model.value);
  show.value = false;
  emit("reload");
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('common.editMembership')"
    :close-on-click-modal="false"
    @closed="$emit('update:modelValue', false)"
  >
    <EditForm v-if="model" ref="form" :model="model" />

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
