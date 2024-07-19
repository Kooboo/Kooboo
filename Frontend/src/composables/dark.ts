import { useDark, useToggle } from "@vueuse/core";

export const dark = useDark();
export const toggleDark = useToggle(dark);
