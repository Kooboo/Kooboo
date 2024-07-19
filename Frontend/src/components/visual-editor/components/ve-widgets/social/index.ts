import { icons } from "./icons";
import { newGuid } from "@/utils/guid";
import { ref } from "vue";

export type SocialType = "share" | "follow" | "others";

export type SocialItem = {
  id: string;
  name: string;
  prefix?: string;
  url: string;
  title: string;
  alternateText: string;
  type: SocialType;
  icon: string;
  previewIcon?: string;
};

const socialGroups = ref<Record<string, SocialItem[]>>({});
for (const key in icons) {
  if (Object.prototype.hasOwnProperty.call(icons, key)) {
    const items: SocialItem[] = icons[key];
    socialGroups.value[key] = items.map((it) => {
      it.icon = socialIcon(it);
      return it;
    });
  }
}

function socialIcon(item: SocialItem) {
  const basePath = import.meta.env.VITE_BASE_PATH || "/";
  return `${basePath}social/${item.type}/${item.icon}`;
}

export function getDefaultSocials() {
  return socialGroups.value["follow"].slice(0, 4).map((it) => {
    it.id = newGuid();
    return it;
  });
}

export function useSocialEffects() {
  return {
    socialGroups,
  };
}
