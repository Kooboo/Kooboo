import type { AxiosError, AxiosRequestConfig } from "axios";

import axios from "axios";
import { useUrlSiteId } from "@/hooks/use-site-id";

const request = axios.create({
  timeout: 1000 * 60,
  baseURL: import.meta.env.VITE_API,
  headers: {
    "Content-type": "application/json;charset=UTF-8",
  },
});
request.interceptors.request.use(
  (config: AxiosRequestConfig) => {
    const token = localStorage.getItem("TOKEN");
    if (!config.headers) {
      config.headers = {};
    }

    if (token) {
      config.headers.Authorization = `bearer ${token}`;
    }

    return config;
  },
  (err: AxiosError) => {
    throw err;
  }
);

interface VeComponentSource {
  body: string;
  metaBindings: [];
  urlParamsBindings: [];
}

export async function getSource(
  tag: string,
  id: string
): Promise<VeComponentSource> {
  const url = useUrlSiteId("Component/GetSource");
  const { data } = await request.get<VeComponentSource>(url, {
    params: {
      tag,
      id,
    },
  });
  return data;
}
