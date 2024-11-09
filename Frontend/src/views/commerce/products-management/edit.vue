<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import EditForm from "./edit-form.vue";
import { ref } from "vue";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const router = useRouter();
const form = ref();
const id = getQueryString("id");

const onSave = async () => {
  await form.value.onSave();
  goBack();
};

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "product management",
    })
  );
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.productManagement'),
        route: { name: 'product management' },
      },
      { name: t('common.editProduct') },
    ]"
  />

  <EditForm :id="id!" ref="form" />

  <KBottomBar @cancel="goBack" @save="onSave" />
</template>
