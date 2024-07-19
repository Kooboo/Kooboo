import { deleteTag, getTags } from "@/api/commerce/tag";
import { ref } from "vue";

export function useTag(type: string) {
  const tags = ref<string[]>([]);

  async function loadTags() {
    tags.value = await getTags(type);
  }

  loadTags();

  async function removeTag(name: string) {
    await deleteTag(type, name);
    loadTags();
  }

  return {
    tags,
    loadTags,
    removeTag,
  };
}
