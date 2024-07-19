import { defineConfig, loadEnv } from "vite";

import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";
import Vue from "@vitejs/plugin-vue";
import VueI18n from "@intlify/vite-plugin-vue-i18n";
import WindiCSS from "vite-plugin-windicss";
import path from "node:path";

export default defineConfig(({ mode }) => {
  process.env = { ...process.env, ...loadEnv(mode, process.cwd()) };

  return {
    base: process.env.VITE_BASE_PATH,
    server: {
      port: 3000,
      proxy: {
        "/_Admin/__cover": {
          target: process.env.VITE_API_PROXY,
          changeOrigin: true,
        },
        "/_thumbnail/.*": {
          target: process.env.VITE_API_PROXY,
          changeOrigin: true,
        },
        "^(?!/_Admin/).*": {
          target: process.env.VITE_API_PROXY,
          changeOrigin: true,
        },
      },
      fs: {
        strict: false,
      },
    },
    plugins: [
      Vue({
        template: {
          compilerOptions: {
            isCustomElement: (tag) => ["svg-editor-component"].includes(tag),
          },
        },
      }),
      AutoImport({
        resolvers: [],
        dts: "src/auto-imports.d.ts",
      }),
      Components({
        resolvers: [ElementPlusResolver({ importStyle: false })],
        dts: "src/auto-import-components.d.ts",
        types: [
          {
            from: "vue-router",
            names: ["RouterLink", "RouterView"],
          },
        ],
      }),
      VueI18n({
        runtimeOnly: true,
        compositionOnly: true,
        include: [path.resolve(__dirname, "locales/**")],
      }),
      WindiCSS({
        scan: {
          dirs: ["src"],
          exclude: [
            "node_modules",
            ".git",
            "public/**/*",
            "*.template.html",
            "index.html",
            "src/assets/iconfont/**/*",
          ],
          include: [],
        },
        transformCSS: "pre",
      }),
    ],
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "src"),
      },
    },
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: ' @import "@/assets/styles/variable.scss";',
        },
      },
    },
    build: {
      rollupOptions: {
        input: {
          main: path.resolve(__dirname, "index.html"),
          component: path.resolve(__dirname, "component.html"),
          monaco: path.resolve(__dirname, "monaco.html"),
          "visual-editor": path.resolve(__dirname, "page-editor-inject.html"),
        },
      },
      outDir: "../Kooboo.Web/_Admin",
      emptyOutDir: true,
      target: "es2015",
    },
  };
});
