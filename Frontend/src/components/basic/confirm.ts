import { ElMessageBox } from "element-plus";
import { h } from "vue";
import LogoIcon from "@/assets/images/logo-transparent.svg?raw";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const showConfirm = (message: string) =>
  ElMessageBox({
    message: h("div", [
      h("div", { innerHTML: LogoIcon, class: "el-message-box__logo" }),
      h("div", message),
    ]),
    customClass: "el-message-box--confirm",
    showCancelButton: true,
    confirmButtonText: $t("common.ok"),
    cancelButtonText: $t("common.cancel"),
    center: true,
    roundButton: true,
    // confirmButtonClass: "el-message-box__btn-confirm",
    // cancelButtonClass: "el-message-box__btn-cancel",
  });

export const showDeleteConfirm = (count?: number) =>
  showConfirm(
    count && count > 1
      ? $t("common.deleteTheseCountRecordsTips", { count })
      : $t("common.deleteThisRecordTips")
  );
export const showDeleteEmailConfirm = () =>
  showConfirm($t("common.deleteEmailTips"));
export const showDeleteMessageConfirm = (count: number) =>
  showConfirm(
    count && count > 1
      ? $t("common.deleteTheseCountMessagesTips", { count })
      : $t("common.deleteThisMessageTips")
  );
export const showDeletesConfirm = () =>
  showConfirm($t("common.deleteItemsTips"));

export const showFileExistsConfirm = () =>
  showConfirm($t("common.fileExistTip"));

export const showLeavePageConfirm = () => showConfirm($t("common.unsavedTip"));

export const showForceSaveConfirm = () =>
  showConfirm($t("common.forceSaveTip"));

export const showHasRelationProductTypeConfirm = () =>
  showConfirm($t("common.deleteHasRelationProductTypeTips"));
