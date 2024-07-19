<template>
  <el-form-item
    :label="field.displayName ?? field.name"
    :prop="field.prop"
    :required="field.required"
  >
    <VueDraggable
      :list="list"
      item-key="id"
      display-prop="value"
      class="w-full"
      handle=".icon-move"
      :group="{
        name: 've-social-control',
        pull: 'clone',
        put: false,
      }"
      @delete="onDelete"
      @add="onAdd"
    >
      <template #item="{ element, index }">
        <div class="w-full mb-8">
          <div class="w-full flex justify-between items-center mb-4">
            <div>
              <img
                class="w-[32px] inline-block cursor-pointer"
                :src="element.previewIcon || element.icon"
                :alt="element.alternateText"
                width="32"
                :title="t('ve.src')"
                @click="onChooseImage(element)"
              />
              <span class="ml-2">{{ element.name }}</span>
            </div>
            <div class="space-x-8 flex items-center">
              <el-tag v-if="element.type === 'share'" size="small">
                {{ t("ve.share") }}
              </el-tag>
              <el-tag v-else-if="element.prefix === 'tel:'" size="small">
                {{ t("common.phone") }}
              </el-tag>
              <el-tag v-else-if="element.prefix === 'mailto:'" size="small">
                {{ t("common.email") }}
              </el-tag>
              <span>{{ t("ve.moreOptions") }}</span>
              <el-switch v-model="element.moreActions" />
              <div class="space-x-8 ml-8">
                <el-icon
                  class="text-orange iconfont icon-delete"
                  data-cy="remove"
                  @click="onDelete(index)"
                />
                <el-icon
                  class="iconfont icon-move cursor-move"
                  data-cy="move"
                />
              </div>
            </div>
          </div>
          <div v-if="element.moreActions">
            <el-input
              v-model="element.title"
              :placeholder="t('common.title')"
              class="mb-4"
            >
              <template #prepend>{{ t("common.title") }}</template>
            </el-input>
            <el-input
              v-model="element.alternateText"
              :placeholder="t('common.altText')"
              class="mb-4"
            >
              <template #prepend>{{ t("common.altText") }}</template>
            </el-input>
          </div>
          <el-input
            v-model="element.url"
            :placeholder="t('common.url')"
            class="mb-4"
          >
            <template #prepend>{{ t("common.url") }}</template>
          </el-input>
        </div>
      </template>
      <template #footer>
        <el-popover :visible="visible" :width="270" trigger="click">
          <template #reference>
            <el-button circle data-cy="add" @click.stop="visible = !visible">
              <el-icon class="text-blue iconfont icon-a-addto" />
            </el-button>
          </template>
          <div @click.stop>
            <el-radio-group
              v-model="activeGroup"
              class="el-radio-group--rounded"
            >
              <el-radio-button
                v-for="item of allTabs"
                :key="item.value"
                :label="item.value"
                :data-cy="item.value"
              >
                {{ item.key }}
              </el-radio-button>
            </el-radio-group>
          </div>
          <div class="flex items-start flex-wrap justify-start" @click.stop>
            <img
              v-for="item in socialItems"
              :key="item.id"
              class="w-[32px] inline-block cursor-pointer m-8"
              :src="item.icon"
              :alt="item.alternateText"
              width="32"
              :title="item.title"
              @click="onAdd(item)"
            />
          </div>
        </el-popover>
      </template>
    </VueDraggable>
    <KMediaDialog
      v-if="visibleMediaDialog"
      v-model="visibleMediaDialog"
      @choose="handleChooseFile"
    />
  </el-form-item>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import KMediaDialog, { type MediaFileItem } from "@/components/k-media-dialog";
import { isClassic } from "@/components/visual-editor/utils";
import type { KeyValue } from "@/global/types";
import { off, on } from "@/utils/dom";
import { newGuid } from "@/utils/guid";
import { cloneDeep } from "lodash-es";
import { computed, onBeforeUnmount, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import VueDraggable from "vuedraggable";
import {
  useSocialEffects,
  type SocialItem,
  type SocialType,
} from "../../ve-widgets/social";

const { socialGroups } = useSocialEffects();

const { t } = useI18n();
const props = defineProps<{
  model: Record<string, any>;
  field: Field;
}>();
const activeGroup = ref<SocialType>("follow");
const list = ref<SocialItem[]>([]);
const activeItem = ref<SocialItem>();
const visible = ref(false);
const socialItems = computed<SocialItem[]>(
  () => socialGroups.value[activeGroup.value]
);
const visibleMediaDialog = ref(false);
const allTabs: KeyValue[] = isClassic()
  ? [
      {
        key: t("ve.social"),
        value: "follow",
      },
      {
        key: t("ve.others"),
        value: "others",
      },
    ]
  : //primary editor
    [
      {
        key: t("ve.follow"),
        value: "follow",
      },
      {
        key: t("ve.share"),
        value: "share",
      },
      {
        key: t("ve.others"),
        value: "others",
      },
    ];

function init() {
  const json: string = props.model[props.field.name] || "[]";
  if (typeof json === "string") {
    list.value = JSON.parse(json);
  }
}
init();

watch(
  () => list.value,
  (data) => {
    props.model[props.field.name] = JSON.stringify(data);
  },
  {
    deep: true,
  }
);

function onAdd(item: SocialItem) {
  const data = cloneDeep(item);
  data.id = newGuid();
  list.value.push(data);
  visible.value = false;
}

function onDelete(index: number) {
  list.value.splice(index, 1);
}

function onChooseImage(item: SocialItem) {
  activeItem.value = item;
  visibleMediaDialog.value = true;
}

function handleChooseFile(files: MediaFileItem[]) {
  const file = files[0];
  if (!activeItem.value || !file) {
    return;
  }
  activeItem.value.icon = file.url;
  activeItem.value.previewIcon = file.previewUrl;
}

function closeDialog() {
  visible.value = false;
}

onMounted(() => {
  on(window, "click", closeDialog);
});
onBeforeUnmount(() => {
  off(window, "click", closeDialog);
});
</script>
