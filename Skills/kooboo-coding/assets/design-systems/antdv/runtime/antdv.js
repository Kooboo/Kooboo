// entry.js
export * from "/antdv.vendor.js";
import { Affix, Anchor, AnchorLink, AutoComplete, AutoCompleteOptGroup, AutoCompleteOption, Alert, Avatar, AvatarGroup, Badge, BadgeRibbon, Breadcrumb, BreadcrumbItem, BreadcrumbSeparator, Button as Button2, ButtonGroup, Calendar, Card, CardGrid, CardMeta, Collapse, CollapsePanel, Carousel, Cascader, Checkbox, CheckboxGroup, Col, Comment, ConfigProvider, DatePicker, MonthPicker, WeekPicker, RangePicker, QuarterPicker, Descriptions, DescriptionsItem, Divider, Dropdown, DropdownButton, Drawer as Drawer2, Empty, FloatButton, FloatButtonGroup, BackTop, Form, FormItem, FormItemRest, Input, InputGroup, InputPassword, InputSearch, Textarea, Image, ImagePreviewGroup, InputNumber, Layout, LayoutHeader, LayoutSider, LayoutFooter, LayoutContent, List, ListItem, ListItemMeta, Menu as Menu2, MenuDivider, MenuItem, MenuItemGroup, SubMenu, Mentions, MentionsOption, Modal, Statistic, StatisticCountdown, PageHeader, Pagination, Popconfirm, Popover, Progress, Radio, RadioButton, RadioGroup, Rate, Result, Row, Select, SelectOptGroup, SelectOption, Skeleton as Skeleton2, SkeletonButton, SkeletonAvatar, SkeletonInput, SkeletonImage, SkeletonTitle, Slider, Space, Compact, Spin, Steps, Step, Switch, Table, TableColumn, TableColumnGroup, TableSummary, TableSummaryRow, TableSummaryCell, Transfer, Tree, TreeNode, DirectoryTree, TreeSelect, TreeSelectNode, Tabs, TabPane, Tag, CheckableTag, TimePicker, TimeRangePicker, Timeline, TimelineItem, Tooltip, Typography, TypographyLink, TypographyParagraph, TypographyText, TypographyTitle, Upload, UploadDragger, LocaleProvider, Watermark, Segmented, QRCode, Tour, App, Flex } from "/antdv.vendor.js";

// src/shell.js
import { computed, defineComponent as defineComponent2, h as h2, onBeforeUnmount, onMounted, ref, watch } from "vue";
import { Button, Drawer, Menu, Skeleton, theme } from "/antdv.vendor.js";
import { loadLucide } from "/lucide.js";

// src/ant-icon.js
import { defineComponent, h } from "vue";
import { antIcons } from "/antdv.vendor.js";
var aliases = {
  "layout-grid": "AppstoreOutlined",
  home: "HomeOutlined",
  menu: "MenuOutlined",
  bell: "BellOutlined",
  user: "UserOutlined",
  "user-round": "UserOutlined",
  users: "TeamOutlined",
  settings: "SettingOutlined",
  "log-out": "LogoutOutlined",
  "chevron-down": "DownOutlined",
  "chevron-right": "RightOutlined",
  "chevron-left": "LeftOutlined",
  "refresh-cw": "ReloadOutlined",
  refresh: "ReloadOutlined",
  plus: "PlusOutlined",
  minus: "MinusOutlined",
  search: "SearchOutlined",
  pencil: "EditOutlined",
  edit: "EditOutlined",
  trash: "DeleteOutlined",
  "trash-2": "DeleteOutlined",
  download: "DownloadOutlined",
  upload: "UploadOutlined",
  check: "CheckOutlined",
  x: "CloseOutlined",
  info: "InfoCircleOutlined",
  "circle-help": "QuestionCircleOutlined",
  "circle-alert": "ExclamationCircleOutlined",
  package: "InboxOutlined",
  boxes: "AppstoreOutlined",
  "arrow-left-right": "SwapOutlined",
  "arrow-down-up": "SwapOutlined",
  "file-text": "FileTextOutlined",
  clipboard: "ProfileOutlined",
  "bar-chart-3": "BarChartOutlined",
  calendar: "CalendarOutlined",
  clock: "ClockCircleOutlined",
  filter: "FilterOutlined",
  "shopping-cart": "ShoppingCartOutlined",
  "credit-card": "CreditCardOutlined",
  heart: "HeartOutlined",
  star: "StarOutlined",
  eye: "EyeOutlined",
  "panel-left-close": "MenuFoldOutlined",
  "panel-left-open": "MenuUnfoldOutlined",
  sun: "BulbOutlined",
  moon: "BulbFilled"
};
function nativeName(name) {
  return aliases[name] || name.replace(/(^|-)([a-z])/g, (_all, _separator, letter) => letter.toUpperCase());
}
function hasAntIcon(name) {
  const resolved = nativeName(name);
  return /(Outlined|Filled|TwoTone)$/.test(resolved) && !!antIcons[resolved];
}
var KbAntIcon = defineComponent({
  name: "KbAntIcon",
  props: { name: { type: String, required: true }, size: { type: [String, Number], default: "1em" }, spin: Boolean, twoToneColor: [String, Array] },
  setup(props) {
    return () => {
      const resolved = nativeName(props.name);
      const icon = hasAntIcon(props.name) ? antIcons[resolved] : void 0;
      return h(icon || antIcons.QuestionCircleOutlined, {
        class: "kb-ant-native-icon",
        "aria-hidden": "true",
        "data-icon-name": resolved,
        "data-icon-missing": icon ? void 0 : props.name,
        style: { fontSize: typeof props.size === "number" ? props.size + "px" : props.size },
        spin: props.spin,
        twoToneColor: props.twoToneColor
      });
    };
  }
});

// src/shell.js
var KbLucideIcon = defineComponent2({
  name: "KbLucideIcon",
  props: { name: { type: String, required: true }, size: { type: [Number, String], default: "1em" } },
  setup(props) {
    const node = ref(null);
    let library, disposed = false;
    const update = () => {
      if (!disposed) node.value = library?.icons?.[props.name.replace(/(^|-)([a-z])/g, (_, a, b) => b.toUpperCase())] || null;
    };
    onMounted(async () => {
      try {
        library = await loadLucide();
        update();
      } catch {
      }
    });
    watch(() => props.name, update);
    onBeforeUnmount(() => {
      disposed = true;
    });
    const renderNode = ([tag, attrs, children]) => h2(tag, attrs, children?.map(renderNode));
    return () => h2("span", { class: "anticon kb-ant-icon", "aria-hidden": "true", style: { fontSize: typeof props.size === "number" ? props.size + "px" : props.size } }, [h2("svg", { viewBox: "0 0 24 24", fill: "none", stroke: "currentColor", "stroke-width": 2, "stroke-linecap": "round", "stroke-linejoin": "round", ...node.value?.[1], width: "1em", height: "1em", focusable: "false" }, node.value?.[2]?.map(renderNode))]);
  }
});
var KbAntShell = defineComponent2({
  name: "KbAntShell",
  props: { appName: { type: String, required: true }, navigation: { type: Array, default: () => [] }, navigationLoading: Boolean, activePath: { type: String, default: "" }, collapsed: Boolean, navigationLabel: { type: String, default: "\u4E3B\u5BFC\u822A" }, menuLabel: { type: String, default: "\u6253\u5F00\u5BFC\u822A" } },
  emits: ["navigate", "update:collapsed"],
  setup(props, { slots, emit, expose }) {
    const main = ref(null), mobile = ref(false), open = ref(false);
    let media;
    const { token } = theme.useToken();
    const sync = () => {
      mobile.value = media.matches;
      if (!media.matches) open.value = false;
    };
    onMounted(() => {
      media = window.matchMedia("(max-width: 767px)");
      sync();
      media.addEventListener("change", sync);
    });
    onBeforeUnmount(() => media?.removeEventListener("change", sync));
    watch(() => props.activePath, () => {
      open.value = false;
      if (main.value) main.value.scrollTop = 0;
    });
    const toggle = () => emit("update:collapsed", !props.collapsed);
    expose({ toggle, openNavigation: () => {
      open.value = true;
    } });
    const items = computed(() => {
      const result = [], groups = /* @__PURE__ */ new Map();
      for (const item of props.navigation) {
        const entry = { key: item.path, label: item.label, title: item.label, icon: item.icon ? () => h2(hasAntIcon(item.icon) ? KbAntIcon : KbLucideIcon, { name: item.icon }) : void 0 };
        if (item.group) {
          if (!groups.has(item.group)) {
            const group = { type: "group", key: "group:" + item.group, label: item.group, children: [] };
            groups.set(item.group, group);
            result.push(group);
          }
          groups.get(item.group).children.push(entry);
        } else result.push(entry);
      }
      return result;
    });
    const navigation = (small = false) => h2("nav", { "aria-label": props.navigationLabel }, props.navigationLoading ? h2("div", { class: "kb-ant-navigation-skeleton", role: "status", "aria-label": "\u6B63\u5728\u51C6\u5907\u5BFC\u822A", "aria-busy": "true" }, [h2(Skeleton, { active: true, title: false, paragraph: { rows: 7 }, "aria-hidden": "true" })]) : slots.navigation?.({ collapsed: !small && props.collapsed }) || h2(Menu, { mode: "inline", inlineCollapsed: !small && props.collapsed, selectedKeys: [props.activePath], items: items.value, onClick: ({ key }) => {
      open.value = false;
      emit("navigate", key);
    } }));
    return () => h2("div", { class: ["kb-ant-shell", { "is-collapsed": props.collapsed }], style: { fontFamily: token.value.fontFamily, fontSize: token.value.fontSize + "px", lineHeight: token.value.lineHeight, "--kb-ant-primary": token.value.colorPrimary, "--kb-ant-bg": token.value.colorBgLayout, "--kb-ant-panel": token.value.colorBgContainer, "--kb-ant-text": token.value.colorText, "--kb-ant-border": token.value.colorBorderSecondary } }, [
      h2("header", { class: "kb-ant-shell-header" }, [mobile.value ? h2(Button, { type: "text", "aria-label": props.menuLabel, "aria-expanded": open.value, onClick: () => {
        open.value = true;
      } }, { icon: () => h2(KbAntIcon, { name: "menu" }) }) : null, h2("strong", { class: "kb-ant-brand" }, props.appName), h2("div", { class: "kb-ant-actions" }, slots.actions?.({ collapsed: props.collapsed, toggle }))]),
      h2("div", { class: "kb-ant-shell-body" }, [!mobile.value ? h2("aside", { class: "kb-ant-shell-aside" }, navigation()) : null, h2("main", { ref: main, class: "kb-ant-shell-main", id: "app-main" }, slots.default?.())]),
      mobile.value ? h2(Drawer, { open: open.value, title: props.appName, placement: "left", width: 280, onClose: () => {
        open.value = false;
      } }, { default: () => navigation(true) }) : null
    ]);
  }
});
export {
  Affix as AAffix,
  Alert as AAlert,
  Anchor as AAnchor,
  AnchorLink as AAnchorLink,
  App as AApp,
  AutoComplete as AAutoComplete,
  AutoCompleteOptGroup as AAutoCompleteOptGroup,
  AutoCompleteOption as AAutoCompleteOption,
  Avatar as AAvatar,
  AvatarGroup as AAvatarGroup,
  BackTop as ABackTop,
  Badge as ABadge,
  BadgeRibbon as ABadgeRibbon,
  Breadcrumb as ABreadcrumb,
  BreadcrumbItem as ABreadcrumbItem,
  BreadcrumbSeparator as ABreadcrumbSeparator,
  Button2 as AButton,
  ButtonGroup as AButtonGroup,
  Calendar as ACalendar,
  Card as ACard,
  CardGrid as ACardGrid,
  CardMeta as ACardMeta,
  Carousel as ACarousel,
  Cascader as ACascader,
  CheckableTag as ACheckableTag,
  Checkbox as ACheckbox,
  CheckboxGroup as ACheckboxGroup,
  Col as ACol,
  Collapse as ACollapse,
  CollapsePanel as ACollapsePanel,
  Comment as AComment,
  Compact as ACompact,
  ConfigProvider as AConfigProvider,
  DatePicker as ADatePicker,
  Descriptions as ADescriptions,
  DescriptionsItem as ADescriptionsItem,
  DirectoryTree as ADirectoryTree,
  Divider as ADivider,
  Drawer2 as ADrawer,
  Dropdown as ADropdown,
  DropdownButton as ADropdownButton,
  Empty as AEmpty,
  Flex as AFlex,
  FloatButton as AFloatButton,
  FloatButtonGroup as AFloatButtonGroup,
  Form as AForm,
  FormItem as AFormItem,
  FormItemRest as AFormItemRest,
  Image as AImage,
  ImagePreviewGroup as AImagePreviewGroup,
  Input as AInput,
  InputGroup as AInputGroup,
  InputNumber as AInputNumber,
  InputPassword as AInputPassword,
  InputSearch as AInputSearch,
  Layout as ALayout,
  LayoutContent as ALayoutContent,
  LayoutFooter as ALayoutFooter,
  LayoutHeader as ALayoutHeader,
  LayoutSider as ALayoutSider,
  List as AList,
  ListItem as AListItem,
  ListItemMeta as AListItemMeta,
  LocaleProvider as ALocaleProvider,
  Mentions as AMentions,
  MentionsOption as AMentionsOption,
  Menu2 as AMenu,
  MenuDivider as AMenuDivider,
  MenuItem as AMenuItem,
  MenuItemGroup as AMenuItemGroup,
  Modal as AModal,
  MonthPicker as AMonthPicker,
  PageHeader as APageHeader,
  Pagination as APagination,
  Popconfirm as APopconfirm,
  Popover as APopover,
  Progress as AProgress,
  QRCode as AQRCode,
  QuarterPicker as AQuarterPicker,
  Radio as ARadio,
  RadioButton as ARadioButton,
  RadioGroup as ARadioGroup,
  RangePicker as ARangePicker,
  Rate as ARate,
  Result as AResult,
  Row as ARow,
  Segmented as ASegmented,
  Select as ASelect,
  SelectOptGroup as ASelectOptGroup,
  SelectOption as ASelectOption,
  Skeleton2 as ASkeleton,
  SkeletonAvatar as ASkeletonAvatar,
  SkeletonButton as ASkeletonButton,
  SkeletonImage as ASkeletonImage,
  SkeletonInput as ASkeletonInput,
  SkeletonTitle as ASkeletonTitle,
  Slider as ASlider,
  Space as ASpace,
  Spin as ASpin,
  Statistic as AStatistic,
  StatisticCountdown as AStatisticCountdown,
  Step as AStep,
  Steps as ASteps,
  SubMenu as ASubMenu,
  Switch as ASwitch,
  TabPane as ATabPane,
  Table as ATable,
  TableColumn as ATableColumn,
  TableColumnGroup as ATableColumnGroup,
  TableSummary as ATableSummary,
  TableSummaryCell as ATableSummaryCell,
  TableSummaryRow as ATableSummaryRow,
  Tabs as ATabs,
  Tag as ATag,
  Textarea as ATextarea,
  TimePicker as ATimePicker,
  TimeRangePicker as ATimeRangePicker,
  Timeline as ATimeline,
  TimelineItem as ATimelineItem,
  Tooltip as ATooltip,
  Tour as ATour,
  Transfer as ATransfer,
  Tree as ATree,
  TreeNode as ATreeNode,
  TreeSelect as ATreeSelect,
  TreeSelectNode as ATreeSelectNode,
  Typography as ATypography,
  TypographyLink as ATypographyLink,
  TypographyParagraph as ATypographyParagraph,
  TypographyText as ATypographyText,
  TypographyTitle as ATypographyTitle,
  Upload as AUpload,
  UploadDragger as AUploadDragger,
  Watermark as AWatermark,
  WeekPicker as AWeekPicker,
  KbAntIcon,
  KbAntShell,
  KbLucideIcon
};
