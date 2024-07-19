<template>
  <div class="relative h-full w-full">
    <template v-for="item of player?.tabList" :key="item.id">
      <div
        v-show="item.id === player.activeId"
        class="!absolute top-0 left-0 right-0 bottom-0"
      >
        <div
          :ref="(e) => player.setDom(item.id, (e as HTMLElement))"
          class="w-full h-full min-h-300px min-w-400px monaco-editor bg-fff"
        />
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import type { MonacoEditor } from "monaco-recorder";
import { Player } from "monaco-recorder";
import { nextTick, reactive, watch } from "vue";
import * as monaco from "monaco-editor";
import { dark } from "@/composables/dark";

const emit = defineEmits<{
  (e: "ready", value: Player): void;
}>();

const player = reactive(
  new Player(
    monaco,
    (id: string, content: string, lang: string | undefined) => {
      const res = new Promise((resolve) => {
        nextTick(() => {
          resolve(
            monaco.editor.create(player.getDom(id), {
              automaticLayout: true,
              fontSize: 14,
              wordWrap: "on",
              fontFamily:
                "Monaco,Consolas,Lucida Console,Liberation Mono,DejaVu Sans Mono,Bitstream Vera Sans Mono,Courier New, monospace",
              minimap: { enabled: true },
              lineNumbersMinChars: 2,
              readOnly: true,
              model:
                monaco.editor.getModel(
                  monaco.Uri.parse(`file:///${id}?disable_auto_close_tag=true`)
                ) ||
                monaco.editor.createModel(
                  content,
                  lang,
                  monaco.Uri.parse(`file:///${id}?disable_auto_close_tag=true`)
                ),
            })
          );
        });
      });
      return res as Promise<MonacoEditor> | any;
    },
    {
      isSkip: true,
      skipTime: 5 * 1000, // 超过5秒的空白压缩成1秒
    }
  )
);

watch(dark, (dark) => monaco.editor.setTheme(dark ? "vs-dark" : "vs"), {
  immediate: true,
});

emit("ready", player as Player);
</script>
