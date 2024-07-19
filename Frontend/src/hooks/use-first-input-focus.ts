import { onMounted } from "vue";

export function useFirstInputFocus(index?: number) {
  onMounted(() => {
    if (index) {
      document.querySelectorAll("input")?.[index]?.focus();
    } else {
      document.querySelector("input")?.focus();
    }
  });
}
