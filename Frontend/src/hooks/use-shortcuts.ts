export type Shortcut =
  | "format"
  | "save"
  | "StartDebug"
  | "DebugContinue"
  | "DebugInto"
  | "DebugOver"
  | "DebugOut"
  | "DebugStop"
  | "matchCase"
  | "useRegularExpression";

interface Config {
  type: Shortcut;
  metaKey?: boolean; //mac command键
  ctrlKey?: boolean;
  shiftKey?: boolean;
  altKey?: boolean;
  key: string;
}

const configs: Config[] = [
  { type: "format", key: "f", shiftKey: true, altKey: true },
  { type: "format", key: "F", shiftKey: true, altKey: true },
  { type: "save", key: "s", metaKey: true },
  { type: "save", key: "s", ctrlKey: true },
  { type: "StartDebug", key: "F8" },
  { type: "DebugContinue", key: "F5" },
  { type: "DebugInto", key: "F11" },
  { type: "DebugOver", key: "F10" },
  { type: "DebugOut", key: "F11", shiftKey: true },
  { type: "DebugStop", key: "F5", shiftKey: true },
  { type: "matchCase", key: "c", altKey: true },
  { type: "matchCase", key: "ç", metaKey: true, altKey: true },
  { type: "useRegularExpression", key: "r", altKey: true },
  { type: "useRegularExpression", key: "®", metaKey: true, altKey: true },
];

import { onMounted, onBeforeUnmount } from "vue";

export function useShortcut(types: Shortcut, callback: () => void) {
  const action = (e: KeyboardEvent) => {
    const found = configs.find(
      (f) =>
        !!f.altKey === !!e.altKey &&
        !!f.ctrlKey === !!e.ctrlKey &&
        !!f.metaKey === !!e.metaKey &&
        !!f.shiftKey === !!e.shiftKey &&
        f.key === e.key
    );
    if (found && types === found.type) {
      callback();
      e.preventDefault();
    }
  };

  onMounted(() => {
    addEventListener("keydown", action);
  });
  onBeforeUnmount(() => {
    removeEventListener("keydown", action);
  });
}
