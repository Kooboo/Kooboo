<template>
  <div
    v-show="previewPlayer.previewId"
    id="previewPlayer"
    class="preview-player bg-[#f3f3f3] dark:bg-[#1e1e1e] dark:text-[#424242] border-1 border-[#e1e2e8] dark:border-444 flex flex-col overflow-hidden z-1000"
    :class="`zoom-${previewPlayer.zoom} ${
      previewPlayer.fullScreen ? 'full-screen' : ''
    }`"
    @contextmenu.stop.prevent="previewPlayer.unload"
    @mousewheel="previewPlayer.handleScroll"
    @mousedown="previewPlayer.moveDialog"
  >
    <div
      class="h-22px flex items-center justify-end bg-[rgba(216,216,216,.3)] dark:bg-[#333] text-m px-2 text-[#1d2b4f] dark:text-fff/50"
    >
      <span
        class="iconfont icon-close cursor-pointer hover:text-blue"
        @click="previewPlayer.unload"
      />
    </div>

    <div
      class="flex-1 preview-player-el overflow-hidden"
      :class="{ 'no-event': !previewPlayer.previewPointerEvents }"
    >
      <EventPlayerView
        :key="previewPlayer.previewId"
        @ready="previewPlayer.previewPlayerReady"
      />
    </div>

    <div v-if="previewPlayer.el" class="relative w-full preview-player-control">
      <PlayerControl
        :player="(previewPlayer.el as Player)"
        @change-time="previewPlayer.el.jumpToTime($event)"
      >
        <template #toolbar>
          <div class="flex items-center space-x-[16px]">
            <el-icon
              class="cursor-pointer hover:text-blue"
              aria-hidden="true"
              @click="previewPlayer.setFullScreen()"
            >
              <FullScreen />
            </el-icon>

            <!-- 缩放 -->
            <div
              v-if="!previewPlayer.fullScreen"
              class="hidden cursor-pointer lg:flex items-center relative"
            >
              <span
                @click="
                  previewPlayer.isShowZoomControl =
                    !previewPlayer.isShowZoomControl
                "
                >{{ previewPlayer.zoom }}x</span
              >
              <span
                class="pl-[4px] icon-pull-down iconfont text-s"
                @click="
                  previewPlayer.isShowZoomControl =
                    !previewPlayer.isShowZoomControl
                "
              />
              <div
                v-if="previewPlayer.isShowZoomControl"
                class="absolute top-0 left-[50%] transform -translate-y-[100%] border border-666 bg-fff -translate-x-[50%] list-none dark:bg-[#1D1E1F]"
              >
                <li
                  v-for="(item, index) in [1, 2, 3, 4, 5]"
                  :key="index"
                  class="hover:bg-gray dark:hover:bg-[#262727] py-[8px] px-[16px] select-none"
                  :class="
                    item === previewPlayer.zoom &&
                    'bg-blue !hover:bg-blue text-fff'
                  "
                  @click="previewPlayer.setZoom(item)"
                >
                  {{ item }}x
                </li>
              </div>
            </div>
          </div>
        </template>
      </PlayerControl>
    </div>
  </div>
</template>

<script lang="ts" setup>
import PlayerControl from "./control.vue";
import EventPlayerView from "./view.vue";
import { reactive, nextTick } from "vue";
import { FullScreen } from "@element-plus/icons-vue";
import type { Player, MonacoEvent } from "monaco-recorder";

const emit = defineEmits<{
  (e: "ready", value: ReturnType<typeof usePreviewPlayer>): void;
}>();

const previewPlayer = usePreviewPlayer();

emit("ready", previewPlayer);

function usePreviewPlayer() {
  // 预览播放器控制
  const previewPlayer = reactive({
    el: null as null | Player,
    previewId: "",
    previewPointerEvents: false, // 播放器鼠标事件禁用 启用可以选中视频里的内容
    fullScreen: false,
    isShowZoomControl: false,
    setFullScreen() {
      previewPlayer.fullScreen = !previewPlayer.fullScreen;
      previewPlayer.previewPointerEvents = previewPlayer.fullScreen;
    },
    /*
        // 默认缩放
        --preview-player-zoom: 4;
        width: calc(192px * var(--preview-player-zoom));
        height: calc(108px * var(--preview-player-zoom) + 50px);
    */
    zoom: 4,
    setZoom(zoom: number) {
      previewPlayer.zoom = zoom;
      previewPlayer.isShowZoomControl = false;
    },
    previewPlayerReady(p: Player) {
      previewPlayer.el = p;
    },
    jump(e: MouseEvent) {
      if (!previewPlayer.el) return;
      var distance = e.offsetX / (e.target as Element).clientWidth;
      const time = distance * previewPlayer.el.totalTime;
      previewPlayer.el.jumpToTime(time);
    },
    load(id: string, events: MonacoEvent[]) {
      // id相同的情况下
      if (previewPlayer.previewId === id) {
        // previewPlayer.unload()
        previewPlayer.centerDialog();
        return;
      }

      previewPlayer.previewId = id;

      previewPlayer.previewPointerEvents = false;
      previewPlayer.isShowZoomControl = false;
      // previewPlayer.zoom = 3
      previewPlayer.fullScreen = false;

      nextTick(() => {
        previewPlayer.el?.load(events);
        previewPlayer.el?.play();

        if (!document.getElementById("previewPlayer")?.style.top) {
          previewPlayer.centerDialog();
        }
      });

      // 如果是移动端全屏展示
      // const reduceMotionQuery = matchMedia("(min-width: 768px)")
      //
      // if (!reduceMotionQuery.matches) {
      //      previewPlayer.fullScreen = true
      // }
    },
    unload() {
      previewPlayer.el?.pause();
      previewPlayer.previewId = "";
      previewPlayer.fullScreen = false;
      previewPlayer.zoom = 4;
    },
    // 鼠标滚动缩放
    handleScroll(e: WheelEvent) {
      if (previewPlayer.previewPointerEvents || previewPlayer.fullScreen)
        return;
      e.preventDefault();
      let direction = e.deltaY > 0 ? "down" : "up";
      let num;
      if (direction == "down") {
        num = previewPlayer.zoom - 1;
      } else {
        num = previewPlayer.zoom + 1;
      }
      if (num > 5 || num < 1) {
        // 超出不处理
        return;
      } else {
        previewPlayer.setZoom(num);
      }
    },
    // 移动窗口
    moveDialog(e: MouseEvent) {
      if (previewPlayer.previewPointerEvents || previewPlayer.fullScreen)
        return;
      moveBox({
        event: e,
        el: "#previewPlayer", // minTop: 55
      });
    },

    // center
    centerDialog() {
      const dom = document.getElementById("previewPlayer");
      if (dom) {
        centerElementOnScreen(dom);
      }
    },
  });

  return previewPlayer;
}

/**
 *
 * @param {EVent} event
 * @param {selectors} el querySelector
 * @param {number} minLeft 最小Left限制
 * @param {number} minTop  最小minTop限制
 * @param {number} margin  右侧与下侧距离边缘的限制
 * @callback moveStart 移动前, moveEnd 移动后, click 点击
 */

type MoveBoxParam = {
  event: MouseEvent;
  el: string;
  minLeft?: number;
  minTop?: number;
  margin?: number;
  moveX?: boolean;
  moveY?: boolean;
  moveStart?: () => void;
  moveEnd?: () => void;
  click?: () => void;
};
function moveBox({
  event,
  el,
  minLeft = 10,
  minTop = 10,
  margin = 30,
  moveX = true,
  moveY = true,
  moveStart,
  moveEnd,
  click,
}: MoveBoxParam) {
  let box: HTMLElement | null = document.querySelector(el);

  if (!box) return;
  let isClick = true;
  let offsetX = event.pageX - box.offsetLeft;
  let offsetY = event.pageY - box.offsetTop;

  moveStart?.();

  document.onmousemove = function (d) {
    isClick = false;
    let left = d.pageX - offsetX;
    let top = d.pageY - offsetY;

    // 可视窗口的大小
    var winWid = document.documentElement.clientWidth;
    var winHei = document.documentElement.clientHeight;

    // 可视窗口的大小去减掉 所需移动的窗口的大小，就可以的出这个窗口所能达到的最边缘值
    winWid = winWid - margin; // - box.offsetWidth
    winHei = winHei - margin; // - box.offsetHeight

    // console.log(winWid, winHei)

    if (left > winWid) {
      left = winWid;
    }
    if (top > winHei) {
      top = winHei;
    }

    if (left <= minLeft) {
      left = minLeft;
    }
    if (top <= minTop) {
      top = minTop;
    }
    if (moveX) box!.style.left = left + "px";
    if (moveY) box!.style.top = top + "px";
  };
  //鼠标抬起，清除鼠标移动事件
  document.onmouseup = function () {
    document.onmousemove = null;
    document.onmouseup = null;
    if (isClick) click?.();
    moveEnd?.();
  };
}

function centerElementOnScreen(element: HTMLElement) {
  // 获取屏幕的宽度和高度
  const screenWidth =
    window.innerWidth ||
    document.documentElement.clientWidth ||
    document.body.clientWidth;
  const screenHeight =
    window.innerHeight ||
    document.documentElement.clientHeight ||
    document.body.clientHeight;

  // 获取要居中的元素的宽度和高度
  const elementWidth = element.offsetWidth;
  const elementHeight = element.offsetHeight;

  // 计算居中的位置
  const leftPosition = (screenWidth - elementWidth) / 2;
  const topPosition = (screenHeight - elementHeight) / 2;

  // 设置元素的样式
  element.style.position = "fixed";
  element.style.left = leftPosition + "px";
  element.style.top = topPosition + "px";
}
</script>

<style scoped>
.preview .shade {
  z-index: 5;
  width: 100%;
  height: 100%;
  top: 0%;
  left: 0%;
  position: absolute;
  background: rgba(0, 0, 0, 0.1);
}

.preview:hover .shade {
  background: rgba(0, 0, 0, 0.05);
}

/* 预览播放器 */
.preview-player {
  --preview-player-zoom: 4;
  position: fixed;
  /*top: 120px;*/
  /*left: 10px;*/
  bottom: 10px;
  right: 10px;
  z-index: 10000;
  width: calc(192px * var(--preview-player-zoom));
  height: calc(108px * var(--preview-player-zoom) + 50px);
}

.preview-player .preview-player-el.no-event {
  pointer-events: none;
  user-select: none;
}

.preview-player.full-screen {
  position: fixed;
  top: 0 !important;
  left: 0 !important;
  width: 100% !important;
  height: 100% !important;
  box-sizing: border-box;
  border: none;
}

.preview-player.full-screen .preview-player-el > *,
.preview-player.full-screen .preview-player-control > * {
  width: 100% !important;
  height: 100% !important;
  transform: scale(1) !important;
}

.preview-player.full-screen .preview-player-control {
  height: 40px !important;
}

.preview-player.zoom-1 {
  --preview-player-zoom: 1.7;
}

.preview-player.zoom-2 {
  --preview-player-zoom: 2;
}

.preview-player.zoom-3 {
  --preview-player-zoom: 3;
}

.preview-player.zoom-1 .preview-player-el > *,
.preview-player.zoom-2 .preview-player-el > * {
  width: 200%;
  height: 200%;
  transform: scale(0.5);
  transform-origin: left top;
}

.preview-player.zoom-3 .preview-player-el > * {
  width: 150%;
  height: 150%;
  transform: scale(0.67);
  transform-origin: left top;
}

.preview-player.zoom-4 {
  --preview-player-zoom: 4;
}

.preview-player.zoom-5 {
  --preview-player-zoom: 5;
}

@media (max-width: 1023.98px) {
  .preview-player {
    --preview-player-zoom: 1.7 !important;
    width: 100% !important;
    height: 44vh !important;
    left: 0 !important;
    bottom: 0 !important;
    top: initial !important;
  }
}
</style>
