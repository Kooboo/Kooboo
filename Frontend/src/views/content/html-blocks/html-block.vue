<script lang="ts" setup>
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { ref } from "vue";
import { useRouter } from "vue-router";
import HtmlblockForm from "./htmlblock-form.vue";

const router = useRouter();
const form = ref();
const id = getQueryString("id");

async function save() {
  await form.value.save();
  goBack();
}
function goBack() {
  router.goBackOrTo(useRouteSiteId({ name: "htmlblocks" }));
}
</script>

<template>
  <div class="p-32 pb-150px">
    <div
      class="rounded-normal bg-fff dark:bg-[#333] py-24 px-56px"
      style="min-height: calc(100vh - 180px)"
    >
      <HtmlblockForm :id="id!" ref="form" />
    </div>
  </div>
  <KBottomBar
    :permission="{
      feature: 'htmlBlock',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
