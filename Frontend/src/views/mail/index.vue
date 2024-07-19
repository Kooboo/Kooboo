<script setup lang="ts">
import LeftMenu from "./components/left-menu/index.vue";
import Header from "@/layout/components/header";
import { useAppStore } from "@/store/app";
import { onUnmounted, watch, toRaw, ref, onMounted } from "vue";
import { useEmailStore } from "@/store/email";
import { useRoute } from "vue-router";
import KmailButton from "@/components/kmail-button/kmail-button.vue";
import SiteButton from "@/components/site-button/site-button.vue";
import type { EmailItem } from "@/api/mail/types";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { getMore, LatestMsgId } from "@/api/mail";

const appStore = useAppStore();
const emailStore = useEmailStore();
const route = useRoute();
const { t } = useI18n();

let timer: ReturnType<typeof setInterval> | null = null;
//进入到邮箱页面时，隐藏头部邮箱的红点
localStorage.setItem("isHasNewMessage", "false");
emailStore.isShowNewMessageLogo = "false";

let lastIndex: number;
clearInterval(timer!);
const latestMessageList = ref<EmailItem[]>([]);
const lateInboxMessageList = ref<EmailItem[]>([]);
const latestInboxMessageList = ref<EmailItem[]>([]);
timer = setInterval(async () => {
  let latestId = await LatestMsgId();

  if (latestId <= emailStore.preLatestId) return;
  //这里传true是去掉更新未读数接口的loading
  await emailStore.loadInboxAddressList(true);
  //若在收件箱列表页，则需根据最新inbox数据更新列表，并有新邮件进来时提示
  if (route.name === "inbox") {
    latestMessageList.value = await getMore(
      emailStore.folder!,
      emailStore.address,
      -1,
      emailStore.messageKeyword || "",
      true
    );
    // 如果有新邮件，就获取最新的inbox列表，用于计算新来了多少封邮件
    lateInboxMessageList.value = await getMore(emailStore.folder!, "", -1, "");
    if (emailStore.preLatestId < latestId) {
      ElMessage.success(
        t("common.receiveCountNewMessage", {
          count: lateInboxMessageList.value.filter(
            (f) => f.id > emailStore.preLatestId
          ).length,
        })
      );
    }

    if (emailStore.messageList.length < 30) {
      lastIndex = emailStore.messageList.length;
    } else {
      lastIndex = 30;
    }
    let oldMessageList = toRaw(emailStore.messageList).slice(0, lastIndex);
    let newMessageList: EmailItem[] = [];
    let oldIdList = oldMessageList.map((i) => i.id);

    //从最新的30条中，找出已被删除的邮件
    var removeMessageList = oldMessageList
      .map((m) => m.id)
      .filter((f) => {
        return !latestMessageList.value
          .map((m: { id: any }) => m.id)
          .includes(f);
      });

    //删除已不存在于最新邮件列表里的邮件
    removeMessageList.forEach((item) => {
      emailStore.messageList.splice(
        emailStore.messageList.findIndex((v) => v.id === item),
        1
      );
    });

    //从最新的30条中，获取新发来的邮件
    latestMessageList.value.forEach((m) => {
      if (!oldIdList.includes(m.id)) {
        newMessageList.push(m);
      }
    });

    //把id相同，read不同的邮件进行read重新赋值，保证read状态能够更新
    emailStore.messageList.forEach((item, index) => {
      //只遍历到最新邮件列表的长度项
      if (index > lastIndex) return;
      // 更改read状态
      let value = latestMessageList.value.find(
        (o: EmailItem) => o.id == item.id && o.read !== item.read
      );
      if (value != undefined) {
        item.read = value.read;
      }
    });

    //把新发来的邮件插入到emailStore.messageList显示
    newMessageList.forEach((f) => {
      if (f.id > oldIdList[0]) {
        emailStore.messageList.unshift(f);
      } else {
        emailStore.messageList.push(f);
      }
    });
  } else {
    //若不在收件箱列表页，则有新邮件进来时提示,无需更新列表;
    //获取最新inbox数据更新列表，并有新邮件进来时提示
    latestInboxMessageList.value = await getMore(
      "Inbox",
      "",
      -1,
      emailStore.messageKeyword || "",
      true
    );
    if (
      latestInboxMessageList.value.find((e) => e.id > emailStore.preLatestId)
    ) {
      ElMessage.success(
        t("common.receiveCountNewMessage", {
          count: latestInboxMessageList.value.filter(
            (f) => f.id > emailStore.preLatestId
          ).length,
        })
      );
    }
  }
  emailStore.preLatestId = latestId;
}, 300000);
onMounted(async () => {
  if (emailStore.folder !== "inbox") {
    emailStore.preLatestId = await LatestMsgId();
  }
});

onUnmounted(() => {
  clearInterval(timer!);
});

watch(
  () => route,
  () => {
    emailStore.address = route.query.address?.toString() ?? "";
    emailStore.folder = (route.query.folder || route.name)?.toString() || "";
  },
  { deep: true, immediate: true }
);

emailStore.loadAddress();

// 不在文件夹列表和不在文件夹的详情页的情况下，刷新页面，加载文件夹列表
if (route.name !== "folder" && route.query.folder !== "folder") {
  emailStore.loadFolders();
}
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <div class="h-full flex flex-col">
      <Header class="pl-40px">
        <template #left>
          <SiteButton />
          <KmailButton />
        </template>
        <template #right />
      </Header>
      <div class="flex-1 overflow-hidden relative">
        <LeftMenu />
        <div class="absolute inset-0 left-202px bg-[#f3f5f5] dark:bg-[#121212]">
          <el-scrollbar id="main-scrollbar" class="w-full">
            <router-view :key="$route.name!" />
          </el-scrollbar>
        </div>
      </div>
    </div>
  </el-config-provider>
</template>
