<script lang="ts" setup>
import { computed, ref } from "vue";
import type { Menu } from "@/api/menu/types";
import { updateTemplate } from "@/api/menu";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import { emptyGuid } from "@/utils/guid";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{ modelValue: boolean; menu: Menu }>();
const { t } = useI18n();
const editor = ref();

const editorOptions = {
  lineNumbersMinChars: 2,
  minimap: { enabled: false },
  renderLineHighlight: "none",
};

const marks = {
  AnchorText: "{anchortext}",
  Href: "{href}",
  SubItems: "{items}",
  ActiveClass: "{activeclass:className}",
  ParentId: "{parentid}",
  CurrentId: "{currentid}",
};

const show = ref(true);

const model = ref({
  containerTop: props.menu.subItemContainer.split("{items}")[0],
  template: props.menu.subItemTemplate,
  containerBottom: props.menu.subItemContainer.split("{items}")[1],
});

const buildMenuHtml = (
  menu: Menu,
  containerTop: string,
  sunTemplate: string,
  containerBottom: string
) => {
  let result = containerTop;
  for (const i of menu.children) {
    let template = sunTemplate;
    if (template.indexOf(marks.Href)) {
      template = template.split(marks.Href).join(i.url ? i.url : "#");
    }

    if (template.indexOf(marks.AnchorText)) {
      template = template.split(marks.AnchorText).join(i.name);
    }

    if (template.indexOf(marks.CurrentId)) {
      template = template.split(marks.CurrentId).join(i.id);
    }

    if (template.indexOf(marks.ParentId)) {
      template = template.split(marks.ParentId).join(i.parentId);
    }

    if (template.indexOf("activeclass:")) {
      const className = (template.match(/{[\w\W]*?:([\w\W]*?)}/) ||
        [])[1]?.toString();
      if (className) {
        template = template
          .split(`{activeclass:${className}}`)
          .join(menu.children[0] === i ? `class="${className}"` : "");
      }
    }

    if (template.indexOf(marks.SubItems) && i.children?.length) {
      let subItems = buildMenuHtml(
        i,
        i.subItemContainer.split("{items}")[0],
        i.subItemTemplate,
        i.subItemContainer.split("{items}")[1]
      );

      template = template.split(marks.SubItems).join(subItems);
    }

    result += template;
  }
  result += containerBottom;
  return result;
};

const preview = computed(() => {
  let result = buildMenuHtml(
    props.menu,
    model.value.containerTop,
    model.value.template,
    model.value.containerBottom
  );
  return result;
});

const onInsertMark = (value: string) => {
  editor.value?.replace(value);
};

const onSave = async () => {
  await updateTemplate({
    Id: props.menu.id,
    RootId: props.menu.rootId === emptyGuid ? props.menu.id : props.menu.rootId,
    SubItemContainer:
      model.value.containerTop + "{items}" + model.value.containerBottom,
    SubItemTemplate: model.value.template,
  });
  show.value = false;
  emit("reload");
};
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog"
    width="1100px"
    :close-on-click-modal="false"
    :title="t('common.editTemplate')"
    @closed="emit('update:modelValue', false)"
  >
    <div class="flex space-x-8">
      <div class="space-y-8 flex-1">
        <div
          class="shadow-s-10 border border-solid border-line dark:border-444 rounded-normal overflow-hidden"
          data-cy="editor-wrapper"
        >
          <MonacoEditor
            v-model="model.containerTop"
            class="min-h-96px h-96px"
            language="html"
            :options="editorOptions"
          />
        </div>
        <div
          class="shadow-s-10 border border-solid border-line dark:border-444 rounded-normal overflow-hidden ml-32"
          data-cy="editor-wrapper"
        >
          <MonacoEditor
            ref="editor"
            v-model="model.template"
            class="min-h-96px h-173px"
            language="html"
            :options="editorOptions"
          />
          <div class="flex space-x-4 p-8">
            <el-tag
              v-for="(value, key) in marks"
              :key="key"
              size="small"
              class="rounded-full cursor-pointer"
              @click="onInsertMark(value)"
              >{{ key }}</el-tag
            >
          </div>
        </div>
        <div
          class="shadow-s-10 border border-solid border-line dark:border-444 rounded-normal overflow-hidden"
          data-cy="editor-wrapper"
        >
          <MonacoEditor
            v-model="model.containerBottom"
            language="html"
            class="min-h-96px h-96px"
            :options="editorOptions"
          />
        </div>
      </div>
      <div class="w-400px bg-card dark:bg-444 rounded-normal flex flex-col">
        <div class="p-8 font-bold">{{ t("common.preview") }}</div>
        <div class="h-full p-8">
          <pre class="overflow-auto h-374px" data-cy="preview">{{
            preview
          }}</pre>
        </div>
      </div>
    </div>
    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'menu',
          action: 'edit',
        }"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
