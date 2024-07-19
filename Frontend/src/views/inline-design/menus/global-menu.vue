<script lang="ts" setup>
import { ref, computed } from "vue";
import { getScreenSize } from "../utils/dom";
import { useI18n } from "vue-i18n";
import {
  saveChanges,
  historyCount,
  stashCount,
  undo,
  redo,
} from "@/views/inline-design/state";
import { editing } from "@/views/inline-design/inline-editor";
import { showColorDialog } from "@/views/inline-design/color";
import { showImageDialog } from "@/views/inline-design/image/global-image-dialog";
import { showLinkDialog } from "@/views/inline-design/link/global-link-dialog";
import { showTextDialog } from "@/views/inline-design/text/global-text-dialog";
import { getQueryString } from "@/utils/url";
import { pageWidths, switchWidth, width } from "@/views/inline-design/page";
import Draggable from "@/components/basic/draggable.vue";
import { doc } from "@/views/inline-design/page";
import PageStyleDialog from "../widget/style-editor-dialog.vue";
import type { PostPage } from "@/api/pages/types";

const props = defineProps<{
  page?: PostPage;
}>();

const { t } = useI18n();
const styleDialogRef = ref();
const handleElement = ref<HTMLElement>();
const screenSize = getScreenSize();

const isNormalDesignPage = computed(() => {
  return props.page?.type === "Designer" && !props.page?.layoutName;
});

const save = async () => {
  const path = getQueryString("path")!;
  await saveChanges(path);
  setTimeout(() => {
    location.reload();
  }, 500);
};

function editGlobalStyles() {
  if (!props.page) return;
  styleDialogRef.value?.init(props.page);
}

const emit = defineEmits<{
  (e: "save", value: PostPage): void;
}>();
</script>

<template>
  <Draggable
    v-if="!editing"
    :handler="handleElement!"
    :init="{ x: screenSize.x - 100, y: 32 }"
  >
    <div class="space-y-8">
      <div ref="handleElement">
        <el-button size="large" plain circle class="cursor-move">
          <el-icon class="iconfont icon-move" />
        </el-button>
      </div>

      <div>
        <el-popover placement="left" trigger="click">
          <template #reference>
            <div @click.stop="">
              <el-tooltip
                :content="t('common.toggleScreenSize')"
                placement="auto"
              >
                <el-button size="large" plain circle>
                  <el-icon class="iconfont icon-database" />
                </el-button>
              </el-tooltip>
            </div>
          </template>
          <div>
            <div
              v-for="item of pageWidths"
              :key="item.name"
              class="py-4 px-8 hover:bg-blue/10 rounded-4px cursor-pointer"
              :class="width.name == item.name ? 'text-blue' : ''"
              @click="switchWidth(item.name)"
            >
              {{ item.display }}
            </div>
          </div>
        </el-popover>
      </div>

      <div v-if="isNormalDesignPage">
        <el-tooltip :content="t('common.styles')" placement="auto">
          <el-button size="large" plain circle @click="editGlobalStyles">
            <el-icon class="iconfont icon-page" />
          </el-button>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.colors')" placement="auto">
          <el-button
            size="large"
            plain
            circle
            @click="showColorDialog(doc?.documentElement)"
          >
            <el-icon class="iconfont icon-background-color" />
          </el-button>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.images')" placement="auto">
          <el-button size="large" plain circle @click="showImageDialog()">
            <el-icon class="iconfont icon-photo" />
          </el-button>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.links')" placement="auto">
          <el-button size="large" plain circle @click="showLinkDialog()">
            <el-icon class="iconfont icon-link1" />
          </el-button>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.text')" placement="auto">
          <el-button size="large" plain circle @click="showTextDialog()">
            <el-icon class="iconfont icon-menu" />
          </el-button>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.undo')" placement="auto">
          <el-badge
            :value="historyCount"
            :hidden="!historyCount"
            class="!dark:border-none"
          >
            <el-button
              size="large"
              plain
              circle
              :disabled="!historyCount"
              @click="undo()"
            >
              <el-icon class="iconfont icon-go-back" />
            </el-button>
          </el-badge>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.redo')" placement="auto">
          <el-badge :value="stashCount" :hidden="!stashCount">
            <el-button
              size="large"
              plain
              circle
              :disabled="!stashCount"
              @click="redo()"
            >
              <el-icon class="iconfont icon-go-back transform rotate-y-180" />
            </el-button>
          </el-badge>
        </el-tooltip>
      </div>

      <div>
        <el-tooltip :content="t('common.save')" placement="auto">
          <el-button
            size="large"
            plain
            circle
            :disabled="!historyCount"
            @click="save"
          >
            <el-icon class="iconfont icon-preservation" />
          </el-button>
        </el-tooltip>
      </div>
    </div>
  </Draggable>
  <PageStyleDialog ref="styleDialogRef" @save="emit('save', $event)" />
</template>
