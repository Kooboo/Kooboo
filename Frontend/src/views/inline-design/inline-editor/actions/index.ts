import type { Action } from "../types";
const actions = import.meta.globEager(
  "@/views/inline-design/inline-editor/actions/*.ts"
);

export function createActions() {
  const list: Action[] = [];

  for (const key in actions) {
    const action = actions[key].default;
    list.push(action);
  }

  return list.sort((a, b) => a.order - b.order);
}
