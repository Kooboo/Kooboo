import { ref } from "vue";
import type { Position } from "../types";
import PageEditor from "./page-editor.vue";
import { computed } from "@vue/reactivity";
import DomBorder from "./dom-border.vue";
import { i18n } from "@/modules/i18n";
import { usePersistent } from "@/store/persistent";
import { getLinks } from "@/api/url";

const { t } = i18n.global;
const persistent = usePersistent();
const pageLinks = ref<string[]>([]);
getLinks().then((rsp) => (pageLinks.value = rsp.pages.map((m) => m.url)));

const pageWidths = [
  {
    name: "full",
    size: undefined,
    display: t("common.fullScreen"),
  },
  {
    name: "pad",
    size: 820,
    display: t("common.pad"),
  },
  {
    name: "phone",
    size: 390,
    display: t("inlineDesign.phone"),
  },
];

const selectionChangeEvent = ref<Event>();
const currentElement = ref<HTMLElement>();
const hoverElement = ref<HTMLElement>();
const clickPosition = ref<Position>();
const culture = ref<string>("en");
const frame = ref();
const leaveTip = ref<string>();

const getWidth = (name: string | null) =>
  pageWidths.find((f) => f.name == name) ?? pageWidths[0];

const width = ref(getWidth(persistent.inlineDesignWidth.value));

const switchWidth = (name: string) => {
  width.value = getWidth(name);
  persistent.inlineDesignWidth.value = width.value.name;
};

const offset = computed(() => {
  if (!frame.value || width.value == pageWidths[0]) return { x: 0, y: 0 };
  const rect = frame.value.element.getBoundingClientRect();

  return {
    x: rect.x,
    y: rect.y,
  };
});

const doc = computed(() => frame.value?.element?.contentDocument as Document);
const win = computed(() => frame.value?.element?.contentWindow as Window);

export {
  doc,
  win,
  selectionChangeEvent,
  currentElement,
  hoverElement,
  clickPosition,
  culture,
  width,
  offset,
  frame,
  PageEditor,
  DomBorder,
  switchWidth,
  pageWidths,
  pageLinks,
  leaveTip,
};
