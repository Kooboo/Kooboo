<script lang="ts" setup>
import { site } from "./settings";
import CodeLog from "./code-log.vue";
import ImageBrowserCache from "./image-browser-cache.vue";
import Lighthouse from "./lighthouse.vue";
import Pwa from "./pwa.vue";
import Multilingual from "./multilingual.vue";
import Sitemap from "./sitemap.vue";
import { useI18n } from "vue-i18n";
import RichTextEditorConfig from "./editor.vue";
import VisitorCountryRestriction from "./visitor-country-restriction.vue";
import { useSiteStore } from "@/store/site";
import { useAppStore } from "@/store/app";
const { t } = useI18n();

const siteStore = useSiteStore();
const appStore = useAppStore();
</script>

<template>
  <div
    v-if="site"
    class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px"
  >
    <div class="max-w-504px">
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.visitorLog")
        }}</span>
        <div class="flex-1" />
        <el-switch v-model="site.enableVisitorLog" data-cy="visitor-log" />
      </el-form-item>
      <VisitorCountryRestriction v-if="appStore.header?.isOnlineServer" />
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{ t("common.sqlLog") }}</span>
        <div class="flex-1" />
        <el-switch v-model="site.enableSqlLog" data-cy="sql-log" />
      </el-form-item>
    </div>
    <CodeLog />
    <div class="max-w-504px">
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.constraintFixOnSave")
        }}</span>
        <Tooltip
          :tip="t('common.constraintFixOnSaveTips')"
          custom-class="ml-4"
        />

        <div class="flex-1" />
        <el-switch
          v-model="site.enableConstraintFixOnSave"
          data-cy="constraint-fix-on-save"
        />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.frontEvent")
        }}</span>
        <Tooltip :tip="t('common.frontEventTips')" custom-class="ml-4" />
        <div class="flex-1" />
        <el-switch v-model="site.enableFrontEvents" data-cy="front-event" />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.JsCssCompression")
        }}</span>
        <div class="flex-1" />
        <el-switch
          v-model="site.enableJsCssCompress"
          data-cy="js-css-compress"
        />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.JsCssBrowserCache")
        }}</span>
        <div class="flex-1" />
        <el-switch
          v-model="site.enableJsCssBrowerCache"
          data-cy="js-css-cache"
        />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">
          {{ t("common.videoBrowserCache") }}
        </span>
        <div class="flex-1" />
        <el-switch
          v-model="site.enableVideoBrowserCache"
          data-cy="video-cache"
        />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">
          {{ t("common.htmlCompression") }}
        </span>
        <div class="flex-1" />
        <el-switch v-model="site.enableHtmlMinifier" data-cy="html-compress" />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">
          {{ t("common.enableImageAlt") }}
        </span>
        <div class="flex-1" />
        <el-switch v-model="site.enableImageAlt" data-cy="image-alt" />
      </el-form-item>
    </div>
    <ImageBrowserCache />
    <Lighthouse />
    <Pwa />
    <Multilingual />
    <RichTextEditorConfig />
    <Sitemap />
    <div class="max-w-504px">
      <el-form-item>
        <span class="font-bold dark:text-fff/86">CORS</span>
        <Tooltip :tip="t('common.corsTips')" custom-class="ml-4" />
        <div class="flex-1" />
        <el-switch v-model="site.enableCORS" data-cy="cors" />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">SPA</span>
        <Tooltip :tip="t('common.spaTips')" custom-class="ml-4" />
        <div class="flex-1" />
        <el-switch v-model="site.enableSPA" data-cy="spa" />
      </el-form-item>
      <el-form-item>
        <template #label>
          <span class="font-bold dark:text-fff/86 text-black">{{
            t("common.additionalCodeSuggestions")
          }}</span>
        </template>
        <el-select v-model="site.codeSuggestions" multiple class="w-full">
          <el-option value="bootstrap-v3">bootstrap-v3</el-option>
          <el-option value="tailwind-v3.1">tailwind-v3.1</el-option>
          <el-option value="element-plus-v2.2">element-plus-v2.2</el-option>
          <el-option value="amp-v0">amp-v0</el-option>
        </el-select>
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.recordSiteLogVideo")
        }}</span>
        <div class="flex-1" />
        <el-switch v-model="site.recordSiteLogVideo" data-cy="force-ssl" />
      </el-form-item>
      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.forceSSL")
        }}</span>
        <Tooltip :tip="t('common.forceSSLTips')" custom-class="ml-4" />
        <div class="flex-1" />
        <el-switch v-model="site.forceSSL" data-cy="force-ssl" />
      </el-form-item>

      <el-form-item v-if="site.continueDownload !== undefined">
        <span class="font-bold dark:text-fff/86">{{
          t("common.continueDownload")
        }}</span>
        <Tooltip :tip="t('common.continueDownloadTips')" custom-class="ml-4" />
        <div class="flex-1" />
        <el-switch
          v-model="site.continueDownload"
          data-cy="continue-download"
        />
      </el-form-item>

      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.enableUpdateSimilarPage")
        }}</span>
        <div class="flex-1" />
        <el-switch v-model="site.enableUpdateSimilarPage" data-cy="force-ssl" />
      </el-form-item>

      <el-form-item>
        <span class="font-bold dark:text-fff/86">{{
          t("common.automateCovertImageToWebp")
        }}</span>
        <div class="flex-1" />
        <el-switch v-model="site.automateCovertImageToWebp" />
      </el-form-item>
      <el-form-item
        v-if="
          !appStore.header?.isOnlineServer || appStore.header.isPrivateServer
        "
      >
        <span class="font-bold dark:text-fff/86">{{
          t("common.enableResourceCache")
        }}</span>
        <Tooltip
          :tip="t('common.enableResourceCacheTip')"
          custom-class="ml-4"
        />
        <div class="flex-1" />
        <el-switch v-model="site.enableResourceCache" />
      </el-form-item>
      <template
        v-if="
          appStore.header?.isOnlineServer && !appStore.header.isPrivateServer
        "
      >
        <el-form-item v-if="siteStore.serviceLevel > 0">
          <span class="font-bold dark:text-fff/86">{{
            t("common.enableDevPassword")
          }}</span>
          <div class="flex-1" />
          <el-switch
            :model-value="site.status == 'Development'"
            @update:model-value="
              site.status =
                site.status == 'Development' ? 'Published' : 'Development'
            "
          />
        </el-form-item>
        <el-form-item v-if="site.status == 'Development'">
          <span class="font-bold dark:text-fff/86">{{
            t("common.devPassword")
          }}</span>
          <div class="flex-1" />
          <el-input
            v-model="site.devPassword"
            :disabled="true"
            data-cy="dev-password"
          />
        </el-form-item>
      </template>
    </div>
  </div>
</template>
