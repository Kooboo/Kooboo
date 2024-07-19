import { monaco } from "./userWorker";

export function addShortcuts(editor: any) {
  editor.addAction({
    id: "ManualTriggerSuggest",
    label: "ManualTriggerSuggest",
    keybindings: [
      monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyJ,
      monaco.KeyMod.CtrlCmd | monaco.KeyCode.Space,
      monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode.Space,
    ],
    precondition: null,
    keybindingContext: null,
    contextMenuGroupId: "ManualTriggerSuggest",
    contextMenuOrder: 1.5,
    run: function (ed: any) {
      ed.getAction("editor.action.triggerSuggest").run();
    },
  });
}
