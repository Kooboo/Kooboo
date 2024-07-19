import type { getCurrentInstance, Ref } from "vue";
import { isRef, ref } from "vue";
import { shallowReactive } from "vue";

type State = ReturnType<typeof getCurrentInstance> | Ref | undefined;

export function usePageState() {
  const pageState = shallowReactive<Record<string, State>>({});

  function getExposedState(componentId: string, exposeName: string) {
    const component = getComponent(componentId);
    if (!component || !component.exposed) return;
    return component.exposed[exposeName];
  }

  function getState<T>(id?: string, exposeName?: string): Ref<T> {
    let result: Ref = ref(undefined);
    if (!id) return result;

    if (exposeName) {
      const state = getExposedState(id, exposeName);
      if (isRef(state)) result = state;
    } else {
      const state = pageState[id];
      if (isRef(state)) result = state;
    }

    return result;
  }

  function getComponent(
    component: string
  ): ReturnType<typeof getCurrentInstance> | undefined {
    const state = pageState[component];
    if (!state || isRef(state)) return;
    return state as ReturnType<typeof getCurrentInstance>;
  }

  function getExposedAction(componentId: string, exposeName: string) {
    const component = getComponent(componentId);
    if (!component || !component.exposed) return;
    const action = component.exposed[exposeName];
    if (typeof action === "function") return action;
  }

  function setState(id: string | undefined, state: State) {
    if (!id || !state) return;
    pageState[id] = state;
  }

  return {
    pageState,
    getState,
    getExposedAction,
    setState,
  };
}

export type PageState = ReturnType<typeof usePageState>;
export const PAGE_STATE_KEY = "__page_state__";
export const PAGE_AUTH_URL = "__page_auth__";
