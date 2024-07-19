<script lang="ts">
import { getDefaultLanguage, i18n } from "@/modules/i18n";
import { useRoute } from "vue-router";
import Cookies from "universal-cookie";
import { useSiteStore } from "@/store/site";
import { copyText } from "@/hooks/use-copy-text";
import type { UpdateTabModel } from "../index.vue";
import { usePreviewUrl } from "@/hooks/use-preview-url";
const $t = i18n.global.t;

export const define = {
  name: "pages",
  icon: "icon-copy",
  display: $t("common.page"),
  route: "pages",
  order: 0,
  permission: "pages",
  actions: [
    {
      name: "refresh",
      display: $t("common.refresh"),
      icon: "icon-Refresh",
      visible: true,
      async invoke() {
        const activity = useDevModeStore().activeActivity;
        if (!activity) return;
        const instance = await activity.panelInstance.promise;
        instance.load();
      },
    },
    {
      name: "more",
      display: $t("common.more"),
      icon: "icon-more",
      visible: true,
      invoke() {
        router.push(useRouteSiteId({ name: "pages" }));
      },
    },
  ],
} as Activity;

export const open = (
  item: { id: string; name: string; type?: string; layoutId?: string },
  panel: Component
) => {
  if (item.type === "Designer") {
    const pageRoute = getDesignerPage(item.id, item.layoutId);
    openInNewTab(router.resolve(pageRoute).href);
    return;
  }
  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: panel,
    icon: "icon-copy",
    actions: [
      {
        name: "save",
        visible: false,
        display: $t("common.save"),
        icon: "icon-preservation",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.save();
        },
      },
      {
        name: "refresh",
        visible: true,
        display: $t("common.refresh"),
        icon: "icon-Refresh",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.load();
        },
      },
      {
        name: "open",
        visible: true,
        display: $t("common.preview"),
        icon: "icon-eyes",
        invoke: async () => {
          const store = usePageStore();
          let found = store.list.find((f) => f.id === item.id);
          if (!found) {
            await store.load();
            found = store.list.find((f) => f.id === item.id);
          }
          if (found) {
            const { onPreview } = usePreviewUrl();
            onPreview!(found.previewUrl);
          }
        },
      },
    ],
  });
};
</script>

<script lang="ts" setup>
import Collapse from "../components/collapse.vue";
import { usePageStore } from "@/store/page";
import FileItem from "../components/file-item.vue";
import type { Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import { toRichPost } from "@/api/pages/types";
import type { PostPage, PostRichPage } from "@/api/pages/types";
import PagePanel from "../tab-panels/page-panel.vue";
import RichPagePanel from "../tab-panels/rich-page-panel.vue";
import { emptyGuid } from "@/utils/guid";
import type { Component } from "vue";
import { ref, inject, computed } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import EditForm from "@/views/pages/normal-page/edit-form.vue";
import EditRichForm from "@/views/pages/rich-page/edit-form.vue";
import { useLayoutStore } from "@/store/layout";
import { useI18n } from "vue-i18n";
import { combineUrl, openInNewTab } from "@/utils/url";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import { getLayout } from "@/api/layout";
import { addonToXml, layoutToAddon } from "@/utils/page";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";
import { getDesignerPage } from "../../../utils/page";

const { t } = useI18n();
const route = useRoute();
const pageStore = usePageStore();
const layoutStore = useLayoutStore();
const showAddItemDialog = ref(false);
const form = ref();
const richForm = ref();
const model = ref<PostPage | PostRichPage>();
const activeAddLayout = ref(false);
let selectedType = ref("normal");
const oldUrl = ref();
const contextMenu = ref();
const cookies = new Cookies();
const siteStore = useSiteStore();

const onAdd = async () => {
  model.value = await pageStore.getPage();
  model.value.published = true;
  selectedType.value = "normal";
  showAddItemDialog.value = true;
};

const onAddLayout = async (id: string) => {
  model.value = await pageStore.getPage();
  model.value.published = true;
  const layout = await getLayout(id);
  const layoutAddon = layoutToAddon(layout.name, layout.body);
  model.value!.body = addonToXml(layoutAddon);
  selectedType.value = "layout";
  showAddItemDialog.value = true;
};

const onAddRich = async () => {
  selectedType.value = "rich";
  model.value = {
    id: emptyGuid,
    body: "",
    title: "",
    name: "",
    url: "",
    cacheByVersion: false,
    cacheMinutes: 3,
    enableCache: false,
    cacheQueryKeys: " ",
    published: true,
  };
  showAddItemDialog.value = true;
};

const onEdit = async (id: string, type: string) => {
  selectedType.value = type;
  model.value = await pageStore.getPage(id);
  oldUrl.value = model.value.urlPath;
  if (type === "rich") {
    model.value = toRichPost(model.value);
  }

  showAddItemDialog.value = true;
};

const onSave = async () => {
  if (selectedType.value === "rich") {
    await richForm.value.validate();
    const id = await pageStore.updateRichPage(model.value! as PostRichPage);
    await pageStore.load();
    const page = pageStore.list.find((f) => f.id == id);
    if (page) open(page, RichPagePanel);
    //更新tab 的 urlPath
    updateModel(page);
  } else {
    await form.value.validate();
    const page = await pageStore.updatePage(model.value! as PostPage);
    if (page.type !== "Designer") {
      open(page, PagePanel);
    }
    pageStore.load();
    //更新tab 的 urlPath
    updateModel(page);
  }
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await pageStore.deletePages([id]);
};

const load = () => {
  pageStore.load();
  layoutStore.load();
};

const getDesignUrl = (page: any) => {
  let path = router.resolve(
    useRouteSiteId({
      name: "inline-design",
      query: {
        path: page.path,
        access_token: cookies.get("jwt_token"),
        inline_design_lang: getDefaultLanguage(),
        scheme: localStorage.getItem("vueuse-color-scheme"),
      },
    })
  ).href;

  if (import.meta.env.PROD) {
    path = combineUrl(siteStore.site.baseUrl, path);
  }

  return path;
};

const activeDesigner = ref(false);
const actions = computed(() => {
  const list = [
    {
      show: true,
      name: t("common.copyTitle"),
      invoke: async (item: any) => {
        copyText(item.name);
      },
    },
    {
      show: true,
      name: t("common.preview"),
      invoke: async (item: any) => {
        const { onPreview } = usePreviewUrl();
        onPreview!(item.previewUrl);
      },
    },
  ];
  if (!activeDesigner.value) {
    list.push({
      show: true,
      name: t("common.settings"),
      invoke: async (item: any) => {
        openInNewTab(
          router.resolve(
            useRouteSiteId({
              name:
                item.type === "RichText"
                  ? "rich-page-edit"
                  : item.layoutId === emptyGuid
                  ? "page-setting"
                  : "layout-page-setting",
              query: {
                id: item.id,
                layoutId:
                  item.layoutId === emptyGuid ? undefined : item.layoutId,
              },
            })
          ).href
        );
      },
    });
  }
  list.push({
    show: siteStore.hasAccess("pages", "edit"),
    name: t("common.inlineEditor"),
    invoke: async (item: any) => {
      const { onPreview } = usePreviewUrl();
      onPreview!(getDesignUrl(item));
    },
  });
  return list;
});

function openContextMenu(e: Event, item: any) {
  activeDesigner.value = item.type === "Designer";
  contextMenu.value?.openMenu(e, item);
}

defineExpose({ load });
const updateModel = inject("updateTabModel") as UpdateTabModel;
</script>

<template>
  <Collapse
    ref="collapse"
    permission="pages"
    :title="t('common.normalPage')"
    add
    :expand-default="true"
    @on-add="onAdd"
  >
    <template v-for="item in pageStore.getNormalPage">
      <FileItem
        v-if="!item.children"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        :name="item.name"
        remove
        permission="pages"
        :padding="12 + item.floor * 6"
        @click="open(item, PagePanel)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="openContextMenu($event, item)"
      >
        <ElIcon
          v-hasPermission="{
            feature: route.query.activity,
            action: 'edit',
            effect: 'hiddenIcon',
          }"
          class="iconfont icon-a-writein hover:text-blue"
          @click="onEdit(item.id, 'normal')"
        />
      </FileItem>
      <ReSubMenu
        v-else
        ref="reSubMenu"
        :key="item.folderName + item.floor"
        :data="item"
        permission="pages"
        :show-edit="true"
        @click-handle="
          open($event, $event.type === 'RichText' ? RichPagePanel : PagePanel)
        "
        @remove="onRemove($event)"
        @edit="onEdit($event.id, 'normal')"
        @click-context-menu="openContextMenu($event.event, $event.item)"
      />
    </template>
  </Collapse>

  <Collapse
    :title="t('common.layoutPage')"
    :expand-default="true"
    permission="pages"
  >
    <template #bar>
      <ElDropdown
        v-if="layoutStore.list.length"
        trigger="click"
        @command="onAddLayout"
        @visible-change="activeAddLayout = $event"
      >
        <ElIcon
          v-hasPermission="{
            feature: route.query.activity,
            action: 'edit',
            effect: 'hiddenIcon',
          }"
          class="iconfont icon-a-addto opacity-0 group-hover:opacity-100 hover:text-blue"
          :class="activeAddLayout ? 'opacity-100' : ''"
        />
        <template #dropdown>
          <ElDropdownItem
            v-for="item of layoutStore.list"
            :key="item.id"
            :command="item.id"
          >
            {{ item.name }}
          </ElDropdownItem>
        </template>
      </ElDropdown>
    </template>
    <template v-for="item in pageStore.getLayoutPage">
      <FileItem
        v-if="!item.children"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        :name="item.name"
        remove
        permission="pages"
        :padding="12 + item.floor * 6"
        @click="open(item, PagePanel)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="openContextMenu($event, item)"
      >
        <ElIcon
          v-hasPermission="{
            feature: route.query.activity,
            action: 'edit',
            effect: 'hiddenIcon',
          }"
          class="iconfont icon-a-writein hover:text-blue"
          @click="onEdit(item.id, 'layout')"
        />
      </FileItem>
      <ReSubMenu
        v-else
        :key="item.folderName + item.floor"
        :data="item"
        permission="pages"
        :show-edit="true"
        @click-handle="
          open($event, $event.type === 'RichText' ? RichPagePanel : PagePanel)
        "
        @remove="onRemove($event)"
        @edit="onEdit($event.id, 'layout')"
        @click-context-menu="openContextMenu($event.event, $event.item)"
      />
    </template>
  </Collapse>
  <Collapse
    :title="t('common.richTextPage')"
    add
    :expand-default="true"
    permission="pages"
    @on-add="onAddRich"
  >
    <template v-for="item in pageStore.getRichPage">
      <FileItem
        v-if="!item.children"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        :name="item.name"
        remove
        permission="pages"
        :padding="12 + item.floor * 6"
        @click="open(item, RichPagePanel)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="openContextMenu($event, item)"
      >
        <ElIcon
          v-hasPermission="{
            feature: route.query.activity,
            action: 'edit',
            effect: 'hiddenIcon',
          }"
          class="iconfont icon-a-writein hover:text-blue"
          @click="onEdit(item.id, 'rich')"
        />
      </FileItem>
      <ReSubMenu
        v-else
        :key="item.folderName + item.floor"
        :data="item"
        permission="pages"
        :show-edit="true"
        @click-handle="
          open($event, $event.type === 'RichText' ? RichPagePanel : PagePanel)
        "
        @remove="onRemove($event)"
        @edit="onEdit($event.id, 'rich')"
        @click-context-menu="openContextMenu($event.event, $event.item)"
      />
    </template>
  </Collapse>
  <div @click.stop>
    <ElDialog
      v-model="showAddItemDialog"
      width="600px"
      :close-on-click-modal="false"
      :title="
        model?.id == emptyGuid ? t('common.newPage') : t('common.editPage')
      "
      destroy-on-close
    >
      <div v-if="model" @keydown.enter="onSave">
        <EditRichForm
          v-if="selectedType === 'rich'"
          ref="richForm"
          :old-url="oldUrl"
          label-position="top"
          :model="(model as PostRichPage)"
        />
        <EditForm
          v-else
          ref="form"
          :old-url-path="oldUrl"
          label-position="top"
          :model="(model as PostPage)"
        />
      </div>
      <template #footer>
        <DialogFooterBar
          @confirm="onSave"
          @cancel="showAddItemDialog = false"
        />
      </template>
    </ElDialog>
  </div>
  <ContextMenu
    ref="contextMenu"
    :actions="actions.filter((f) => f.show === true)"
  />
</template>
