/* eslint-disable @typescript-eslint/no-explicit-any */
import { onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { isString } from "lodash-es";
import { UPDATE_MODEL_EVENT } from "@/constants/constants";

export default function useKTabs(
  defaultActive: string,
  routeName: string,
  emit: any
) {
  const activeTab = ref(defaultActive);

  emit(UPDATE_MODEL_EVENT, activeTab.value);
  watch(
    () => activeTab.value,
    () => {
      emit(UPDATE_MODEL_EVENT, activeTab.value);
    }
  );

  const route = useRoute();
  const router = useRouter();
  const doRouter = (name: any) => {
    router.push({
      name: routeName,
      query: {
        ...route.query,
        name,
      },
    });
  };

  onMounted(() => {
    setTimeout(() => {
      const { name } = route.query;
      if (isString(name)) {
        activeTab.value = name;
      }
      doRouter(activeTab.value);
    }, 20);
  });

  function handleTabClick(val: any) {
    doRouter(val.paneName);
  }

  function selectTab(val: any) {
    activeTab.value = val;
    doRouter(val);
  }

  return {
    activeTab,
    handleTabClick,
    selectTab,
  };
}
