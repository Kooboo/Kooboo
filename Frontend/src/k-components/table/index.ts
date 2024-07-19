import KTable from "./k-table.vue";
import KTableAction from "./k-table-action.vue";

export default { KTable, KTableAction };

export interface Action {
  get?: string;
  post?: string;
  to?: string;
  confirm?: string;
  label?: string;
}
