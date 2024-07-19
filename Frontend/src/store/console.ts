import { defineStore } from "pinia";
import { ref } from "vue";
import type { DNS, Domain } from "@/api/console/types";
import { getList } from "@/api/console";
import type { SiteItem } from "@/api/site";
import { getSiteList } from "@/api/site";
export const useConsoleStore = defineStore("consoleStore", () => {
  const domainList = ref<Domain[]>();
  const dnsRecordList = ref<DNS[]>();
  const siteList = ref<SiteItem[]>();

  const loadDomains = async () => {
    domainList.value = await getList();
  };
  const loadSiteList = async () => {
    siteList.value = (await getSiteList()) as SiteItem[];
  };

  return {
    domainList,
    siteList,
    dnsRecordList,
    loadDomains,
    loadSiteList,
  };
});
