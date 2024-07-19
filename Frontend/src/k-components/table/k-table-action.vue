<script lang="ts" setup>
import { isRef } from "vue";
import { getCurrentInstance } from "vue";
import type { Action } from ".";

interface Props {
  get?: string;
  post?: string;
  to?: string;
  confirm?: string;
  label?: string;
}

const props = defineProps<Props>();
const actions = getCurrentInstance()?.parent?.exposed?.actions;

if (actions && isRef(actions)) {
  (actions.value as Action[]).push({
    confirm: props.confirm,
    get: props.get,
    post: props.post,
    to: props.to,
    label: props.label,
  });
}
</script>
