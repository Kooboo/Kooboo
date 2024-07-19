import { createApp } from "vue/dist/vue.esm-bundler.js";
import "virtual:windi.css";
import "element-plus/dist/index.css";
import KButton from "./k-button.vue";
import KInput from "./k-input.vue";
import KForm from "./k-form.vue";
import KPagination from "./k-pagination.vue";
import KText from "./k-text.vue";
import KHtml from "./k-html.vue";
import KImage from "./k-image.vue";
import KPage from "./k-page.vue";
import dataComponents from "./data";
import layoutComponents from "./layout";
import tableComponents from "./table";

import { install as installI18n } from "@/modules/i18n";

const app = createApp({
  components: {
    KButton,
    KInput,
    KPagination,
    KForm,
    KText,
    KPage,
    KHtml,
    KImage,
    ...tableComponents,
    ...layoutComponents,
    ...dataComponents,
  },
});

installI18n(app);
app.mount("#app");
