import type { Action } from "../types";
import EditAction from "../icon-action.vue";
import { endEdit } from "@/views/inline-design/inline-editor";

export default {
  name: "save",
  invoke: () => {
    endEdit(false);
  },
  component: EditAction,
  order: 1,
  icon: "icon-yes3",
  divider: true,
} as Action;
