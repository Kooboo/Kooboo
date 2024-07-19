import { showLeavePageConfirm } from "@/components/basic/confirm";
import { onMounted, onUnmounted } from "vue";
import { type RouteRecordName, onBeforeRouteLeave } from "vue-router";

export function useSaveTip(
  replacer?: (key: string, value: unknown) => unknown,
  beforeRouteLeaveHook?: {
    defaultActiveMenu: string;
    activeMenuChanged?: (value?: RouteRecordName | string | null) => void;
    modelGetter: () => unknown;
  }
) {
  const beforeunloadAction = (event: {
    preventDefault: () => void;
    returnValue: string;
  }) => {
    if (showBeforeUnload) {
      event.preventDefault();
      event.returnValue = "";
    }
  };
  onMounted(() => {
    window.addEventListener("beforeunload", beforeunloadAction);
  });
  onUnmounted(() => {
    window.removeEventListener("beforeunload", beforeunloadAction);
  });
  let initialValue: string | undefined = undefined;
  let showBeforeUnload: boolean;

  const init = (value: unknown) => {
    showBeforeUnload = false;
    initialValue = JSON.stringify(value, replacer).replace(/\\r\\n/g, "\\n");
  };

  const changed = (value: unknown) => {
    if (initialValue === undefined) return false;
    const newValue = JSON.stringify(value, replacer).replace(/\\r\\n/g, "\\n");
    showBeforeUnload = initialValue !== newValue;
    return initialValue !== newValue;
  };

  const check = async (value: unknown) => {
    if (changed(value)) {
      await showLeavePageConfirm();
    }
  };

  if (beforeRouteLeaveHook) {
    onBeforeRouteLeave(async (to, from, next) => {
      if (to.name === "login") {
        next();
      } else {
        beforeRouteLeaveHook.activeMenuChanged?.call(
          null,
          to.meta.activeMenu ?? to.name
        );
        const model = beforeRouteLeaveHook.modelGetter?.call(null);
        if (!model) {
          next();
          return;
        }
        await check(model)
          .then(() => {
            next();
          })
          .catch(() => {
            beforeRouteLeaveHook.activeMenuChanged?.call(
              null,
              beforeRouteLeaveHook.defaultActiveMenu
            );
            next(false);
          });
      }
    });
  }
  return { init, check, changed, getValue: () => initialValue };
}
