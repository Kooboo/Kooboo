import { endEdit } from "@/views/inline-design/inline-editor";
import type { Action } from "../types";
import EditAction from "../icon-action.vue";

export default {
  name: "cancel",
  invoke: () => {
    endEdit(true);
  },
  component: EditAction,
  order: 2,
  icon: "icon-close",
} as Action;
