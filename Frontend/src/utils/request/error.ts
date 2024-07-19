import type { AxiosError } from "axios";
import { i18n } from "@/modules/i18n";
import { errorMessage } from "@/components/basic/message";

const $t = i18n.global.t;

export default {
  async showMessage(config: AxiosError) {
    const response = config.response?.data;
    const messages: string[] = [];
    if (config.response && config.response?.status >= 500) {
      messages.push($t("common.internalServerError"));
    } else if (!response) {
      messages.push($t("common.networkError"));
    } else if (typeof response === "string") {
      messages.push(response);
    } else {
      messages.push(...response);
    }

    for (const message of messages) {
      errorMessage(message);
    }
  },
};
