<script lang="ts" setup>
import { addScheduleApi, editScheduleApi } from "@/api/mail";
import { ref, watch, nextTick } from "vue";
import {
  useHourMinuteSecond,
  useDate,
  useHourMinute,
  useUtcLocalDate,
  useTime,
} from "@/hooks/use-date";
import { ElMessage } from "element-plus";

import { useI18n } from "vue-i18n";
import type { Schedule } from "@/api/mail/types";
import {
  rangeRule,
  requiredRule,
  toEmailRule,
  availableInviteEmailAddressRule,
} from "@/utils/validate";
import SelectInput from "../components/select-input/index.vue";
import { useEmailStore } from "@/store/email";
import { showConfirm } from "@/components/basic/confirm";
import type { Rules } from "async-validator";
import { getEmailAddress } from "@/utils/get-email-address";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  currentSchedule: Schedule;
}>();
const { t } = useI18n();

const show = ref(true);
const form = ref();
const showDateError = ref(false);
const emailStore = useEmailStore();

const model = ref<Schedule>({} as Schedule);
const rules = {
  calendarTitle: [
    requiredRule(t("common.valueRequiredTips")),
    rangeRule(1, 50),
  ],
  contact: [
    toEmailRule(),
    availableInviteEmailAddressRule(
      emailStore.addressList.map((m) => m.address)
    ),
  ],
} as Rules;
const initScheduleExceptContact = ref();
const initContact = ref();

const confirm = async () => {
  await form.value.validate();
  let start = useUtcLocalDate(
    useDate(model.value.startDate) +
      " " +
      useHourMinuteSecond(model.value.startTime)
  );
  let end = useUtcLocalDate(
    useDate(model.value.endDate) +
      " " +
      useHourMinuteSecond(model.value.endTime)
  );
  if (!emailStore.defaultAddress) {
    ElMessage.error(t("common.setDefaultEmailFirst"));
    return;
  }
  let unExpired = useTime(start) > useTime(new Date());
  if (model.value.id) {
    let addContact = [];
    let removeContact = [];
    let isChangeSchedule = false;
    isChangeSchedule =
      initScheduleExceptContact.value !== JSON.stringify(model.value, replacer);

    addContact = model.value.contact.filter((f: string) => {
      return !initContact.value.some(
        (s: string) => getEmailAddress(s) === getEmailAddress(f)
      );
    });

    removeContact = initContact.value.filter((f: string) => {
      return !model.value.contact.some(
        (s: string) => getEmailAddress(s) === getEmailAddress(f)
      );
    });

    //不同情况的不同消息提示
    if (isChangeSchedule && unExpired) {
      if (!addContact.length && !removeContact.length) {
        if (model.value.contact.length) {
          await showConfirm(
            t("common.anUpdatedEmailWillBeSentToTheParticipants")
          );
        }
      } else if (addContact.length && !removeContact.length) {
        await showConfirm(
          t("common.anEmailWillBeSentToExistentAndAddedParticipants")
        );
      } else if (!addContact.length && removeContact.length) {
        await showConfirm(
          t("common.anEmailWillBeSentToExistentAndRemovedParticipants")
        );
      } else {
        await showConfirm(
          t("common.anEmailWillBeSentToExistentAddedAndRemovedParticipants")
        );
      }
    } else if (unExpired) {
      if (!addContact.length && removeContact.length) {
        await showConfirm(
          t("common.aCancellationEmailWillBeSentToRemovedParticipants")
        );
      } else if (addContact.length && !removeContact.length) {
        await showConfirm(
          t("common.AnInvitationEmailWillBeSentToAddedParticipants")
        );
      } else if (addContact.length && removeContact.length) {
        await showConfirm(
          t("common.anEmailWillBeSentToAddedAndRemovedParticipants")
        );
      }
    }

    await editScheduleApi({
      id: model.value.id,
      calendarTitle: model.value.calendarTitle,
      start: start,
      end: end,
      mark: model.value.mark,
      location: model.value.location,
      contact: model.value.contact,
      isNotify: true,
      isOrganizer: model.value.isOrganizer,
      organizer: emailStore.defaultAddress,
      addContact: addContact,
      removeContact: removeContact,
    });
  } else {
    if (model.value.contact.length && unExpired) {
      await showConfirm(
        t("common.anInvitationEmailWillBeSentToTheParticipants")
      );
    }
    await addScheduleApi({
      organizer: emailStore.defaultAddress,
      calendarTitle: model.value.calendarTitle,
      start: start,
      end: end,
      mark: model.value.mark,
      location: model.value.location,
      contact: model.value.contact,
      isNotify: true,
    });
  }

  emit("reload");
  show.value = false;
};

function replacer(key: string, value: any) {
  if (key === "contact") {
    return undefined;
  }
  return value;
}

const load = () => {
  model.value = props.currentSchedule;
  initScheduleExceptContact.value = JSON.stringify(model.value, replacer);
  initContact.value = model.value.contact;
};
load();

watch(
  () => model.value,
  () => {
    let endTime = new Date(
      useDate(model.value.endDate) + " " + useHourMinute(model.value.endTime)
    ).getTime();
    let startTime = new Date(
      useDate(model.value.startDate) +
        " " +
        useHourMinute(model.value.startTime)
    ).getTime();
    if (endTime <= startTime) {
      showDateError.value = true;
    } else {
      showDateError.value = false;
    }
  },
  {
    deep: true,
  }
);

watch(
  () => model.value.contact,
  (n) => {
    if (n?.length) {
      nextTick().then(() => {
        document
          .querySelectorAll(".el-hover-title .el-tag__content")
          .forEach((i) => {
            i.setAttribute("title", (i as HTMLElement).innerText);
          });
      });
    }
  },
  {
    deep: true,
  }
);
</script>

<template>
  <el-dialog
    :model-value="show"
    width="620px"
    :close-on-click-modal="false"
    :title="model.id ? t('common.editSchedule') : t('common.newSchedule')"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="400px" wrap-class="pr-12">
      <el-form
        ref="form"
        label-position="top"
        :model="model"
        :rules="rules"
        @submit.prevent
      >
        <el-form-item :label="t('common.scheduleSubject')" prop="calendarTitle">
          <el-input v-model="model.calendarTitle" data-cy="calendarTitle" />
        </el-form-item>
        <el-form-item :label="t('common.participants')" prop="contact">
          <SelectInput
            v-model="model.contact"
            :input-value="model.contact"
            class="!w-full"
          />
        </el-form-item>

        <div class="flex items-center justify-between">
          <el-form-item :label="t('common.startTime')" prop="startTime">
            <div class="flex items-center space-x-16 w-full">
              <el-date-picker
                v-model="model.startDate"
                class="!w-140px"
                type="date"
                :clearable="false"
              />
              <el-time-picker
                v-model="model.startTime"
                format="HH:mm"
                class="!w-100px"
                :clearable="false"
              />
            </div>
          </el-form-item>
          <el-form-item :label="t('common.endTime')" prop="endTime">
            <div class="flex items-center space-x-16 w-full">
              <el-date-picker
                v-model="model.endDate"
                class="!w-140px"
                type="date"
                :clearable="false"
              />
              <el-time-picker
                v-model="model.endTime"
                class="!w-100px"
                format="HH:mm"
                :clearable="false"
              />
            </div>
          </el-form-item>
        </div>
        <div v-if="showDateError" class="text-[#F56C6C] text-12px -mt-12 mb-8">
          <el-icon class="iconfont icon-Tips" />
          {{ t("common.theEndTimeShouldBeLaterThanTheStartTime") }}
        </div>

        <el-form-item :label="t('common.location')" prop="location">
          <el-input v-model="model.location" data-cy="location" />
        </el-form-item>
        <el-form-item :label="t('common.description')" prop="mark">
          <KEditor
            v-model="model.mark"
            :hidden-code="true"
            :hidden-image="true"
            :min_height="150"
            :max_height="250"
          />
        </el-form-item>
      </el-form>
    </el-scrollbar>

    <template #footer>
      <DialogFooterBar
        :disabled="showDateError"
        @confirm="confirm"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>

<!-- select-input里的tag输入框超出省略号  -->
<style scoped>
:deep(.el-input__inner) {
  @apply ellipsis;
}

:deep(.el-select .el-select__tags-text) {
  display: block;
}

:deep(span.el-tag__content) {
  @apply max-w-300px;
}
</style>
