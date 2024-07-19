import { ElLoading, ElMessage } from "element-plus";
import type { LoadingInstance } from "element-plus/es/components/loading/src/loading";
import type { UploadFile } from "element-plus/es/components/upload";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export function useFileUpload() {
  let instance: LoadingInstance | null;

  function uploadChange(_file: UploadFile, fileList: UploadFile[]) {
    const uploading = fileList.some((f) => f.status === "uploading");
    if (uploading) {
      if (!instance) {
        instance = ElLoading.service({
          background: "rgba(0, 0, 0, 0.5)",
        });
      }
      return;
    }
    if (instance) {
      instance.close();
      instance = null;
    }
  }

  function getAccepts(fileType: "image") {
    switch (fileType) {
      case "image":
        return [
          "image/bmp",
          "image/x-windows-bmp",
          "image/png",
          "image/jpeg",
          "image/gif",
          "image/webp",
          "image/svg+xml",
          "image/x-icon",
          "image/vnd.microsoft.icon",
        ];
    }
  }

  function checkFile(accepts: string[], file: { type: string; name: string }) {
    if (!accepts.includes(file.type)) {
      setTimeout(() => {
        ElMessage({
          message: $t("common.fileInvalid", { fileName: file.name }),
          grouping: true,
          type: "error",
        });
      }, 0);

      return false;
    }
    return true;
  }

  return {
    uploadChange,
    getAccepts,
    checkFile,
  };
}
