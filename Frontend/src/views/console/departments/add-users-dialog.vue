<template>
  <el-dialog
    v-model="show"
    width="600px"
    :title="t('common.addUser')"
    @closed="emit('update:modelValue', false)"
  >
    <ElForm label-position="top">
      <ElFormItem :label="t('common.user')">
        <ElSelect v-model="model.userName" class="w-full">
          <ElOption
            v-for="item in filteredList"
            :key="item.id"
            :label="item.userName"
            :value="item.userName"
          />
        </ElSelect>
      </ElFormItem>

      <ElFormItem :label="t('common.function')">
        <ElInput v-model="model.function" />
      </ElFormItem>
      <ElFormItem :label="t('common.isManager')">
        <ElSwitch v-model="model.isManager" />
      </ElFormItem>
    </ElForm>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.add')"
        :disabled="!model.userName"
        @confirm="handleAdd"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { UsersList } from "@/api/organization/types";
import { getUsers } from "@/api/organization";
import { addUser } from "@/api/organization/department";
import { useAppStore } from "@/store/app";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  id: string;
  excludes: string[];
}>();
const userList = ref<UsersList[]>([]);
const appStore = useAppStore();
const model = ref({
  userName: "",
  isManager: false,
  function: "",
  departmentId: props.id,
});

async function load() {
  userList.value = await getUsers(appStore.currentOrg?.id);
}

load();

const filteredList = computed(() => {
  let result = userList.value;

  if (props.excludes) {
    result = result.filter((f) => !props.excludes!.includes(f.id));
  }

  return result;
});

const show = ref(true);

const handleAdd = async () => {
  await addUser(model.value);
  show.value = false;
  emit("reload");
};
</script>
