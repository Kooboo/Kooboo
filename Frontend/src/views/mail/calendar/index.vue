<template>
  <div class="p-20px pb-16 h-[calc(100vh-50px)] flex dark:text-[#fffa]">
    <!-- 日历 -->
    <el-calendar
      v-model="day"
      class="flex-1 rounded-normal shadow-s-20 h-full kCalendar"
    >
      <template #dateCell="{ data }">
        <div class="h-full group">
          <p class="p-4 px-8 flex items-center justify-between">
            <span :class="data.isSelected ? 'text-blue' : ''">{{
              data.day.split("-").slice(1).join("-")
            }}</span>
            <el-icon
              class="el-icon iconfont icon-a-addto text-blue hidden group-hover:block text-12px font-bold"
              :title="t('common.newSchedule')"
              @click.stop="newSchedule(data.day)"
            />
          </p>
          <ul class="overflow-hidden scheduleList">
            <li
              v-for="item in calendarList"
              :key="item.id"
              :title="item.calendarTitle"
            >
              <div
                v-if="
                  isDuringTime(
                    useDate(data.day),
                    useDate(item.start),
                    useDate(item.end)
                  )
                "
                class="w-full my-2px p-4 text-12px flex items-center flex-row-reverse dark:bg-[#18222c] h-20px leading-5"
              >
                <div class="flex items-center space-x-4 w-full">
                  <div class="h-6px w-6px rounded-full bg-blue" />
                  <span
                    class="text-000 flex-1 dark:text-[#fffa] flex-1 ellipsis"
                  >
                    {{ item.calendarTitle }}
                  </span>
                </div>
              </div>
            </li>
          </ul>
          <div
            class="moreAction hidden absolute bottom-0 left-1/2 transform -translate-x-1/2 w-full h-24 text-center text-blue z-10"
          >
            <el-icon
              class="iconfont icon-more hover:text-blue rotate-90 transform text-14px h-24"
            />
          </div>
        </div>
      </template>
    </el-calendar>
    <!-- 日程列表 -->
    <el-scrollbar
      class="bg-fff dark:bg-[#333] shadow-s-20 transition-all duration-500 rounded-normal relative dark:text-[#fffa]"
      :class="showScheduleDetail ? ' w-300px  ml-16' : 'w-0 p-0 '"
    >
      <div v-if="showScheduleDetail" class="overflow-hidden">
        <h1 class="font-bold text-center my-8 p-12 text-20px">
          {{ useMonthDay(day) }} ( {{ week[new Date(day).getDay()] }} )
        </h1>

        <ul v-if="currentScheduleList?.length">
          <el-popover
            v-for="item in currentScheduleList"
            :key="item.id"
            placement="left"
            :width="400"
            trigger="click"
            popper-style="padding:0px;"
            popper-class="!p-0 scheduleDetail"
          >
            <template #reference>
              <li
                class="w-full flex items-baseline text-m hover:bg-[#f5f6f7] p-16 cursor-pointer dark:hover:bg-[#244764]"
              >
                <div class="text-999 w-80px">
                  <div class="flex items-center space-x-8">
                    <div class="h-6px w-6px rounded-full bg-blue" />
                    <div>
                      {{
                        useDate(new Date(day)) === useDate(item.start)
                          ? useHourMinute(item.start)
                          : isDuringTime(
                              useTime(item.end),
                              useDate(new Date(day)) + " 00:00:00",
                              getNextDate(useDate(new Date(day)), +1) +
                                " 00:00:00"
                            )
                          ? useHourMinute(item.end)
                          : t("common.allDay")
                      }}
                    </div>
                  </div>
                  <div
                    v-if="useDate(day) !== useDate(item.start)"
                    class="pl-14px"
                  >
                    {{
                      useDate(day) === useDate(item.end) ? t("common.end") : ""
                    }}
                  </div>
                </div>

                <div
                  class="flex-1 text-999 break-words text-start space-y-4"
                  style="word-break: break-word"
                >
                  <div class="text-000 font-bold dark:text-[#fffa]">
                    <span>
                      {{ item.calendarTitle }}
                    </span>
                  </div>
                  <div v-if="item.location" class="flex space-x-4">
                    <el-icon class="iconfont icon-dingwei h-22px" />
                    <span class="flex-1 break-words text-start">{{
                      item.location
                    }}</span>
                  </div>
                  <div v-if="item.mark" class="flex space-x-4 overflow-auto">
                    <el-icon class="iconfont icon-wenjian h-22px" />
                    <RichShadow :html="item.mark" class="leading-22px" />
                  </div>
                </div>
              </li>
            </template>
            <!-- 日程详情弹框 -->
            <div>
              <el-scrollbar max-height="550px" view-class="text-m ">
                <!-- schedule -->
                <div class="p-16">
                  <div class="flex space-x-8">
                    <div class="flex items-center h-22px">
                      <div class="h-8 w-8 rounded-full bg-blue" />
                    </div>
                    <h1 class="text-000 font-bold dark:text-[#fffa]">
                      {{ item.calendarTitle }}
                    </h1>
                  </div>
                  <div class="flex items-center mt-8">
                    <div
                      :class="
                        useDate(item.start) === useDate(item.end)
                          ? 'flex space-x-8'
                          : ''
                      "
                    >
                      <div>
                        {{ useMonthDay(item.start) }}
                        ( {{ week[new Date(item.start).getDay()] }} )
                      </div>
                      <div>
                        {{ useHourMinute(item.start) }}
                      </div>
                    </div>
                    <span class="mx-12">——</span>
                    <div>
                      <div v-if="useDate(item.start) !== useDate(item.end)">
                        {{ useMonthDay(item.end) }}
                        ( {{ week[new Date(item.end).getDay()] }} )
                      </div>
                      <div>
                        {{ useHourMinute(item.end) }}
                      </div>
                    </div>
                  </div>
                  <div v-if="item.contact.length">
                    <el-divider class="my-8" />
                    <span class="mr-4">{{ item.totalCount }}</span>
                    <span class="">{{ t("calendar.participants") }}</span>
                    <div class="text-[#959DA6] text-12px mb-8">
                      <span v-if="item.acceptCount"
                        >{{ item.acceptCount }} {{ t("calendar.yes")
                        }}<span
                          v-if="
                            (item.acceptCount !== item.totalCount &&
                              item.declineCount) ||
                            item.tentativeCount ||
                            item.awaitingCount
                          "
                          >,
                        </span></span
                      >
                      <span v-if="item.declineCount"
                        >{{ item.declineCount }} {{ t("calendar.no")
                        }}<span
                          v-if="
                            (item.declineCount !== item.totalCount &&
                              item.tentativeCount) ||
                            item.awaitingCount
                          "
                          >,
                        </span></span
                      >
                      <span v-if="item.tentativeCount"
                        >{{ item.tentativeCount }} {{ t("calendar.maybe")
                        }}<span
                          v-if="
                            item.tentativeCount !== item.totalCount &&
                            item.awaitingCount
                          "
                          >,
                        </span></span
                      >
                      <span v-if="item.awaitingCount"
                        >{{ item.awaitingCount }}
                        {{ t("calendar.awaiting") }}</span
                      >
                    </div>
                    <ul>
                      <div
                        v-if="item.organizer"
                        class="flex items-center justify-between mb-8"
                      >
                        <div>
                          <div>
                            {{ item.organizer }}
                          </div>
                          <div class="text-[#959DA6] text-12px">
                            {{ t("common.organizer") }}
                          </div>
                        </div>
                        <el-icon class="iconfont icon-yes text-green mr-8" />
                      </div>
                      <li
                        v-for="itm in item.attendeeStatus?.filter(
                          (f) => f.address !== item.organizer
                        )"
                        :key="itm.address"
                        class="flex justify-between items-center"
                        :title="itm.address"
                      >
                        <span class="ellipsis">{{ itm.address }}</span>
                        <el-icon
                          class="iconfont mr-8"
                          :class="{
                            ' icon-yes text-green ':
                              itm.participationStatus === 1,
                            ' icon-delete4 text-orange ':
                              itm.participationStatus === 2,
                            ' icon-problem text-blue':
                              itm.participationStatus === 0,
                            ' icon-shalou': itm.participationStatus === -1,
                          }"
                        />
                      </li>
                    </ul>
                  </div>
                  <el-divider v-if="item.location || item.mark" class="my-8" />

                  <div v-if="item.location" class="text-999 flex">
                    <el-icon class="iconfont icon-dingwei h-22px w-14px mr-4" />
                    <span class="flex-1 break-words text-start">{{
                      item.location
                    }}</span>
                  </div>

                  <div v-if="item.mark" class="flex text-999">
                    <el-icon class="iconfont icon-wenjian h-22px mr-4" />
                    <RichShadow :html="item.mark" class="leading-22px" />
                  </div>
                </div>
              </el-scrollbar>
              <!-- footer -->
              <div
                class="text-right flex flex-row-reverse bg-[#f5f6f7] dark:bg-444 p-12 w-full"
              >
                <IconButton
                  class="text-orange hover:text-orange"
                  icon="icon-delete "
                  :tip="t('common.delete')"
                  @click="deleteSchedule(item)"
                />
                <IconButton
                  v-if="item.isOrganizer"
                  icon="icon-a-writein"
                  :tip="t('common.edit')"
                  data-cy="edit"
                  @click="editSchedule(item)"
                />
              </div>
            </div>
          </el-popover>
        </ul>
        <div
          v-else
          class="absolute top-1/2 left-1/2 transform -translate-x-1/2 text-center space-y-12 text-14px w-full"
        >
          <div class="text-999">
            {{ t("common.noScheduleForThatDay") }}
          </div>
          <div
            class="text-blue cursor-pointer"
            @click="newSchedule(useDate(new Date(day)))"
          >
            {{ t("common.newSchedule") }}
          </div>
        </div>
      </div>
    </el-scrollbar>
  </div>
  <div
    class="flex rounded-full bg-fff shadow-s-10 w-26px flex items-center h-26px cursor-pointer dark:bg-666 absolute top-32 right-306px flex items-center justify-center transition-all duration-500"
    :class="!showScheduleDetail ? ' -right-26px' : ''"
    @click="showScheduleDetail = false"
  >
    <el-icon
      class="iconfont icon-a-nextstep2 cursor-pointer text-blue text-s"
    />
  </div>
  <div
    class="fixed top-90px -right-100px bg-fff dark:bg-999 p-12 shadow-s-20 rounded-l-full cursor-pointer transition-all duration-500 flex items-center justify-center hover:text-blue"
    :class="!showScheduleDetail ? ' !right-0' : ''"
    :title="t('common.schedule')"
    @click="showScheduleDetail = true"
  >
    <el-icon class="iconfont icon-richeng text-20px" />
  </div>

  <NewScheduleDialog
    v-if="showNewScheduleDialog"
    v-model="showNewScheduleDialog"
    :current-schedule="currentSchedule!"
    @reload="load()"
  />
</template>

<script lang="ts" setup>
import { getEmailCalendar, deleteScheduleApi } from "@/api/mail";
import { onMounted, ref, watch, nextTick } from "vue";
import NewScheduleDialog from "./new-schedule-dialog.vue";
import {
  useDate,
  useYearMonth,
  useTime,
  useMonthDay,
  useHourMinute,
  isDuringTime,
  getNextDate,
  week,
} from "@/hooks/use-date";
import type { Schedule, ScheduleAPI } from "@/api/mail/types";
import { useI18n } from "vue-i18n";
import { showConfirm, showDeleteConfirm } from "@/components/basic/confirm";
import { useEmailStore } from "@/store/email";
import RichShadow from "@/views/mail/components/rich-editor-content/index.vue";

const { t } = useI18n();
const day = ref(new Date());
const showScheduleDetail = ref(true);
const currentScheduleList = ref<ScheduleAPI[]>();
const calendarList = ref<ScheduleAPI[]>();
const showNewScheduleDialog = ref(false);
const currentSchedule = ref<Schedule>({} as Schedule);
const emailStore = useEmailStore();

const load = async () => {
  calendarList.value = await getEmailCalendar(
    getNextDate(useDate(day.value), -31) + " 00:00:00",
    getNextDate(useDate(day.value), +31) + " 00:00:00"
  );
  initScheduleDetail();
};
onMounted(async () => {
  await load();
  isShowMoreAction();
});

const initScheduleDetail = () => {
  currentScheduleList.value = [];
  calendarList.value?.forEach((f: any) => {
    if (isDuringTime(useDate(day.value), useDate(f.start), useDate(f.end))) {
      currentScheduleList.value?.push(f);
    }
  });
};

const newSchedule = (d: string) => {
  showNewScheduleDialog.value = true;

  let currentTime = new Date();
  let startHour;
  let endHour;
  let index = currentTime.getHours();
  let startMinute;
  let hourList = [];
  for (var i = 0; i < 24; i++) {
    let s = i.toString();
    if (i < 10) {
      s = "0" + i;
    }
    hourList.push(s);
  }

  if (currentTime.getMinutes() < 30) {
    startMinute = "30";
  } else {
    startMinute = "00";
    index += 1;
  }
  startHour = hourList[index % 24];
  endHour = hourList[(index + 1) % 24];

  currentSchedule.value.startDate = d;
  currentSchedule.value.startTime = d + ` ${startHour}:${startMinute}:00`;
  currentSchedule.value.endDate = d;
  currentSchedule.value.endTime = d + ` ${endHour}:${startMinute}:00`;
};

const editSchedule = (row: ScheduleAPI) => {
  showNewScheduleDialog.value = true;
  currentSchedule.value.id = row.id;
  currentSchedule.value.startDate = useDate(row.start);
  currentSchedule.value.startTime = useTime(row.start);
  currentSchedule.value.endDate = useDate(row.end);
  currentSchedule.value.endTime = useTime(row.end);
  currentSchedule.value.calendarTitle = row.calendarTitle;
  currentSchedule.value.mark = row.mark;
  currentSchedule.value.location = row.location;
  currentSchedule.value.contact = row.contact.filter(
    (f) => f !== row.organizer
  );
  currentSchedule.value.isOrganizer = row.isOrganizer!;
};

const deleteSchedule = async (item: ScheduleAPI) => {
  let unExpired = useTime(item.start) > useTime(new Date());
  if (item.contact.length && item.isOrganizer && unExpired) {
    await showConfirm(t("common.aCancellationEmailWillBeSentToParticipants"));
  } else {
    await showDeleteConfirm();
  }
  await deleteScheduleApi(item.id!, emailStore.defaultAddress);
  await load();
};

const isShowMoreAction = () => {
  nextTick(() => {
    document.querySelectorAll(".scheduleList").forEach((f) => {
      let contentHeight = f.getBoundingClientRect().height + 24;
      let contentBoxHeight =
        f.parentElement!.parentElement!.parentElement!.getBoundingClientRect()
          .height;
      let element = f.nextElementSibling as HTMLElement;
      if (
        parseInt(contentHeight.toString()) >
        parseInt(contentBoxHeight.toString())
      ) {
        element.style.display = "block";
      } else {
        element.style.display = "none";
      }
    });
  });
};

watch(
  () => day.value,
  async (newValue, oldValue) => {
    initScheduleDetail();
    if (useYearMonth(oldValue!) != useYearMonth(newValue)) {
      await load();
      isShowMoreAction();
    }
  },
  { immediate: true }
);

watch(
  () => calendarList.value,
  () => {
    isShowMoreAction();
  }
);

watch(
  () => showNewScheduleDialog.value,
  () => {
    if (!showNewScheduleDialog.value) currentSchedule.value = {} as Schedule;
  }
);

//给tinymce编辑器的菜单弹框设置z-index样式
const index = ref();
watch(
  () => showNewScheduleDialog.value,
  () => {
    nextTick(() => {
      if (showNewScheduleDialog.value) {
        index.value = Array.from<HTMLElement>(
          document.querySelectorAll(".el-overlay")
        ).at(-1)?.style.zIndex;
        document.body.style.setProperty("--tox-tinymce-aux-index", index.value);
        document.body.classList.add("editScheduleDialog");
      } else {
        document.body.classList.remove("editScheduleDialog");
      }
    });
  }
);

window.onresize = () => {
  nextTick(() => {
    isShowMoreAction();
  });
};
</script>
<style scoped>
:deep(.el-calendar__title) {
  font-weight: bold;
  font-size: 36px;
}
:deep(.dark .el-calendar__title) {
  color: white;
}

:deep(.el-calendar__header) {
  display: flex;
  align-items: center;
  padding: 20px;
}

:deep(.el-calendar-day) {
  padding: 0;
}
:deep(.el-calendar-day:hover) {
  background: none;
}
:deep(.el-calendar) {
  overflow: auto;
  display: flex;
  flex-direction: column;
}
:deep(.el-calendar__body) {
  height: 100%;
  flex: 1 1 0%;
}
:deep(.el-calendar-table) {
  height: 100%;
}
:deep(.el-calendar-table__row td) {
  overflow: hidden;
  position: relative;
  transition: none;
}
:deep(.el-calendar-table__row td:hover) {
  background: #ecf5ff;
}

:deep(.el-calendar-table__row td:hover .moreAction) {
  background-image: linear-gradient(
    180deg,
    hsla(0, 0%, 100%, 0),
    #e5f1fc 49%,
    #e5f1fc
  );
}

/* 滚动条样式*/
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

/* 滚动条上的滚动滑块 */
::-webkit-scrollbar-thumb {
  background-color: #dedfe1;
  border-radius: 4px;
}
.dark *::-webkit-scrollbar-thumb {
  background-color: #fff3;
}
.is-selected .moreAction {
  background-image: linear-gradient(
    180deg,
    hsla(0, 0%, 100%, 0),
    #e5f1fc 49%,
    #e5f1fc
  );
}
.dark .is-selected .moreAction {
  background-image: linear-gradient(
    180deg,
    hsla(0, 0%, 100%, 0),
    #32495f 49%,
    #32495f
  );
}
.moreAction {
  background-image: linear-gradient(
    180deg,
    hsla(0, 0%, 100%, 0),
    #fff 49%,
    #fff
  );
}
.dark .moreAction {
  background-image: linear-gradient(
    180deg,
    hsla(0, 0%, 100%, 0),
    #32495f 49%,
    #32495f
  );
}
</style>
<style>
.scheduleDetail.el-popover.el-popper {
  word-break: break-word;
  text-align: start;
}
.dark .kCalendar .el-calendar-table__row td:hover {
  background: #18222c;
}
.dark .kCalendar .el-calendar-table__row td:hover .moreAction {
  background-image: linear-gradient(
    180deg,
    hsla(0, 0%, 100%, 0),
    #32495f 49%,
    #32495f
  );
}
.editScheduleDialog .tox-tinymce-aux {
  z-index: var(--tox-tinymce-aux-index);
}
</style>
