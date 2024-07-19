import request from "@/utils/request";
import type {
  DataCenter,
  Organization,
  OrganizationList,
  UsersList,
} from "../organization/types";
import type { IAddUserParam, IDeleteUserParam } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export function getOrganizations() {
  return request.get<OrganizationList[]>("organization/getOrganizations");
}

export function userDepartOrg(orgId: string) {
  return request.get("organization/UserLeaveOrg", { orgId });
}

export function createOrgUser(model: any) {
  return request.post("organization/CreateOrgUser", model);
}

export function getOrg() {
  return request.get<Organization>("organization/getUserOrg");
}
export function createOwnOrg() {
  return request.get("organization/CreateOwnOrg");
}

export function getUsers(params = {}) {
  return request.get<UsersList[]>("organization/getUsers", params);
}

export function changeUserOrg(organizationId: string) {
  return request.post(
    "organization/changeUserOrg",
    {
      organizationId,
    },
    undefined,
    {
      successMessage: $t("common.organizationChangingSuccess"),
    }
  );
}

export function getCurrentCenter() {
  return request.get("organization/DataCenterList");
}

export function getCurrentDataCenter() {
  return request.get("DataCenter/CurrentDC");
}

export function getDataCenterList() {
  return request.get<DataCenter[]>("DataCenter/List");
}

export function enableServer(name: string) {
  return request.post("DataCenter/Enable", { name }, undefined, {
    successMessage: $t("common.enableDataCenterTips"),
  });
}

export function disableServer(name: string) {
  return request.post("DataCenter/disable", { name }, undefined, {
    successMessage: $t("common.disableDataCenterTips"),
  });
}

export function makeDefault(name: string) {
  return request.post("DataCenter/MakeDefault", { name }, undefined, {
    successMessage: $t("common.defaultDataCenterModifiedTips"),
  });
}
export function ChangeDataCenter(OrganizationId: string, DataCenterId: string) {
  return request.get("organization/ChangeDataCenter", {
    OrganizationId,
    DataCenterId,
  });
}

export function addUser(data: IAddUserParam) {
  return request.post("organization/addUser", data, undefined, {
    successMessage: $t("common.addSuccess"),
  });
}

export function deleteUser(data: IDeleteUserParam) {
  return request.post("organization/deleteUser", data, undefined, {
    successMessage: $t("common.removeSuccess"),
  });
}
