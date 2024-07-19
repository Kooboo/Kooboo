<script lang="tsx">
import type { PropType } from "vue";
import { defineComponent, createVNode } from "vue";
import type { IKTabItem } from "./types";
import useKTabs from "./use-k-tabs";

export default defineComponent({
  name: "KTabs",
  props: {
    items: {
      type: Object as PropType<Array<IKTabItem>>,
      required: true,
    },
    defaultActive: {
      type: String,
      default: "",
    },
    routeName: {
      type: String,
      required: true,
    },
    lazyRender: {
      type: Boolean,
      default: false,
    },
  },
  emits: ["tab-click", "update:modelValue"],
  setup(props: any, { emit, expose }) {
    const defaultActive = props.defaultActive || props.items[0].name;
    const { activeTab, handleTabClick, selectTab } = useKTabs(
      defaultActive,
      props.routeName,
      emit
    );

    expose({
      selectTab,
    });

    const onTabClick = (val: any) => {
      emit("tab-click", val);
      handleTabClick(val);
    };

    return () => (
      <div class="k-tabs">
        <el-tabs vModel={activeTab.value} onTabClick={onTabClick}>
          {props.items.map((item: any) => (
            <el-tab-pane key={item.name} label={item.label} name={item.name}>
              {props.lazyRender
                ? activeTab.value === item.name
                  ? createVNode(item.content)
                  : ""
                : createVNode(item.content)}
            </el-tab-pane>
          ))}
        </el-tabs>
      </div>
    );
  },
});
</script>

<style lang="scss" scoped>
:deep(.el-tabs__item) {
  text-align: center;
  width: 120px;
  height: 40px;

  margin: 6px 8px;
  box-sizing: border-box;
  transition: all 0.3s ease-in-out;
}
:deep(.el-tabs__nav-wrap) {
  background: #e9eaf0;
  padding: 6px 0;
  box-sizing: border-box;
  .is-active {
    background: #ffffff;
    border-radius: 20px;
    box-shadow: 0px 2px 4px 0px rgb(0 0 0 / 10%);
  }
  .el-tabs__active-bar {
    background-color: transparent;
  }
}
</style>
