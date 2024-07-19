import request from "@/utils/request";

export interface Department {
  id: string;
  name: string;
  displayName: string;
}

export function getDepartments() {
  return request.get<Department[]>("department/list");
}

export function getDepartmentUsers(id: string) {
  return request.get<Department[]>("department/users", { id });
}

export function addUser(model: any) {
  return request.post("department/addUser", model);
}

export function addDepartment(model: any) {
  return request.post("department/post", model);
}

export function deleteDepartments(ids: string[]) {
  return request.post("department/deletes", { ids });
}

export function deleteUsers(departmentId: string, userNames: string[]) {
  return request.post("department/deleteUsers", { userNames, departmentId });
}
