<script lang="ts" setup>
import { computed, ref } from "vue";
import { getType } from "@/api/template";
import { useTemplateStore } from "@/store/template";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const templateStore = useTemplateStore();

const typeList = ref();

const isLoading = ref(false);

const load = async () => {
  try {
    await templateStore.changePage(templateStore.pagedTemplate.pageNr);
    typeList.value = await getType();
  } catch (error) {
    console.error(error);
  } finally {
    isLoading.value = true;
  }
};

const categories = computed(() => {
  return templateStore?.pagedTemplate?.facets?.find(
    (i) => i.name === "category"
  )?.labels;
});

const colors = computed(() => {
  return templateStore?.pagedTemplate?.facets?.find((i) => i.name === "color")
    ?.labels;
});

load();
</script>

<template>
  <div class="w-full bg-fff dark:bg-[#333] shadow-s-10" :loading="isLoading">
    <header
      class="max-w-1120px mx-auto py-16px dark:text-999 flex items-center gap-16px template-header"
    >
      <el-select
        v-model="templateStore.currentType"
        @update:model-value="templateStore.changePage(1)"
      >
        <el-option
          v-for="item in typeList"
          :key="item.key"
          :label="item.value"
          :value="item.key"
        />
      </el-select>

      <el-select
        v-if="['', 'Site'].includes(templateStore.currentType)"
        class="hidden-input"
      >
        <template #prefix>
          <span
            class="text-16px font-normal text-[#192845] dark:text-[#cfd3dc]"
            >{{ t("common.topics") }}</span
          >
        </template>
        <template #empty>
          <el-checkbox-group
            v-if="categories"
            v-model="templateStore.category"
            class="p-24px flex-1 grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-16px k-checkbox-group"
            @update:model-value="templateStore.changePage(1)"
          >
            <el-checkbox
              v-for="item in categories"
              :key="item.name"
              :label="item.name"
            >
              <div class="flex items-center h-21px">
                <span class="text-blue text-sm font-normal text-#0087C2">{{
                  item.name
                }}</span>
                <span class="ml-4 text-[#8BA5B0] text-sm font-normal"
                  >({{ item.count }})</span
                >
              </div>
            </el-checkbox>
          </el-checkbox-group>

          <el-empty v-else class="w-min-700px" />
        </template>
      </el-select>

      <el-select
        v-if="['', 'Site'].includes(templateStore.currentType)"
        class="hidden-input"
      >
        <template #prefix>
          <span
            class="text-16px font-normal text-[#192845] dark:text-[#cfd3dc]"
            >{{ t("common.color") }}</span
          >
        </template>
        <template #empty>
          <el-checkbox-group
            v-if="colors"
            v-model="templateStore.color"
            class="p-24px flex-1 grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-16px k-checkbox-group"
            @update:model-value="templateStore.changePage(1)"
          >
            <el-checkbox
              v-for="item in colors"
              :key="item.name"
              :label="item.name"
            >
              <div class="flex items-center h-21px">
                <div
                  class="w-[20px] h-[20px] rounded-full"
                  :class="{ 'border border-[#B1C5CE]': item.name == 'White' }"
                  :style="{ background: item.name }"
                />

                <span class="ml-4 text-[#8BA5B0] text-sm font-normal"
                  >({{ item.count }})</span
                >
              </div>
            </el-checkbox>
          </el-checkbox-group>
          <el-empty v-else class="w-min-700px" />
        </template>
      </el-select>
    </header>
  </div>

  <div class="max-w-1120px mx-auto py-16px flex flex-wrap items-center gap-8px">
    <template v-if="templateStore.category.length">
      <div
        v-for="item in templateStore.category"
        :key="item"
        class="h-42px px-[32px] bg-fff dark:bg-[#333] rounded-[21px] border border-[#8CA6B1] flex items-center gap-8px"
      >
        <span class="text-[#8BA5B0] text-m">{{ t("common.topics") }} : </span>

        <span class="text-m text-[#2F2E41] dark:text-[#fffa]">{{ item }}</span>

        <el-icon
          class="iconfont icon-close text-16px cursor-pointer ml-auto text-[#8CA6B1] hover:text-[#2F2E41] dark:text-[#fffa] dark:hover:text-[#fffd]"
          @click="templateStore.removeCategory(item)"
        />
      </div>
    </template>

    <template v-if="templateStore.color.length">
      <div
        v-for="item in templateStore.color"
        :key="item"
        class="h-42px px-[32px] bg-fff dark:bg-[#333] rounded-[21px] border border-[#8CA6B1] flex items-center gap-8px"
      >
        <span class="text-[#8BA5B0] text-m">{{ t("common.color") }} : </span>

        <span class="text-m text-[#2F2E41] dark:text-[#fffa]">{{ item }}</span>

        <el-icon
          class="iconfont icon-close text-16px cursor-pointer ml-auto text-[#8CA6B1] hover:text-[#2F2E41] dark:text-[#fffa] dark:hover:text-[#fffd]"
          @click="templateStore.removeColor(item)"
        />
      </div>
    </template>

    <div
      v-if="templateStore.category.length || templateStore.color.length"
      class="text-sm text-[#8CA6B1] cursor-pointer hover:text-blue ml-[12px] h-[42px] flex items-center"
      @click="templateStore.clearAll()"
    >
      {{ t("common.clearAll") }}
    </div>
  </div>

  <div class="w-full">
    <div
      class="max-w-1120px mx-auto"
      :class="templateStore.pagedTemplate.totalPages > 1 ? '' : 'mb-24'"
    >
      <div
        v-if="templateStore.pagedTemplate.list.length"
        class="w-full grid gap-16 grid-cols-4 py-8"
      >
        <div
          v-for="item of templateStore.pagedTemplate.list"
          :key="item.id"
          class="w-full h-214px"
        >
          <router-link
            class="h-full w-full flex flex-col shadow-s-10 overflow-hidden cursor-pointer transition-all duration-300 hover:(shadow-m-20) rounded-xl"
            :to="{
              name: 'template-detail',
              query: {
                templateId: item.id,
                currentFolder: getQueryString('currentFolder'),
              },
            }"
            data-cy="template"
          >
            <div
              class="dark:bg-666 bg-fff flex-1 bg-no-repeat bg-cover"
              :style="{
                backgroundImage: `url(${item.thunbnailUrl})`,
              }"
            />

            <div class="bg-fff dark:bg-666 dark:text-fff p-8">
              <div class="px-4 py-4 flex justify-between items-center">
                <span
                  class="text-m max-w-150px ellipsis"
                  :title="item.name"
                  data-cy="template-name"
                  >{{ item.name }}</span
                >
                <div>
                  <el-icon
                    class="iconfont icon-xiazai-wenjianxiazai-05 h-full"
                  />
                  <span
                    class="ml-4 text-s text-666 dark:text-fff"
                    data-cy="downloads"
                    >{{ item.downloadCount }}</span
                  >
                </div>
              </div>
            </div>
          </router-link>
        </div>
      </div>
      <div
        v-else
        class="flex items-center justify-center mt-100px"
        data-cy="no-matching-template"
      >
        <div>
          <img
            src="@/assets/images/no_template@2x.png"
            class="w-360px h-230px"
          />
          <div class="text-center text-m text-999 mt-36px">
            {{ t("common.noTemplate") }}
          </div>
        </div>
      </div>
    </div>
  </div>

  <div v-if="templateStore.pagedTemplate.totalPages > 1" class="my-24">
    <el-pagination
      layout="prev, pager, next,jumper"
      :page-size="templateStore.pagedTemplate.pageSize"
      :page-count="templateStore.pagedTemplate.totalPages"
      :current-page="templateStore.pagedTemplate.pageNr"
      @current-change="templateStore.changePage"
    />
  </div>
</template>

<style lang="scss">
.template-header .el-select {
  width: 150px;

  &.hidden-input {
    .el-input__inner {
      visibility: hidden;
    }
  }

  .el-input__inner {
    font-size: 16px;
    font-weight: normal;
    @apply text-[#192845] dark:text-[#cfd3dc];
  }

  .el-input__suffix {
    &::before {
      content: "";
      display: none;
    }

    .el-select__caret {
      color: #606266;
    }
  }
}

.k-checkbox-group {
  .el-checkbox__inner {
    border-color: #dbdbdb;
  }

  .el-checkbox__inner:hover {
    background-color: transparent;
    border-color: $main-blue;
  }

  .is-checked .el-checkbox__inner:hover {
    background-color: $main-blue;
  }

  .el-checkbox__input {
    transform: scale(1.143);
  }
}
</style>
