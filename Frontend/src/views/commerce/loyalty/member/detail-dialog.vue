<script lang="ts" setup>
import { type Member, getMember } from "@/api/commerce/loyalty";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import PointsList from "./points-list.vue";
import MembershipsList from "./memberships-list.vue";
import { useTime } from "@/hooks/use-date";
import MembershipStatus from "./membership-status.vue";

const props = defineProps<{
  modelValue: boolean;
  id: string;
}>();

const show = ref(true);
const { t } = useI18n();
const activeTab = ref("membership");
const member = ref<Member>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

async function load() {
  member.value = await getMember(props.id);
}

load();
</script>

<template>
  <el-dialog
    :model-value="show"
    width="900px"
    :title="t('common.detail')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div v-if="member">
      <el-descriptions>
        <el-descriptions-item :label="t('common.contact')">
          {{ member.firstName }}
          {{ member.lastName }}</el-descriptions-item
        >
        <el-descriptions-item :label="t('common.email')">{{
          member.email
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.phone')">{{
          member.phone
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.membership')">
          <MembershipStatus :status="member.status" />
        </el-descriptions-item>
        <el-descriptions-item
          v-if="member.status?.startedAt"
          :label="t('common.startAt')"
        >
          {{ useTime(member.status.startedAt + "Z") }}
        </el-descriptions-item>
        <el-descriptions-item
          v-if="member.status?.endAt"
          :label="t('common.endAt')"
        >
          {{ useTime(member.status.endAt + "Z") }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('commerce.points')">
          <ElTag round>{{ member.points }}</ElTag>
        </el-descriptions-item>
      </el-descriptions>
      <el-tabs v-model="activeTab">
        <el-tab-pane name="membership" :label="t('common.membershipDetail')">
          <MembershipsList
            :member-id="member.id"
            :membership-id="member.status?.membership?.id"
            @reload="
              load();
              $emit('reload');
            "
          />
        </el-tab-pane>
        <el-tab-pane name="points" :label="t('commerce.pointsDetail')">
          <PointsList
            :member-id="member.id"
            :max-points="member.points"
            @reload="
              load();
              $emit('reload');
            "
          />
        </el-tab-pane>
      </el-tabs>
    </div>
  </el-dialog>
</template>

<style scoped>
:deep(.el-tabs__content) {
  @apply !px-0 !py-12;
}
:deep(.el-tabs__header) {
  @apply !px-0 !bg-transparent;
}
</style>
