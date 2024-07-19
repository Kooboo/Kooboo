import { getQueryString } from "@/utils/url";

const maxCount = 8;

interface RecentVisits {
  name: string;
  url: string;
  icon?: string;
  count: number;
  lastVisit: number;
}

export function getRecentlyVisits() {
  const siteId = getQueryString("siteId");
  let list: RecentVisits[] = [];
  const json = localStorage.getItem(`${siteId}_RecentlyVisited`);
  if (json) list = JSON.parse(json);
  return list;
}

export function countRecentVisits(name: string, url: string, icon?: string) {
  const siteId = getQueryString("siteId");
  if (!siteId) return;
  let list = getRecentlyVisits();
  const json = localStorage.getItem(`${siteId}_RecentlyVisited`);
  if (json) list = JSON.parse(json);
  const item = list.find((f) => f.name == name);
  const now = new Date().getTime();
  if (item) {
    item.count++;
    item.lastVisit = now;
  } else {
    list.push({
      name,
      url,
      icon,
      count: 1,
      lastVisit: now,
    });
  }

  list = list.sort((a, b) => {
    const aLastVisit = a.lastVisit || now;
    const bLastVisit = b.lastVisit || now;
    let aDay = (now - aLastVisit) / 86400000;
    if (aDay < 1) aDay = 1;
    let bDay = (now - bLastVisit) / 86400000;
    if (bDay < 1) bDay = 1;
    return b.count / bDay - a.count / aDay;
  });

  if (list.length > maxCount) {
    list = list.slice(0, maxCount);
  }

  localStorage.setItem(`${siteId}_RecentlyVisited`, JSON.stringify(list));
}
