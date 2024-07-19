import dayjs from "dayjs";
import { i18n } from "@/modules/i18n";
import utc from "dayjs/plugin/utc";
import timezone from "dayjs/plugin/timezone";
dayjs.extend(utc);
dayjs.extend(timezone);
const $t = i18n.global.t;

export function getCurrentTimeZone() {
  return dayjs.tz.guess();
}

export function useUtcLocalDate(date: string | Date) {
  return dayjs(date).utc().local().format();
}

export function useDate(date: string | Date, format = "YYYY-MM-DD") {
  return dayjs(date).format(format);
}

export function useTime(date: string | Date) {
  if (!date) return "";
  return useDate(date, "YYYY-MM-DD HH:mm:ss");
}

export function useHourMinuteSecond(date: string | Date) {
  return useDate(date, "HH:mm:ss");
}

export function useMonthDay(date: string | Date) {
  return useDate(date, "MM-DD");
}

export function useYearMonth(date: string | Date) {
  return useDate(date, "YYYY-MM");
}

export function useHourMinute(date: string | Date) {
  return useDate(date, "HH:mm");
}

// 判断某个时间是否在某个时间段内
export function isDuringTime(
  currentT: string | number | Date,
  startT: string | number | Date,
  endT: string | number | Date
) {
  const currentDate = new Date(currentT);
  const startDate = new Date(startT);
  const endDate = new Date(endT);

  const a = currentDate.getTime() - startDate.getTime();
  const b = currentDate.getTime() - endDate.getTime();

  if (a < 0 || b > 0) {
    return false;
  } else {
    return true;
  }
}

// 获取后几天或者前几天的日期
export function getNextDate(date: string, day: number) {
  const dd = new Date(date);
  dd.setDate(dd.getDate() + day);
  const y = dd.getFullYear();
  const m =
    dd.getMonth() + 1 < 10 ? "0" + (dd.getMonth() + 1) : dd.getMonth() + 1;
  const d = dd.getDate() < 10 ? "0" + dd.getDate() : dd.getDate();
  return y + "-" + m + "-" + d;
}

export const week = [
  $t("common.sunday"),
  $t("common.monday"),
  $t("common.tuesday"),
  $t("common.wednesday"),
  $t("common.thursday"),
  $t("common.friday"),
  $t("common.saturday"),
];
