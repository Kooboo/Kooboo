<script lang="ts" setup>
import { ref, onMounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useEmailStore } from "@/store/email";
import { MemberDelete, memberList, memberPost } from "@/api/mail";
import { emailRule, rangeRule, requiredRule } from "@/utils/validate";
import { showConfirm } from "@/components/basic/confirm";

interface PropsType {
  group: any;
  modelValue: boolean;
}
const props = defineProps<PropsType>();

const { t } = useI18n();
const show = ref(true);
const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
const emailStore = useEmailStore();
const addressList = ref<string[]>();
const showAddAddress = ref(false);
const form = ref();
const model = ref({
  id: "",
  address: "",
});
const rules = {
  address: [
    requiredRule(t("common.inputEmailTips")),
    emailRule,
    rangeRule(1, 150),
  ],
};
const load = async () => {
  addressList.value = await memberList(model.value.id);
};
const handleAdd = () => {
  showAddAddress.value = true;
};

const onAdd = async () => {
  await form.value.validate();
  await memberPost(model.value.id, model.value.address);
  emailStore.loadAddress("addresses");
  showAddAddress.value = false;
  load();
};
const onDelete = async (item: string) => {
  await showConfirm(t("common.removeMemberTips"));
  await MemberDelete(model.value.id, item);
  emailStore.loadAddress("addresses");
  load();
};

onMounted(async () => {
  model.value.id = props.group.id;
  load();
});

watch(
  () => showAddAddress.value,
  () => {
    model.value.address = "";
  }
);
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.groupMembers')"
    @closed="emits('update:modelValue', false)"
  >
    <div>
      <IconButton
        v-if="!showAddAddress"
        circle
        class="text-blue mb-8"
        icon="icon-a-addto"
        :tip="t('common.add')"
        @click="handleAdd"
      />
    </div>
    <div>
      <span>{{ t("common.address") }}</span>
      <div class="w-full h-2px bg-gray dark:bg-444 my-4" />
    </div>
    <ul>
      <li
        v-for="(item, index) of addressList"
        :key="index"
        class="bg-[#f8f8f8] dark:bg-444"
      >
        <div
          class="flex justify-between items-center px-12 h-40px border-b border-gray border-solid dark:border-[#555]"
        >
          <span class="ellipsis mr-16" :title="item">{{ item }}</span>
          <el-button round type="danger" size="small" @click="onDelete(item)">{{
            t("common.remove")
          }}</el-button>
        </div>
      </li>
    </ul>
    <el-form
      v-if="showAddAddress"
      ref="form"
      :model="model"
      :rules="rules"
      @keydown.enter="onAdd"
      @submit.prevent
    >
      <el-form-item prop="address">
        <div>
          <div class="flex items-center my-4">
            <el-input v-model="model.address" class="w-350px" />
            <div class="ml-20px">
              <el-button round type="primary" class="!h-30px" @click="onAdd">{{
                t("common.save")
              }}</el-button>
            </div>
            <div class="ml-12">
              <el-button
                round
                class="!h-30px"
                @click="showAddAddress = false"
                >{{ t("common.cancel") }}</el-button
              >
            </div>
          </div>
        </div>
      </el-form-item>
    </el-form>
  </el-dialog>
</template>
