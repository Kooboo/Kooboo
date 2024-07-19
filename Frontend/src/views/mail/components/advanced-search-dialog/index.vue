<template>
  <el-dialog
    v-model="show"
    width="550px"
    :close-on-click-modal="false"
    :title="t('common.advancedSearchForEmail')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="left" label-width="110px">
      <el-form-item :label="t('common.keyword')">
        <el-input v-model="model.keyword" />
      </el-form-item>
      <el-form-item :label="t('common.position')">
        <el-select v-model="model.position" class="w-full">
          <el-option
            v-for="position in positionList"
            :key="position.name"
            :label="position.displayName"
            :value="position.name"
          />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('mail.from')">
        <el-input v-model="model.from" />
      </el-form-item>
      <el-form-item :label="t('mail.to')">
        <el-input v-model="model.to" />
      </el-form-item>
      <el-form-item prop="date" :label="t('common.date')">
        <el-select v-model="model.dateType" class="w-full">
          <el-option
            v-for="data in dateList"
            :key="data.value"
            :label="data.displayName"
            :value="data.value"
          />
        </el-select>
        <el-date-picker
          v-if="model.dateType === 6"
          v-model="dateValue"
          class="mt-12"
          value-format="YYYY-MM-DD"
          type="daterange"
          range-separator="To"
          :start-placeholder="t('common.startDate')"
          :end-placeholder="t('common.endDate')"
          @change="changeTime"
        />
      </el-form-item>
      <el-form-item prop="county" :label="t('common.folder')">
        <el-select
          v-model="model.searchFolder"
          class="w-full"
          :fit-input-width="true"
        >
          <template #default>
            <el-option
              v-for="folder in folderList"
              :key="folder.name"
              :label="folder.displayName"
              :value="folder.name"
            />
            <SelectSubDropDown
              v-for="item in emailStore.folderMenuTree"
              :key="item.name"
              :option-item="item"
              @select-folder="selectFolder($event)"
            />
          </template>
        </el-select>
      </el-form-item>

      <el-form-item :label="t('common.readOrUnread')">
        <el-select v-model="model.readOrUnread" class="w-full">
          <el-option
            v-for="item in readOrUnreadList"
            :key="item.value"
            :label="item.displayName"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.search')"
        @confirm="searchEmail"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { useEmailStore } from "@/store/email";
import type { DateModelType } from "element-plus";
import SelectSubDropDown from "./select-sub-drop-down.vue";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
interface PropsType {
  modelValue: boolean;
}
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "searchEmail"): void;
}>();

const props = defineProps<PropsType>();
const emailStore = useEmailStore();

const { t } = useI18n();
const show = ref(true);
const selectFolder = (folderName: string) => {
  model.value.searchFolder = folderName;
};

const model = ref(
  emailStore.searchModel || {
    keyword: emailStore.messageKeyword ?? "",
    position: "subject",
    from: "",
    to: "",
    dateType: 0,
    startDate: "",
    endDate: "",
    searchFolder: "allEmails",
    readOrUnread: -1,
  }
);
const dateValue = ref<[DateModelType, DateModelType]>([
  model.value.startDate,
  model.value.endDate,
]);

const changeTime = () => {
  if (
    dateValue.value &&
    dateValue.value.some((item: DateModelType) => item !== null)
  ) {
    model.value.startDate = dateValue.value[0].toString();
    model.value.endDate = dateValue.value[1].toString();
  } else {
    model.value.startDate = "";
    model.value.endDate = "";
  }
};

const positionList = ref([
  {
    name: "subject",
    displayName: t("common.subject"),
  },
  {
    name: "attachments",
    displayName: t("common.attachmentName"),
  },
  {
    name: "emailBody",
    displayName: t("common.emailBody"),
  },
  {
    name: "allContents",
    displayName: t("common.allContent"),
  },
]);

const dateList = ref([
  {
    value: 0,
    displayName: t("mail.any"),
  },
  {
    value: 1,
    displayName: t("mail.inOneDay"),
  },
  {
    value: 2,
    displayName: t("mail.inThreeDays"),
  },
  {
    value: 3,
    displayName: t("mail.inAWeek"),
  },
  {
    value: 4,
    displayName: t("mail.inTwoWeeks"),
  },
  {
    value: 5,
    displayName: t("mail.inAMonth"),
  },
  {
    value: 6,
    displayName: t("mail.customise"),
  },
]);

const readOrUnreadList = ref([
  {
    value: -1,
    displayName: t("mail.any"),
  },
  {
    value: 1,
    displayName: t("common.read"),
  },
  {
    value: 0,
    displayName: t("common.unread"),
  },
]);

const folderList = ref([
  {
    name: "allEmails",
    displayName: t("common.allEmails"),
  },
  {
    name: "inbox",
    displayName: t("common.inbox"),
  },
  {
    name: "sent",
    displayName: t("mail.sent"),
  },
  {
    name: "drafts",
    displayName: t("common.drafts"),
  },
  {
    name: "spam",
    displayName: t("common.spam"),
  },
  {
    name: "trash",
    displayName: t("common.trash"),
  },
]);

const searchEmail = async () => {
  model.value.keyword = model.value.keyword.trim();
  model.value.from = model.value.from.trim();
  model.value.to = model.value.to.trim();

  emailStore.searchModel = model.value;
  emit("update:modelValue", false);
  emit("searchEmail");
};
</script>
