<script lang="ts" setup>
import { updateBase64 } from "@/api/module/files";
import { ImageEditor } from "@/components/k-media-dialog";
import { onMounted, ref, watch } from "vue";

const emit = defineEmits<{
  (e: "changed", value: boolean): void;
}>();

const props = defineProps<{
  id: string;
  name: string;
  params: { url: string; type: string; moduleId: string };
}>();

const newImageUrl = ref();
const media = ref();
onMounted(() => {
  const mediaObject = { ...props, ...props.params };
  media.value = mediaObject;
});

const save = async () => {
  await updateBase64(
    props.params.type,
    props.params.moduleId,
    props.name,
    newImageUrl.value.split("base64,")[1]
  );
  emit("changed", false);
};
watch(
  () => newImageUrl.value,
  () => {
    emit("changed", true);
  }
);
defineExpose({ save });
</script>

<template>
  <ImageEditor
    v-if="media"
    v-model:base64="newImageUrl"
    :name="media.name"
    :url="media.url"
  />
</template>
