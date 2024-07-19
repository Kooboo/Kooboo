import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;

export const timeType = [
  { name: "second", id: 6, display: $t("common.seconds") },
  { name: "minutes", id: 3, display: $t("common.minutes") },
  { name: "hour", id: 1, display: $t("common.hours") },
  { name: "day", id: 0, display: $t("common.days") },
  { name: "week", id: 4, display: $t("common.weeks") },
  { name: "month", id: 5, display: $t("common.months") },
];
