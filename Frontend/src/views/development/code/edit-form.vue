<script lang="ts" setup>
import { isUniqueName } from "@/api/code";
import type { PostCode } from "@/api/code/types";
import {
  isUniqueNameRule,
  simpleNameRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";
import { computed } from "@vue/reactivity";
import type { Rules } from "async-validator";
import { onMounted, ref, watch } from "vue";
import { getEventList } from "@/api/events";
import { useI18n } from "vue-i18n";
import type { EventTypeItem } from "@/api/events/types";
import { toName } from "@/utils/url";
import { useCodeStore } from "@/store/code";

const props = defineProps<{
  model: PostCode;
  editMode: boolean;
}>();

const codeStore = useCodeStore();
const { t } = useI18n();
const form = ref();

const rules = computed(
  () =>
    ({
      name: props.editMode
        ? []
        : [
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            simpleNameRule(),
            isUniqueNameRule(isUniqueName, t("common.codeNameExistsTips")),
          ],
      url: [requiredRule(t("common.urlRequiredTips"))],
    } as Rules)
);

const nameChanged = (value: string) => {
  if (props.model && !props.model.url) {
    props.model.url = `/${value}`;
  }
};
const groupEvents = ref<{ category: string; events: EventTypeItem[] }[]>();

onMounted(async () => {
  if (props.model.codeType?.toLocaleLowerCase() === "event") {
    const events = await getEventList();
    groupEvents.value = [...new Set(events.map((item) => item.category))].map(
      (category) => ({
        category,
        events: events.filter((e) => e.category === category),
      })
    );
  }
});

const nameEdited = ref(false);

watch(
  () => props.model.url,
  (url) => {
    if (nameEdited.value || props.editMode) return;
    props.model.name = toName(url);
  }
);

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form
    v-if="model"
    ref="form"
    :model="model"
    :rules="rules"
    @submit.prevent
  >
    <el-form-item :label="t('common.codeName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="editMode"
        :title="model.name"
        data-cy="code-name"
        @change="nameChanged"
        @input="nameEdited = true"
      />
    </el-form-item>
    <el-form-item
      v-if="model.codeType?.toLocaleLowerCase() === 'api'"
      label="URL"
      prop="url"
    >
      <el-input v-model="model.url" class="min-w-300px" data-cy="url" />
    </el-form-item>
    <el-form-item v-if="groupEvents">
      <el-select
        v-model="model.eventType"
        class="min-w-300px w-full"
        :disabled="editMode"
        default-first-option
      >
        <el-option-group
          v-for="group in groupEvents"
          :key="group.category"
          :label="group.category"
        >
          <el-option
            v-for="item in group.events"
            :key="item.name"
            :label="item.name"
            :value="item.name"
          />
        </el-option-group>
      </el-select>
    </el-form-item>

    <el-form-item
      v-if="
        !(model.codeType == 'PageScript' && model.isEmbedded) &&
        model.codeType != 'CodeBlock'
      "
      :label="t('common.scriptType')"
    >
      <el-radio-group
        v-model="model.scriptType"
        class="el-radio-group--rounded"
      >
        <el-radio-button
          v-for="item of codeStore.scriptTypes"
          :key="item.key"
          :label="item.key"
          :data-cy="item.key"
          >{{ item.value }}</el-radio-button
        >
      </el-radio-group>
    </el-form-item>
  </el-form>
</template>
