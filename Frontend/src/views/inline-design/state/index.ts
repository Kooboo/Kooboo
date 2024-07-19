import type { Update } from "@/api/site/inline-design";
import { update } from "@/api/site/inline-design";
import type { KeyValue } from "@/global/types";
import { computed, ref } from "vue";
import type { Change } from "./change";
import type { Operation } from "./operation";
import { K_ATTRIBUTE_PLACEHOLDER, K_DIRTY } from "../constants";
import { getChildElements } from "@/utils/dom";
import { newGuid } from "@/utils/guid";
import type { KoobooBinding } from "../binding";
import { getKoobooId } from "../binding";

export const stashes = ref<Operation<Change>[][]>([]);
export const histories = ref<Operation<Change>[][]>([]);

export function undo() {
  const history = histories.value[histories.value.length - 1];
  console.log(history);
  if (history) {
    for (const i of [...history].reverse()) {
      i.undo();
    }

    stashes.value.push(history);
  }

  histories.value.pop();
}

export function redo() {
  const history = stashes.value[stashes.value.length - 1];
  console.log(history);
  if (history) {
    for (const i of history) {
      i.redo();
    }

    histories.value.push(history);
  }

  stashes.value.pop();
}

export function addHistory(operations: Operation<Change>[]) {
  console.log(operations);
  histories.value.push(operations);
  stashes.value = [];
}

export async function saveChanges(pageId: string) {
  const updates: Update[] = [];

  for (const history of histories.value) {
    for (const operation of history) {
      for (const change of operation.changes) {
        updates.push(toUpdate(change));
      }
    }
  }

  await update(pageId, updates);
}

export const historyCount = computed(() => histories.value.length);
export const stashCount = computed(() => stashes.value.length);

export function ensurePlaceholderAttribute(el: Element) {
  let id = el.getAttribute(K_ATTRIBUTE_PLACEHOLDER);

  if (!id) {
    id = newGuid();
    el.setAttribute(K_ATTRIBUTE_PLACEHOLDER, id);
  }

  return id;
}

export function markDirty(
  el: Element,
  scope: "children" | "self" | "brothers" = "children"
) {
  let elements: Element[] = [];

  switch (scope) {
    case "brothers":
      if (el.parentElement) elements = getChildElements(el.parentElement, true);
      break;
    case "self":
      elements = getChildElements(el, true, true);
      break;
    case "children":
    default:
      elements = getChildElements(el, true);
      break;
  }

  for (const i of elements) {
    i.setAttribute(K_DIRTY, "");
  }
}

function isDirty(el: Element) {
  return el.hasAttribute(K_DIRTY);
}

export function tryGetPureParent(el: HTMLElement, endWith?: KoobooBinding) {
  let isSelf = true;
  let current = el;

  while (current) {
    if (!isDirty(current) && getKoobooId(current)) break;

    if (
      current == endWith?.reference ||
      endWith?.references?.includes(current)
    ) {
      return null;
    }

    current = current.parentElement!;
    isSelf = false;
  }

  return {
    isSelf,
    element: current,
  };
}

function toUpdate(change: any): Update {
  const infos: KeyValue[] = [];

  for (const key in change) {
    infos.push({
      key: key.toLowerCase(),
      value: change[key]?.toString(),
    });
  }

  return {
    source: change.source,
    infos,
  };
}
