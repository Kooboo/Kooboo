import type { Install } from "./types";
import { VueMasonryPlugin } from "vue-masonry";

export const install: Install = (app) => {
  app.use(VueMasonryPlugin);
};
