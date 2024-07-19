import type { DatabaseType } from "@/api/database";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import type {
  LocationQueryRaw,
  RouteLocationRaw,
  RouteQueryAndHash,
} from "vue-router";
import { useRoute } from "vue-router";
export function useTable() {
  const route = useRoute();
  const listRoute: Record<string, DatabaseType> = {
    table: "Database",
    "sqlite.table": "Sqlite",
    "mysql.table": "MySql",
    "sqlserver.table": "SqlServer",
  };

  let dbType: DatabaseType;
  const dbTypeQs = getQueryString("dbType");
  if (dbTypeQs) {
    dbType = dbTypeQs as DatabaseType;
  } else {
    dbType = listRoute[route.name as string];
  }
  if (!dbType) {
    dbType = "Database";
  }

  function appendQueryToRoute(route: RouteLocationRaw) {
    const routeResult = useRouteSiteId(route);
    if (dbType !== "Database") {
      const query = (routeResult as RouteQueryAndHash)
        .query as LocationQueryRaw;
      query.dbType = dbType;
    }
    return routeResult;
  }

  function getListRouteName() {
    const name = Object.keys(listRoute).find(
      (key) => listRoute[key] === dbType
    );
    return name;
  }

  return {
    dbType,
    appendQueryToRoute,
    getListRouteName,
  };
}
