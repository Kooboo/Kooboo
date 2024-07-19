import { getQueryString } from "@/utils/url";
import { ignoreCaseEqual } from "@/utils/string";

export function isClassic() {
  return ignoreCaseEqual("true", getQueryString("classic") ?? "false");
}
