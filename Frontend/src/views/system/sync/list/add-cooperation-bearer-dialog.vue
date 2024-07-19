<script setup lang="ts">
import { ref } from "vue";
import type { ElForm } from "element-plus";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import { getRoles } from "@/api/site/user";
import { getCooperationBearer } from "@/api/publish";
import { openInAnchorTag } from "@/utils/url";
import { useSiteStore } from "@/store/site";

interface PropsType {
  modelValue: boolean;
}
interface EmitsType {
  (e: "update:model-value", value: boolean): void;
  (e: "create-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const siteStore = useSiteStore();

const form = ref<InstanceType<typeof ElForm>>();
let model = ref({
  role: "",
  expire: new Date(Date.now() + 3600 * 1000 * 24 * 365),
});
const roles = ref<string[]>([]);
const show = ref(true);

getRoles().then((rsp) => {
  roles.value = rsp;
  if (roles.value.length) model.value.role = roles.value[0];
});

const handleCreate = async () => {
  await form.value?.validate();
  const token = await getCooperationBearer(
    model.value.role,
    model.value.expire,
    siteStore.site.id
  );

  openInAnchorTag(
    "data:application/octet-stream;charset=utf-8," + encodeURIComponent(token),
    "bearer.json"
  );
  show.value = false;
  emits("create-success");
};
</script>

<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.newCooperationBearer')"
    @closed="$emit('update:model-value', false)"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      label-position="top"
      @submit.prevent
    >
      <el-form-item :label="t('common.role')" prop="role" required>
        <el-select v-model="model.role" class="w-full" data-cy="roles">
          <el-option
            v-for="item of roles"
            :key="item"
            :value="item"
            :label="item"
            data-cy="role-opt"
          />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('common.expireDate')" prop="expire" required>
        <ElDatePicker v-model="model.expire" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.create')"
        @confirm="handleCreate"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
