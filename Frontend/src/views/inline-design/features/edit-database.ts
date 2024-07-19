import { editDatabase } from "@/views/inline-design/database";
import { dbSources, getBinding, getKoobooBindings } from "../binding";
import { i18n } from "@/modules/i18n";
import type { DatabaseType } from "@/api/database";

const { t } = i18n.global;

export const name = "editDatabase";
export const display = t("common.database");
export const icon = "icon-form";
export const order = 0;
export const immediate = true;

const map: Record<string, DatabaseType> = {
  indexdb: "Database",
  sqlite: "Sqlite",
  mysql: "MySql",
  sqlserver: "SqlServer",
};

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getBinding(bindings, dbSources);
  if (!binding || binding.source == "mongo") return false;
  return true;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getBinding(bindings, dbSources)!;
  await editDatabase(map[binding.source], binding.table!, binding.id);
  location.reload();
}
