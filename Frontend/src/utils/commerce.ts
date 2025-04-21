import { useAppStore } from "@/store/app";

export function toSeoName(str?: string) {
  if (!str) return "";
  const fragments = str
    .toLowerCase()
    .split(/[-[\]\\{}#%^*+=_|~<>.?!'"/:;,()$&@\s\r\n\f]/g)
    .filter((f) => f);
  return fragments.join("-");
}

export function localDisplay(values: Record<string, string>, fallback: string) {
  try {
    if (!values) return fallback;
    const langs = [navigator.language];
    if (navigator.language.includes("-")) {
      langs.push(...navigator.language.split("-"));
    }

    for (const element of langs) {
      const key = Object.keys(values).find((f) =>
        f?.includes(element.toLowerCase())
      );
      if (key) return values[key];
    }
  } catch (error) {
    //
  }

  return fallback;
}

export function systemDisplay(
  values: Record<string, string>,
  fallback: string
) {
  try {
    if (!values) return fallback;
    let lang = useAppStore().header?.user.language;
    if (lang == "zh") lang = "cn";
    if (!lang) return fallback;
    const key = Object.keys(values).find((f) =>
      f?.includes(lang.toLowerCase())
    );
    if (key) return values[key];
  } catch (error) {
    //
  }

  return fallback;
}
