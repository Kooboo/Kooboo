import type { ResolvedRoute } from "@/api/route/types";
import { errorMessage } from "@/components/basic/message";
import { i18n } from "@/modules/i18n";
import { provide } from "vue";

const $t = i18n.global.t;

export default {
  automaticLayout: true,
  fontSize: 14,
  wordWrap: "on",
  fontFamily:
    "Monaco,Consolas,Lucida Console,Liberation Mono,DejaVu Sans Mono,Bitstream Vera Sans Mono,Courier New, monospace",
} as any;

export const onFileOpenInjectionFlag = "monaco:openFile";
export const onFileNotFoundInjectionFlag = "monaco:onFileNotFound";

export function provideDefaultFileNotFound() {
  provide(onFileNotFoundInjectionFlag, (url: string) => {
    errorMessage($t("common.resourceNotFound", { url }));
  });
}

export function provideDefaultFileOpen() {
  provide(onFileOpenInjectionFlag, (req: ResolvedRoute) => {
    console.log([onFileOpenInjectionFlag, req]);
  });
}
