import { i18n } from "@/modules/i18n";
import dayjs from "dayjs";

const t = i18n.global.t;

export function weekToDates(input: string) {
  if (!input) {
    return input;
  }

  const [year, week] = input.split("-");
  if (!year || !week) {
    return input;
  }

  const baseDate = dayjs(`${year}-01-01`).startOf("year");
  const start = baseDate.add(+week - 1, "week").format("YYYY-MM-DD");
  const end = baseDate
    .add(+week, "week")
    .add(-1, "day")
    .format("YYYY-MM-DD");

  return `${start} ~ ${end}`;
}

export function timeZoneOffset() {
  return -(new Date().getTimezoneOffset() / 60);
}

export const durationUnits = [
  { key: "Year", value: t("common.year") },
  { key: "Month", value: t("common.month") },
  { key: "Week", value: t("common.week") },
  { key: "Day", value: t("common.day") },
];
