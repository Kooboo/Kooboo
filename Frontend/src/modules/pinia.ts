import type { Install } from "./types";
import { createPinia } from "pinia";

export const install: Install = (app) => {
  app.use(createPinia());
};
