import type { AxiosError } from "axios";
import axios from "axios";
import loading from "./loading";
import error from "./error";
import type { RequestConfig } from "./types";
import { router } from "@/modules/router";
import { useAppStore } from "@/store/app";
import { usePersistent } from "../../store/persistent";
import { errorMessage, notAccessMessage } from "@/components/basic/message";
import { useDiffStore } from "@/store/diff";
import { ElMessage } from "element-plus";

const instance = axios.create({
  timeout: 1000 * 60,
  baseURL: import.meta.env.VITE_API,
  headers: {
    "Content-type": "application/json;charset=UTF-8",
  },
});

instance.interceptors.request.use(
  (config: RequestConfig) => {
    if (!config.hiddenLoading) loading.open();
    return config;
  },
  (err: AxiosError) => {
    const config: RequestConfig = err.request;
    if (!config.hiddenLoading) loading.close();
    throw err;
  }
);

instance.interceptors.response.use(
  (config) => {
    const requestConfig: RequestConfig = config.config;
    const {
      access_token,
      accesstoken,
      AccessToken,
      IsOnlineServer,
      isonlineserver,
      isOnlineServer,
      ForceUpgrade,
      forceUpgrade,
      forceupgrade,
    } = config.headers;
    const { isOnlineServer: isOnlineServer_ } = usePersistent();

    if (IsOnlineServer || isonlineserver || isOnlineServer) {
      isOnlineServer_.value = JSON.parse(
        (IsOnlineServer || isonlineserver || isOnlineServer).toLowerCase()
      );
    }

    if (ForceUpgrade || forceUpgrade || forceupgrade) {
      useAppStore().forceUpgrade();
    }

    if (config.status === 200 && (access_token || accesstoken || AccessToken)) {
      useAppStore().login(access_token || accesstoken || AccessToken);
    }

    const successMessage = (config?.config as RequestConfig)?.successMessage;

    if (config.status === 200 && successMessage) {
      ElMessage.success({
        message: successMessage,
        grouping: true,
      });
    }

    if (!requestConfig.hiddenLoading) loading.close();
    return config;
  },
  async (err: AxiosError) => {
    const config: RequestConfig = err.config;
    if (!config.hiddenLoading) loading.close();

    if (err.response?.status === 409) {
      const diffStore = useDiffStore();
      const data = JSON.parse(err.config.data);
      await diffStore.showDiaLog(data, err.response.data);
      err.config.data = JSON.stringify(data);
      return await instance.request(err.config);
    }

    if (err.response?.status === 403) {
      notAccessMessage();
      throw Error();
    }

    if (err.response?.status === 401) {
      if (router.currentRoute.value.name === "login") return;
      router.replace({ name: "login" });
      notAccessMessage();
    } else if (!config.hiddenError) {
      error.showMessage(err);
    }

    if (err.response?.status === 400 && config.errorMessage) {
      errorMessage(config.errorMessage, config.keepShowErrorMessage);
    }

    throw err;
  }
);

export default instance;
