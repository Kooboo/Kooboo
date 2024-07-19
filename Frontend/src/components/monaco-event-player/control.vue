<template>
  <div
    class="w-full flex items-center text-xs px-[16px] bg-[rgba(216,216,216,.3)] dark:bg-[#333]"
    style="height: 40px; white-space: nowrap"
  >
    <div
      v-if="player"
      class="flex items-center w-full h-full text-[#1d2b4f] dark:text-fff/50"
    >
      <slot name="before" />
      <!-- 播放暂停 -->
      <div class="flex items-center gap-[16px] text-666 dark:text-fff/50">
        <span
          v-if="player?.prev"
          class="icon-left iconfont text-[20px] cursor-pointer hover:text-blue player-btn"
          @click="player?.prev()"
        />
        <span
          class="iconfont text-[20px] cursor-pointer hover:text-blue"
          :class="player.playStatus ? 'icon-stop' : 'icon-play1'"
          @click="
            () => {
              if (!player) return;
              if (player.currentTime >= player.totalTime && !player?.pollStatus)
                player.jumpToStart();
              player.toggle();
            }
          "
        />
        <span
          v-if="player?.prev"
          class="icon-next iconfont text-[20px] cursor-pointer hover:text-blue player-btn"
          @click="player?.next()"
        />
      </div>

      <!-- 当前时间 -->
      <div class="px-[16px]">{{ player.formatTime(player.currentTime) }}</div>

      <!-- 进度条 -->
      <div
        class="flex-1 h-full w-full flex flex-auto items-center cursor-pointer relative"
        @click.capture="jumpTime"
      >
        <!-- 遮罩 -->
        <div
          class="w-full h-full absolute z-50"
          @mousemove.self="hoverPointer.handleMove"
          @mouseleave.self="hoverPointer.hide"
          @mouseover.self="hoverPointer.show"
        />
        <!-- 指针 -->
        <div
          v-show="hoverPointer.isShow"
          class="bg-blue absolute h-full w-[2px] transform -translate-x-[50%] pointer-events-none z-40"
          :style="{ left: `${hoverPointer.offsetX}px` }"
        >
          <span
            class="absolute px-4 top-0 text-[12px] text-blue transform rounded-full"
            :class="{ '-translate-x-[100%]': hoverPointer.isLeft }"
            >{{ player.formatTime(hoverPointer.time) }}</span
          >
        </div>
        <!-- 进度条背景 -->
        <div
          class="w-full h-[5px] absolute z-20 rounded-full overflow-hidden"
          style="background-color: rgba(166, 166, 166, 0.5)"
        />
        <!-- 进度条进度 -->
        <div
          class="h-[5px] bg-[#55b7f1] absolute z-30 rounded-full overflow-hidden"
          :style="{
            width: (player.getProgress(player.currentTime) || 0) + '%',
          }"
        />

        <!-- 自定义内容 -->
        <div class="w-full h-full flex items-center absolute z-30">
          <slot />
        </div>
        <!-- 事件密度 -->
        <div
          class="w-full h-[70%] flex items-center absolute z-10 opacity-50 list-none"
        >
          <li
            v-for="(item, index) in player.highProgressBar"
            :key="index"
            class="bg-[#55b7f1]"
            style="margin-left: 0.1%; width: 0.4%"
            :style="{ height: `${item}%` }"
          />
        </div>
      </div>

      <!-- 总时间 -->
      <div class="px-[16px]">{{ player.formatTime(player.totalTime) }}</div>

      <slot name="toolbar" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from "vue";
import type { Player } from "monaco-recorder";

const props = defineProps<{
  player: Player;
}>();

const emit = defineEmits<{
  (e: "changeTime", value: number): void;
}>();

function jumpTime(e: MouseEvent) {
  if (!props.player) return;
  let distance = e.offsetX / (e.target as Element).clientWidth;
  if (distance < 0) distance = 0;
  if (distance > 1) distance = 1;
  let time = distance * props.player.totalTime;
  if (time < 0) time = 0;
  emit("changeTime", time);
}
// 悬浮指针
const hoverPointer = reactive({
  isShow: false,
  offsetX: 0,
  time: 0,
  isLeft: false, // 时间是否在左边 时间超过70%放左边
  show() {
    hoverPointer.isShow = true;
  },
  hide() {
    hoverPointer.isShow = false;
  },
  handleMove(e: MouseEvent) {
    hoverPointer.offsetX = e.offsetX;
    var distance = e.offsetX / (e.target as Element).clientWidth;
    if (distance < 0) distance = 0;
    if (distance > 1) distance = 1;
    hoverPointer.isLeft = distance > 0.7;
    hoverPointer.time = distance * props.player.totalTime;
  },
});
</script>

<style>
.preview-player.zoom-1 .player-btn {
  display: none;
}
</style>
