<template>
  <NoMatchingSite v-if="keywordsLength" />
  <div v-else class="empty-folder">
    <div class="empty-folder__head">
      <img
        class="empty-folder__img"
        src="@/assets/images/empty_state_folder.svg"
      />
    </div>
    <div>
      <div class="empty-folder__title">
        {{ t("common.folderIsEmpty", { folder: folder.key }) }}
      </div>
      <div class="empty-folder__intro">
        {{ t("common.createSiteInFolder") }}
      </div>
      <a class="empty-folder__link" @click="goBack">
        {{ t("common.backToAllSites") }}
      </a>
    </div>
  </div>
</template>
<script lang="ts" setup>
import type { FolderItem } from "../type";

import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import NoMatchingSite from "./no-matching-site.vue";
interface PropsType {
  folder: FolderItem;
  keywordsLength: number;
}
defineProps<PropsType>();

const router = useRouter();
const { t } = useI18n();
function goBack() {
  router.replace({
    name: "home",
  });
}
</script>
<style lang="scss" scoped>
.empty-folder {
  width: 547px;
  margin: 64px auto 0;
  text-align: center;
  &__head {
    border-bottom: 1px solid $main-color;
    .dark & {
      border-bottom: 1px solid gray;
    }
  }
  &__img {
    width: 159px;
    margin-bottom: -1px;
    display: inline-block;
  }
  &__title {
    margin-top: 32px;
    font-weight: bold;
    .dark & {
      color: #666;
    }
  }
  &__intro {
    margin-top: 8px;
    color: #444;
  }
  &__link {
    margin-top: 16px;
    color: $main-blue;
    text-decoration: underline;
    cursor: pointer;
    display: inline-block;
  }
}
</style>
