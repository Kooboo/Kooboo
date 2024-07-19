<script lang="ts" setup>
import { SECURITY_CODE_SEND_TIME } from "@/constants/constants";
import {
  useLocalStorage,
  StorageSerializers,
  refDefault,
  useIntervalFn,
} from "@vueuse/core";
import { ref, watch, toRef } from "vue";

import { useI18n } from "vue-i18n";

interface Props {
  disabled?: boolean;
  /** unit: seconds */
  duration?: number;
}

interface Emits {
  (e: "click", payload: MouseEvent): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const duration = refDefault(toRef(props, "duration"), 60);

const { t } = useI18n();

const startTime = useLocalStorage<number | null>(
  SECURITY_CODE_SEND_TIME,
  null,
  {
    serializer: StorageSerializers.number,
  }
);

const getTimeRemaining = () => {
  if (typeof startTime.value !== "number") return 0;
  return Math.max(
    Math.ceil(duration.value! - (Date.now() - startTime.value) / 1000),
    0
  );
};

const timeRemaining = ref(getTimeRemaining());
const countingDown = ref(timeRemaining.value > 0);
const { pause, resume } = useIntervalFn(
  () => void (timeRemaining.value = getTimeRemaining()),
  1000,
  { immediate: countingDown.value, immediateCallback: true }
);

const beginCountdown = () => {
  countingDown.value = true;
  startTime.value = Date.now();
  resume();
};

watch(timeRemaining, (timeRemaining) => {
  if (timeRemaining <= 0) {
    pause();
    countingDown.value = false;
  }
});

const onClick = (e: MouseEvent) => {
  beginCountdown();
  emit("click", e);
};

defineExpose({
  beginCountdown,
  timeRemaining,
});
</script>

<template>
  <el-button
    :disabled="disabled || countingDown || timeRemaining > 0"
    round
    v-bind="$attrs"
    @click="onClick"
  >
    {{
      countingDown || timeRemaining > 0
        ? t("common.waitSeconds", { time: timeRemaining })
        : t("common.sendVerificationCode")
    }}
  </el-button>
</template>
