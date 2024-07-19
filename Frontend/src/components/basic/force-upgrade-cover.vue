<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <div
    class="fixed inset-0 bg-666/80 z-10000 flex justify-center items-center flex-col text-fff space-y-24"
  >
    <svg
      t="1707284787104"
      viewBox="0 0 1024 1024"
      version="1.1"
      xmlns="http://www.w3.org/2000/svg"
      p-id="5967"
      width="200"
      height="200"
    >
      <path
        d="M512 64C264.58 64 64 264.58 64 512s200.58 448 448 448 448-200.58 448-448S759.42 64 512 64z m281.43 729.43a396.67 396.67 0 1 1 85.31-126.53 396.65 396.65 0 0 1-85.31 126.53z"
        fill="#38B487"
        p-id="5968"
      />
      <path
        d="M519 290.91a10 10 0 0 0-14.06 0L379.65 416.23a10 10 0 0 0 7 17h75.53v203.21h99.56V433.21h75.53a10 10 0 0 0 7-17zM462.22 686.22h99.56V736h-99.56z"
        fill="#38B487"
        p-id="5969"
      />
    </svg>
    <div class="flex flex-col items-center">
      <div class="font-bold text-2l">{{ t("common.newVersionTip") }}</div>
      <a href="https://www.kooboo.com/upgrade" class="underline cursor-pointer">
        {{ t("common.pleaseUpgradeTip") }}
      </a>
    </div>
  </div>
</template>
