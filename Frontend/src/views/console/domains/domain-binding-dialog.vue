<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.newBinding')"
    @close="handleClose"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
    >
      <el-form-item prop="subDomain" :label="t('common.domain')">
        <el-input
          v-model="model.subDomain"
          placeholder="www"
          data-cy="subdomain"
        >
          <template #append>
            <span
              disabled
              class="ellipsis max-w-240px px-16 dark:bg-[#222] dark:rounded-tr-normal dark:rounded-br-normal dark:border-[#4c4d4f] dark:border-1 border-solid dark:border-l-0"
              :title="model.rootDomain"
              data-cy="root-domain"
              >.{{ model.rootDomain }}
            </span></template
          >
        </el-input>
      </el-form-item>
      <el-form-item prop="SiteId" :label="t('common.site')">
        <el-select v-model="model.SiteId" class="w-full">
          <el-option
            v-for="item in siteList"
            :key="item.siteId"
            :label="item.siteDisplayName"
            :value="item.siteId"
            data-cy="site-opt"
          />
        </el-select>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.create')"
        @confirm="handleCreate"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import type { ElForm } from "element-plus";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import useOperationDialog from "@/hooks/use-operation-dialog";
import {
  domainBindingIsUniqueNameRule,
  rangeRule,
  requiredRule,
  subDomainRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import type { Domain } from "@/api/console/types";
import { postBinding } from "@/api/console";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import type { SiteItem } from "@/api/site";
import { getSiteList } from "@/api/site";
import { getList } from "@/api/console";

interface PropsType {
  modelValue: boolean;
  domain: Domain;
}

interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "create-success"): void;
}

const route = useRoute();
const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const siteList = ref<SiteItem[]>();
const domainList = ref();

const form = ref<InstanceType<typeof ElForm>>();

const model = ref({
  subDomain: "",
  rootDomain: "",
  SiteId: "",
});

const rules = {
  subDomain: [
    subDomainRule,
    requiredRule(t("common.inputValue")),
    rangeRule(1, 63),
    domainBindingIsUniqueNameRule(model.value),
  ],
  SiteId: [requiredRule(t("common.inputValue"))],
} as Rules;

watch(
  () => visible.value,
  async (val) => {
    if (val) {
      siteList.value = await getSiteList();
      domainList.value = await getList();
      form.value?.resetFields();
      model.value.subDomain = "";
      model.value.rootDomain = domainList.value.filter(
        (i: any) => i.id === route.query.id
      )[0].domainName;
      model.value.SiteId = siteList.value![0]?.siteId;
    }
  }
);

function handleCreate() {
  form.value?.validate(async (valid) => {
    if (valid) {
      await postBinding(
        model.value.subDomain,
        model.value.rootDomain,
        model.value.SiteId
      );
      handleClose();
      emits("create-success");
    }
  });
}
</script>

<style scoped>
:deep(.el-input-group__append) {
  background-color: #f5f7fa;
}

.el-select :deep(.el-input__suffix) {
  padding: 0;
  border: none;
}
</style>
