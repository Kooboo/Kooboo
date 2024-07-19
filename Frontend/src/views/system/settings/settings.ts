import { saveSite, uploadPackage } from "@/api/site";

import { ElMessage } from "element-plus";
import type { Site } from "@/api/site/site";
import { i18n } from "@/modules/i18n";
import { ref } from "vue";
import { refreshMonacoCache } from "@/components/monaco-editor/monaco";
import { useSiteStore } from "@/store/site";

export const site = ref<Site | undefined>();
const siteStore = useSiteStore();
const $t = i18n.global.t;

export const events = {} as {
  onPwaSave: (site: Site) => void;
  onMultilingualSave: (site: Site) => void;
  onLighthouseSave: (site: Site) => void;
  onCustomSettingsSave: (site: Site) => void;
  onVisitorCountryRestrictionsSave: (site: Site) => void;
};

export const importAccept = [
  "application/zip",
  "application/x-zip",
  "application/octet-stream",
  "application/x-zip-compressed",
];

export const load = async () => {
  site.value = JSON.parse(JSON.stringify(siteStore.site));
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const onUploadPackage = async (file: any) => {
  if (
    file.name.indexOf(".") > -1 &&
    file.name.split(".").reverse()[0] === "zip"
  ) {
    const formdata = new FormData();
    formdata.append("file", file.raw);
    await uploadPackage(formdata);
    await siteStore.loadSite();
    load();
  } else {
    ElMessage.error($t("common.fileFormatIsIncorrect"));
  }
};

export const save = async () => {
  if (site.value?.specialPath) {
    site.value.specialPath = Array.from(
      new Set(site.value.specialPath.filter((f) => f.trim() !== ""))
    );
  }
  const _site: Site = JSON.parse(JSON.stringify(site.value));
  events.onPwaSave(_site);
  events.onMultilingualSave(_site);
  events.onVisitorCountryRestrictionsSave?.(_site);
  events.onLighthouseSave(_site);
  events.onCustomSettingsSave(_site);

  await saveSite(_site);
  siteStore.loadSite();
  siteStore.loadSites();
  refreshMonacoCache();
};
