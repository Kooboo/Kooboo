import {
  useLocalStorage,
  StorageSerializers,
  createGlobalState,
} from "@vueuse/core";
import { StorageConstants } from "@/constants/storage-constants";

export const usePersistent = createGlobalState(() => {
  return {
    token: useStorage<string>(StorageConstants.TOKEN),
    language: useStorage<string>(StorageConstants.LANGUAGE),
    isOnlineServer: useStorage<boolean>(
      StorageConstants.IsOnlineServer,
      "boolean"
    ),
    inlineDesignWidth: useStorage<string>("inlineDesignWidth"),
  };
});

function useStorage<T>(
  name: string,
  type:
    | "string"
    | "number"
    | "boolean"
    | "object"
    | "any"
    | "map"
    | "set"
    | "date" = "string"
) {
  return useLocalStorage<T | null>(name, null, {
    serializer: StorageSerializers[type],
  });
}
