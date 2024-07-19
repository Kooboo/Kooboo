export function toSeoName(str?: string) {
  if (!str) return "";
  return str
    .toLowerCase()
    .split(" ")
    .filter((f) => f)
    .join("-");
}
