<template>
  <div class="prop-header !mt-0 dark:bg-gray">
    <div class="text-xs text-bold">
      {{ t("ve.contentProperties") }}
    </div>
    <div class="flex justify-evenly items-center">
      <div
        class="w-10 h-11 icon-color cursor-pointer flex justify-center items-center hover:bg-999 hover:text-fff"
        @click="resetHolder"
      >
        <el-tooltip
          effect="dark"
          :content="t('common.close')"
          placement="bottom"
        >
          <svg
            width="20"
            viewBox="0 0 1024 1024"
            xmlns="http://www.w3.org/2000/svg"
            data-v-029747aa=""
          >
            <path
              fill="currentColor"
              d="M831.872 340.864 512 652.672 192.128 340.864a30.592 30.592 0 0 0-42.752 0 29.12 29.12 0 0 0 0 41.6L489.664 714.24a32 32 0 0 0 44.672 0l340.288-331.712a29.12 29.12 0 0 0 0-41.728 30.592 30.592 0 0 0-42.752 0z"
            />
          </svg>
        </el-tooltip>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { resetHolder } from "../ve-content/effects";

const { t } = useI18n();
</script>

<style lang="scss" scoped>
.prop-header {
  width: 100%;
  height: 2.75rem;
  padding: 0 1.25rem;
  margin-top: 1.25rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: white;
  color: #8f9a99;
  font-size: 0.75rem;
  font-weight: bold;
  border-top: 1px solid #e1e4e7;
  border-bottom: 1px solid #e1e4e7;
  box-shadow: 0 -2px 6px rgb(0 0 0 / 20%);
}
</style>
