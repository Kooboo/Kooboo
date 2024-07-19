import "ts-replace-all";
import "./assets/iconfont/iconfont";
import "./assets/iconfont/iconfont.css";
import "element-plus/dist/index.css";
import "virtual:windi.css";
import "./assets/styles/element-reset.scss";
import "element-plus/theme-chalk/dark/css-vars.css";

import App from "./App.vue";
import { createApp } from "vue";

// inspired by: https://github.com/antfu/vitesse/blob/main/src/main.ts
Object.values(import.meta.globEager("./modules/*.ts"))
  .reduce((app, { install }) => (install?.(app), app), createApp(App))
  .mount("#app");
