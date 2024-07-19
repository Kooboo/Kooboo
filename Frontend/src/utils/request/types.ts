import type { AxiosRequestConfig } from "axios";

export interface RequestConfig extends AxiosRequestConfig {
  hiddenLoading?: boolean;
  hiddenError?: boolean;
  successMessage?: string;
  errorMessage?: string;
  keepShowErrorMessage?: boolean;
}
