import { handleMessage } from "../../utils/message";
import { ref } from "vue";

const currentTab = ref("widgets");

export function handleIframeMessages() {
  handleMessage({
    ["switch-tab"](data) {
      currentTab.value = data.item;
    },
  });
}

export function useSidebarEffects() {
  currentTab.value = "widgets";
  return {
    currentTab,
  };
}
