import dayjs from "dayjs";

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
