<script lang="ts" setup>
import { getList } from "@/api/component";
import type { Component, TagObject } from "@/api/component/types";
import SortableList from "@/components/sortable-list/index.vue";
import type { SortEvent, Addon, Placeholder } from "@/global/types";
import { ref } from "vue";
import AddonDialog from "./addon-dialog.vue";
import { useI18n } from "vue-i18n";
import { layoutToAddon } from "@/utils/page";
import type { AddonSource } from "../source";
import { tryAddSource } from "../source";
import { computed } from "@vue/reactivity";

const props = defineProps<{ model: Addon; sources: AddonSource[] }>();
const { t } = useI18n();
const componentList = ref<Component[]>([]);
const component = ref<Component>();
const placeholder = ref<Placeholder>();
getList().then((rsp) => (componentList.value = rsp));

function getSlot(addons: Addon[], addon: Addon) {
  if (addon.type !== "layout") return;
  addons.push(addon);

  for (const item of addon.content as Placeholder[]) {
    for (const i of item.addons) {
      getSlot(addons, i);
    }
  }

  return addons;
}

const slots = computed(() => {
  const addons: Addon[] = [];
  getSlot(addons, props.model);
  return addons;
});

const onAdd = async (tagObjects: TagObject[]) => {
  for (const tagObject of tagObjects) {
    const addon: Addon = {
      id: tagObject.name,
      type: component.value!.tagName.toLowerCase(),
      attributes: { id: tagObject.name },
    };

    const source = await tryAddSource(props.sources, addon);
    if (component.value?.requireEngine) {
      addon.attributes["engine"] = component.value.engineName;
    }

    if (component.value?.attribute) {
      var temp = document.createElement("div");
      temp.innerHTML = `<div ${component.value?.attribute} />`;
      temp = temp.children.item(0) as HTMLDivElement;

      for (const i of temp.getAttributeNames()) {
        addon.attributes[i] = temp.getAttribute(i)!;
      }
    }

    if (addon.type === "layout") {
      addon.content = layoutToAddon(addon.id, source.source.body).content;
    }

    placeholder.value!.addons.push(addon);
  }
};

const onDelete = (addon: Addon, placeholder: Placeholder) => {
  placeholder!.addons = placeholder!.addons.filter((f) => f !== addon);
  props.sources.splice(
    props.sources.findIndex((f) => f.id === addon.id && f.tag === addon.type),
    1
  );
};

const onSort = (e: SortEvent, placeholder: Placeholder) => {
  const item = placeholder.addons.splice(e.oldIndex, 1)[0];
  placeholder.addons.splice(e.newIndex, 0, item);
};

const showDialog = ref(false);

const onShowDialog = (
  currentComponent: Component,
  currentPlaceholder: Placeholder
) => {
  component.value = currentComponent;
  placeholder.value = currentPlaceholder;
  showDialog.value = true;
};
</script>

<template v-if="componentList.length">
  <template v-for="addonSlot of slots" :key="addonSlot.id">
    <el-collapse
      :model-value="(addonSlot.content as Placeholder[]).map((m) => m.name)"
    >
      <el-collapse-item
        v-for="i of addonSlot.content as Placeholder[]"
        :key="i.name"
        :name="i.name"
        :title="
          addonSlot == model
            ? t('common.positionName', { name: i.name })
            : t('common.positionNameAndFrom', {
                name: i.name,
                from: addonSlot.id,
              })
        "
      >
        <SortableList
          :list="i.addons"
          id-prop="id"
          display-prop="id"
          @delete="(id, addon) => onDelete(addon, i)"
          @sort="onSort($event, i)"
        >
          <template #right="{ item }">
            <el-tag
              size="small"
              class="rounded-full mr-8"
              data-cy="content-tag"
              >{{ item.type }}</el-tag
            >
          </template>
          <template #bottom>
            <el-dropdown trigger="click" @command="onShowDialog($event, i)">
              <el-button circle>
                <el-icon
                  class="text-blue iconfont icon-a-addto"
                  data-cy="add"
                />
              </el-button>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item
                    v-for="item of componentList"
                    :key="item.tagName"
                    :command="item"
                    :data-cy="item.tagName"
                  >
                    <span>{{ item.displayName }}</span>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </template>
        </SortableList>
      </el-collapse-item>
    </el-collapse>
  </template>

  <AddonDialog
    v-if="showDialog"
    v-model="showDialog"
    :model="component!"
    :sources="sources"
    @selected="onAdd"
  />
</template>
