<script setup lang="ts">
/* eslint-disable vue/no-v-html */
// https://gist.github.com/kcak11/4a2f22fb8422342b3b3daa7a1965f4e4#file-countries-json
import codes from "@/assets/static/country-and-region-codes.json";
import { useElementBounding, useVModel } from "@vueuse/core";
import { computed, ref } from "vue";
import { CaretBottom, Search } from "@element-plus/icons-vue";
import groupBy from "lodash/groupby";
import { onClickOutside } from "@vueuse/core";
import { useI18n } from "vue-i18n";

interface Props {
  modelValue: string;
  regionCode: string;
}

interface Emits {
  (event: "update:modelValue"): void;
  (event: "update:regionCode"): void;
}

const { locale, t } = useI18n();
const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const vModel = useVModel(props, "modelValue", emit);
const regionCode = useVModel(props, "regionCode", emit);

const regionCodeFilter = ref("");
const hightLight = (str: string, key: string | RegExp) => {
  if (typeof key === "string") {
    return str.replace(
      key.includes("+") ? key : RegExp(key, "i"),
      (val: any) => `<span style="color:rgba(34, 150, 243, 1)">${val}</span>`
    );
  }
};

const filteredRegionCodes = computed(() => {
  const filter = regionCodeFilter.value.toLocaleLowerCase();
  const filtered: (typeof codes[any] & {
    highlighted: Partial<typeof codes[any]>;
  })[] = [];

  for (const i of codes) {
    for (const k in i) {
      if (k === "nameCN" && locale.value !== "zh") continue;
      if (i[k as keyof typeof i].toLocaleLowerCase().includes(filter)) {
        filtered.push({
          ...i,
          highlighted: {
            [k]: hightLight(i[k as keyof typeof i], filter),
          },
        });
        break;
      }
    }
  }
  return filtered;
});

const groupedCodesPY = ref();

if (localStorage.getItem("lang") === "zh") {
  groupedCodesPY.value = groupBy(
    codes.sort((a, b) => a.namePY.charCodeAt(0) - b.namePY.charCodeAt(0)),
    (i) => i.namePY[0].toUpperCase()
  );
} else {
  groupedCodesPY.value = groupBy(
    codes.sort((a, b) => a.name.charCodeAt(0) - b.name.charCodeAt(0)),
    (i) => i.name[0].toUpperCase()
  );
}

const showCodesMenu = ref(false);
const codesMenu = ref<HTMLDivElement | null>(null);
const inputContainer = ref<HTMLDivElement | null>(null);
const showButton = ref<HTMLButtonElement | null>(null);

const { width, height } = useElementBounding(inputContainer);

onClickOutside(codesMenu, () => (showCodesMenu.value = false), {
  ignore: [showButton],
});

const currentDialCode = computed(() =>
  codes.find(({ dialCode }) => dialCode === regionCode.value)
);
</script>

<template>
  <div ref="inputContainer" class="relative w-full">
    <el-input v-model="vModel" maxlength="11" v-bind="$attrs">
      <template #prepend>
        <div class="text-444 dark:text-fff/60 outline-none font-bold">
          <button
            ref="showButton"
            class="flex items-center justify-center px-4 w-66px transition hover:bg-[#eaf4fc] dark:hover:bg-444 h-32px rounded-5px"
            type="button"
            :class="showCodesMenu ? 'bg-[#eaf4fc] dark:bg-444' : ''"
            data-cy="region-code-btn"
            @click="showCodesMenu = !showCodesMenu"
          >
            +{{ +regionCode }}
            <el-icon
              class="transform ml-4"
              :class="showCodesMenu ? 'rotate-180' : 'rotate-0'"
            >
              <CaretBottom />
            </el-icon>
          </button>
        </div>
      </template>
    </el-input>
  </div>

  <div
    v-show="showCodesMenu"
    ref="codesMenu"
    class="absolute z-9999 flex flex-col items-center w-full h-440px p-10px shadow-m-10 bg-fff dark:bg-[#222] dark:text-fff/86 border-1 border-gray dark:border-transparent border-solid mt-1 rounded-normal z-10 font-family text-14px leading-32px overflow-hidden"
    :style="{
      width: width + 'px',
      top: height + 'px',
    }"
    data-cy="region-code-dialog"
  >
    <div class="sticky top-0 w-full h-fit">
      <el-input
        v-model="regionCodeFilter"
        type="text"
        class="w-full border-gray dark:border-transparent border-1 rounded-normal region-code-input"
        :placeholder="t('common.search')"
        clearable
        :prefix-icon="Search"
        data-cy="region-code-search"
      />
      <button
        v-if="currentDialCode"
        class="flex flex-row mt-10px items-center justify-between w-full px-10px rounded-normal hover:bg-blue-10 dark:hover:bg-444 text-left"
        type="button"
        data-cy="selected-region"
        @click="
            regionCode = currentDialCode!.dialCode;
            showCodesMenu = false;
          "
      >
        <span class="font-bold" data-cy="country">
          {{ locale === "en" ? currentDialCode!.name : currentDialCode!.nameCN }}
        </span>
        <span
          class="font-light text-999 dark:text-fff/60"
          data-cy="region-code"
        >
          {{ currentDialCode!.dialCode }}
        </span>
      </button>
      <el-divider class="my-10px" />
    </div>
    <el-scrollbar class="h-400px w-full">
      <template v-if="!regionCodeFilter.length">
        <template v-for="(v, k) in groupedCodesPY" :key="k">
          <h3 class="font-bold py-5px px-10px">{{ k }}</h3>

          <ul>
            <li v-for="{ nameCN, isoCode, dialCode, name } in v" :key="isoCode">
              <button
                class="flex flex-row items-center justify-between w-full px-10px rounded-normal hover:bg-blue-10 dark:hover:bg-444 text-left"
                type="button"
                data-cy="region-item"
                @click="
                  regionCode = dialCode;
                  showCodesMenu = false;
                "
              >
                <span class="font-normal" data-cy="country">
                  {{ locale === "en" ? name : nameCN }}
                </span>
                <span
                  class="font-light text-999 dark:text-fff/86"
                  data-cy="region-code"
                >
                  {{ dialCode }}
                </span>
              </button>
            </li>
          </ul>
        </template>
      </template>
      <template v-else-if="filteredRegionCodes.length">
        <ul>
          <li
            v-for="{
              isoCode,
              dialCode,
              highlighted,
              name,
              nameCN,
            } in filteredRegionCodes"
            :key="isoCode"
            data-cy="region-code-search-results"
          >
            <button
              class="flex flex-row items-center justify-between w-full px-10px rounded-normal hover:bg-blue-10 dark:hover:bg-444 text-left"
              type="button"
              @click="
                regionCode = dialCode;
                showCodesMenu = false;
              "
            >
              <span class="ellipsis font-bold">
                <span
                  v-if="locale === 'en'"
                  class="ellipsis"
                  v-html="highlighted.name ?? name"
                />
                <span
                  v-else
                  data-cy="country"
                  v-html="highlighted.nameCN ?? nameCN"
                />
                <span
                  v-if="highlighted.isoCode"
                  class="ml-4px font-light text-10px"
                  v-html="highlighted.isoCode"
                />
                <span
                  v-if="locale !== 'en' && highlighted.name"
                  class="ml-4px font-light text-10px ellipsis"
                  data-cy="region-code"
                  v-html="highlighted.name"
                />
              </span>
              <span
                class="font-light ml-8px text-999"
                v-html="highlighted.dialCode ?? dialCode"
              />
            </button>
          </li>
        </ul>
      </template>
      <template v-else
        ><span class="flex justify-center">{{ t("common.noData") }} </span>
      </template>
    </el-scrollbar>
  </div>
</template>

<style lang="scss" scoped>
/* Firefox */
input[type="number"] {
  -moz-appearance: textfield;
}

// https://stackoverflow.com/a/20941546/14835397
input::-webkit-calendar-picker-indicator {
  position: absolute;
  left: 9999px;
}

:deep(.el-input-group__prepend) {
  padding: 4px;
  border-top-left-radius: 8px;
  border-bottom-left-radius: 8px;
  background-color: transparent;
}
</style>
