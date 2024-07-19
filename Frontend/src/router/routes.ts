import type { RouteRecordRaw } from "vue-router";
import siteRoutes from "./site";
import sitesRoutes from "./sites";
import accountRoutes from "./account";
import mailRoutes from "./mail";
import consoleRoute from "./console";
import commonRouters from "./common";
import templateRoutes from "./template";
import partnerRoutes from "./partner";

const routes: Array<RouteRecordRaw> = [
  ...sitesRoutes,
  ...mailRoutes,
  ...consoleRoute,
  ...accountRoutes,
  ...siteRoutes,
  ...commonRouters,
  ...templateRoutes,
  ...partnerRoutes,

  {
    path: "/:pathMatch(.*)",
    redirect: "404",
  },
];

export default routes;
