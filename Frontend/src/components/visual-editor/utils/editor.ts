import type {
  KeyValue,
  RichEditorInstance,
  RichEditorMenuItem,
} from "@/global/types";

import { i18n } from "@/modules/i18n";

type RichEditorMenuButtons = {
  name: string;
  text: string;
  menus: KeyValue[];
};

export function extendEmailEditorButtons(): Record<string, any> {
  const t = i18n.global.t;

  const buttonGroups: RichEditorMenuButtons[] = [
    {
      name: "mergeFields",
      text: t("ve.mergeFields"),
      menus: [
        {
          key: t("tinymce.name"),
          value: "{{Contact.Name}}",
        },
        {
          key: t("tinymce.emailAddress"),
          value: "{{Contact.EmailAddress}}",
        },
      ],
    },
    {
      name: "specialLinks",
      text: t("ve.specialLinks"),
      menus: [
        {
          key: t("ve.unsubscribe"),
          value: `<a href="Link.Unsubscribe" target="_blank">${t(
            "ve.unsubscribe"
          )}</a>`,
        },
      ],
    },
  ];
  const buttons = buttonGroups.map((it) => it.name).join(" ");
  return {
    "additional-toolbar-buttons": buttons,
    "editor-setup": (editor: RichEditorInstance) => {
      for (const { name, text, menus } of buttonGroups) {
        editor.ui.registry.addMenuButton(name, {
          text,
          fetch: (callback: (items: RichEditorMenuItem[]) => void) => {
            const items: RichEditorMenuItem[] = menus.map((it) => {
              return {
                type: "menuitem",
                text: it.key,
                onAction() {
                  editor.insertContent(it.value);
                },
              };
            });
            callback(items);
          },
        });
      }
    },
  };
}
