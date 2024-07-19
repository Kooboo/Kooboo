<template>
  <div class="p-24 flex">
    <div
      ref="outBox"
      class="rounded-normal h-[calc(100vh-160px)] bg-fff shadow-s-10 p-24 pr-8 dark:bg-[#252526] w-full transition-all duration-500"
      :class="
        getExtensionDrawerOpen ? 'box-border w-[calc(100%-22%-46px)]' : ''
      "
    >
      <el-scrollbar height="h-full" class="pr-16" always>
        <el-form
          ref="form"
          label-position="left"
          label-width="100px"
          :model="model"
          :rules="rules"
          @submit.prevent
        >
          <div ref="topPartForm">
            <el-form-item :label="t('mail.from')" prop="from">
              <el-select
                :model-value="model.from"
                class="w-480px"
                placeholder=" "
                :title="
              addressFromList?.filter((i: any) => i.id === model.from)[0]?.address
            "
                data-cy="from"
                @update:model-value="
    (e: any) => {
      model.from = addressFromList?.find((i: any) => i.id === e)?.id;
      (model as any).fromAddress = addressFromList?.find((i: any) => i.id === e)?.address;
    }
  "
                @change="onChangeFrom()"
              >
                <el-option
                  v-for="item in addressFromList"
                  :key="item.id"
                  style="max-width: 480px"
                  :title="item.address"
                  :label="item.displayName"
                  :value="item.id"
                  data-cy="from-opt"
                />
              </el-select>
              <span
                v-if="saveTime !== ''"
                class="text-s text-999"
                :class="signature !== '' ? 'px-8' : 'px-12 '"
              >
                {{
                  t("common.draftSavedAtTime", {
                    time: useHourMinute(saveTime),
                  })
                }}</span
              >
            </el-form-item>
            <el-form-item id="to" :label="t('mail.to')" prop="to">
              <div
                id="toInputShade"
                class="bg-blue h-10 w-480px absolute rounded-5px z-9999 hidden opacity-0 inputShade"
                @drop="drop($event, 'to')"
                @dragover="allowDrop"
              />
              <SelectInput
                v-model="model.to"
                :input-value="model.to || []"
                data-cy="to"
                @drop="drop($event, 'to')"
                @dragover="allowDrop"
              />
              <div class="pl-12 space-x-12 text-s">
                <span
                  v-if="!model.cc"
                  class="cursor-pointer text-999 hover:text-blue"
                  data-cy="add-cc"
                  @click="model.cc = []"
                  >{{ t("common.addCc") }}</span
                >
                <span
                  v-if="!model.bcc"
                  class="cursor-pointer text-999 hover:text-blue"
                  data-cy="add-bcc"
                  @click="model.bcc = []"
                  >{{ t("common.addBcc") }}</span
                >
              </div>
            </el-form-item>
            <!-- 抄送 密送 -->

            <el-form-item
              v-if="model.cc"
              id="cc"
              :label="t('common.carbonCopy')"
              prop="cc"
            >
              <div
                id="ccInputShade"
                class="bg-blue h-10 w-480px absolute rounded-5px z-9999 hidden opacity-0 inputShade"
                @drop="drop($event, 'cc')"
                @dragover="allowDrop"
              />
              <SelectInput
                v-model="model.cc"
                :input-value="model.cc"
                data-cy="cc"
                @drop="drop($event, 'cc')"
                @dragover="allowDrop"
              />
              <div class="pl-12 text-orange text-s">
                <span class="cursor-pointer" @click="model.cc = undefined">{{
                  t("common.delete")
                }}</span>
              </div>
            </el-form-item>

            <el-form-item
              v-if="model.bcc"
              id="bcc"
              :label="t('common.blindCarbonCopy')"
              prop="bcc"
            >
              <div
                id="bccInputShade"
                class="bg-blue h-10 w-480px absolute rounded-5px z-9999 hidden opacity-0 inputShade"
                @drop="drop($event, 'bcc')"
                @dragover="allowDrop"
              />
              <SelectInput
                v-model="model.bcc"
                :input-value="model.bcc"
                data-cy="bcc"
                @drop="drop($event, 'bcc')"
                @dragover="allowDrop"
              />

              <div class="pl-12 text-orange text-s">
                <span class="cursor-pointer" @click="model.bcc = undefined">{{
                  t("common.delete")
                }}</span>
              </div>
            </el-form-item>

            <el-form-item :label="t('common.subject')" prop="subject">
              <el-input
                v-model="model.subject"
                class="w-480px subjectInput"
                data-cy="subject"
              />
            </el-form-item>

            <el-form-item :label="t('common.attachments')" prop="attachments">
              <div class="space-x-16 flex">
                <div class="h-40px flex items-center">
                  <label
                    class="h-32 bg-blue rounded-full flex items-center justify-center cursor-pointer"
                  >
                    <span
                      class="mx-12 flex text-fff h-32 w-50px justify-center cursor-pointer"
                    >
                      {{ t("common.select") }}
                    </span>
                    <input
                      id="attachments"
                      class="hidden"
                      type="file"
                      data-cy="upload-attachment"
                      @change="addAttachment"
                    />
                  </label>
                </div>
                <!-- 附件 -->
                <div class="flex flex-wrap items-center">
                  <el-tag
                    v-for="item in model.attachments"
                    :key="item.fileName"
                    class="rounded-full px-16 mr-8 my-4"
                    size="large"
                    effect="plain"
                    round
                    closable
                    @close="removeAttachment(item.fileName)"
                  >
                    <div class="max-w-132px ellipsis h-24 leading-6">
                      {{ item.fileName }}
                    </div>
                  </el-tag>
                </div>
              </div>
            </el-form-item>
          </div>

          <el-form-item
            :label="t('common.content')"
            class="contentFormItem mb-0"
          >
            <KEditor
              v-model="model.html"
              :manual-upload="true"
              :hidden-code="true"
              :only-new-window="true"
              :min_height="editorHeight"
              :max_height="outBox?.offsetHeight"
            />
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>
    <ExtensionDrawer
      ref="extensionDrawerRef"
      :context="context"
      :message-id="parseInt(getQueryString('sourceId') ?? '0')"
      type="extendcompose"
    />
  </div>

  <div
    v-bind="$attrs"
    class="k-bottom-bar bg-gray/10 py-15px pr-130px text-right space-x-32 absolute bottom-0 w-full left-0 z-1 backdrop-filter backdrop-blur-md shadow-m-20"
  >
    <el-button
      round
      type="primary"
      data-cy="send"
      :disabled="!autoSaveDraftFlag"
      @click="send"
      >{{ t("common.send") }}</el-button
    >
    <el-button
      plain
      round
      type="primary"
      data-cy="save-draft"
      @click="saveDraft({ isLeave: false, showLoading: true })"
      >{{ t("common.saveDraft") }}</el-button
    >
    <el-button round data-cy="cancel" @click="cancel">{{
      t("common.cancel")
    }}</el-button>
  </div>
  <ElDialog
    v-model="alertDialog.show"
    :width="alertDialog.width || '600px'"
    :title="alertDialog.title || t('common.tooltip')"
  >
    <div v-html="alertDialog.body" />
  </ElDialog>
  <LeaveConfirmDialog
    v-if="showLeaveConfirmDialog"
    v-model="showLeaveConfirmDialog"
    @save-draft="saveDraft({ isLeave: true })"
    @cancel-save-draft="cancelSaveDraft"
    @close-dialog="closeConfirmDialog"
  />
</template>

<script lang="ts">
import { i18n } from "@/modules/i18n";
import ExtensionDrawer from "@/views/mail/components/extensions-drawer/index.vue";
import { dark, toggleDark } from "@/composables/dark";

const $t = i18n.global.t;
const addressFromList = ref<Address[]>();
export default {
  async beforeRouteEnter(to: any, from: any) {
    var query = getQueryString("messageId");
    if (!query) {
      query = getQueryString("sourceId");
    }

    addressFromList.value = await getFromList(query ?? "0");
    if (addressFromList.value && addressFromList.value.length === 0) {
      ElMessage.error($t("common.emptyAddressTips"));
    }
  },
};
</script>
<script lang="ts" setup>
import {
  ref,
  watch,
  nextTick,
  computed,
  onMounted,
  onBeforeUnmount,
  onUnmounted,
} from "vue";
import { useI18n } from "vue-i18n";
import type { Rules } from "async-validator";
import KEditor from "@/components/k-editor/index.vue";
import type { Address, EmailDraft } from "@/api/mail/types";
import {
  compose,
  forward,
  reply,
  sendEmail,
  postEmail,
  postAttachment,
  deleteAttachment,
  getFromList,
  getSignature,
  reEdit,
} from "@/api/mail/index";
import { requiredRule, toEmailRule } from "@/utils/validate";
import type { ElForm } from "element-plus";
import { ElMessage } from "element-plus";
import { useRoute, useRouter, onBeforeRouteLeave } from "vue-router";
import { getQueryString } from "@/utils/url";
import SelectInput from "../components/select-input/index.vue";
import LeaveConfirmDialog from "@/components/leave-confirm-dialog/index.vue";
import { useHourMinute, getCurrentTimeZone } from "@/hooks/use-date";
import { ElLoading } from "element-plus";
import { useEmailStore } from "@/store/email";

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const form = ref<InstanceType<typeof ElForm>>();
const model = ref({} as EmailDraft);
const outBox = ref();
const extensionDrawerRef = ref();
const getExtensionDrawerOpen = computed(() => {
  return extensionDrawerRef.value?.open ?? false;
});
const editorHeight = ref();
const topPartForm = ref();
let observer: MutationObserver;
let type = getQueryString("type") as SendType;

// 当前邮件的id,如果保存了草稿,就更新为新草稿的id
const currentMessageId = ref();
let initialValue: string | undefined = undefined;
const checkUnSave = ref(true);
const showLeaveConfirmDialog = ref(false);
const nextRouter = ref<any>({
  name: "",
  params: {},
  query: {},
});
let timer: ReturnType<typeof setInterval> | null = null;
const saveTime = ref("");
let loadingInstance: { close: () => void } | undefined;
const emailStore = useEmailStore();
const signature = ref();
const autoSaveDraftFlag = ref(true);

// 浏览器离开提示
let showBeforeUnload: boolean;
const beforeunloadAction = (event: {
  preventDefault: () => void;
  returnValue: string;
}) => {
  if (showBeforeUnload) {
    event.preventDefault();
    event.returnValue = "";
  }
};
const changeSignature = (isFrontOfContent?: boolean) => {
  if (!model.value.html.match("<!--signature -->")) {
    // 如果model.value.html为空，则签名的前面加空行，否则不加
    let signatureContent =
      model.value.html === ""
        ? `<div>&nbsp;</div>\n<!--signature -->\n<div>&nbsp;</div>\n<!--signature -->`
        : `<!--signature --><div>&nbsp;</div><!--signature -->`;
    // 如果是在回复或转发页面，签名加在内容的前面，否则加在后面
    if (isFrontOfContent) {
      model.value.html =
        `<div>&nbsp;</div>\n<!--signature -->\n<div>&nbsp;</div><!--signature -->\n` +
        model.value.html;
    } else {
      model.value.html = model.value.html + signatureContent;
    }
  }
  let replacement = signature.value
    ? `<!--signature -->\n<div class="signature" style="-webkit-user-modify: read-only; pointer-events: none;">\n<div style="background: #e4e5e6; width: 40px; height: 1px; margin: 15px 0;">&nbsp;</div>\n${signature.value}\n</div>\n<!--signature -->`
    : "<!--signature --><!--signature -->";
  model.value.html = model.value.html.replace(
    /(<!--signature -->)([\s\S]*)(<!--signature -->)/,
    replacement
  );
};

const onChangeFrom = async () => {
  signature.value = await getSignature(model.value.from!);
  changeSignature();
};

onMounted(() => {
  const resizeObserver = new ResizeObserver((entries) => {
    for (const entry of entries) {
      editorHeight.value =
        outBox.value?.offsetHeight - topPartForm.value?.offsetHeight - 70;
    }
  });
  resizeObserver.observe(outBox.value);
  currentMessageId.value = route.query.messageId;
  // 添加监听浏览器关闭的beforeunload事件
  window.addEventListener("beforeunload", beforeunloadAction);
});

onBeforeUnmount(() => {
  observer?.disconnect();
});

const alertDialog = ref({
  show: false,
  title: "",
  body: "",
  width: "600px",
});

const rules = {
  from: [requiredRule(t("common.senderRequired"))],
  to: [toEmailRule()],
  cc: [toEmailRule()],
  bcc: [toEmailRule()],
  subject: [requiredRule(t("common.subjectRequiredTips"))],
} as Rules;
type SendType = "Drafts" | "Forward" | "Reply" | "ReplyAll" | "EditAgain";

const dragArea = ref();
const allowDrop = (ev: { preventDefault: () => void }) => {
  ev.preventDefault();
};
const drag = (
  ev: {
    dataTransfer: { setData: (arg0: string, arg1: any) => void };
    target: { innerText: any };
  },
  name: string
) => {
  //拖动时阻止subject输入框和content输入框的鼠标事件
  document
    .querySelector(".contentFormItem")
    ?.classList.add("pointer-events-none");
  document.querySelector(".subjectInput")?.classList.add("pointer-events-none");

  //拖动时，显示所有输入框的背景色遮罩
  document.querySelectorAll(".inputShade").forEach((i) => {
    //不显示当前拖动的输入框遮罩，防止触发不了鼠标事件
    if (i.getAttribute("id") === name + "InputShade") return;
    i.classList.add("block");
    i.classList.remove("hidden");
    i.classList.remove("opacity-10");
  });

  dragArea.value = name;
  ev.dataTransfer.setData("text/plain", ev.target.innerText);
};

const drop = (ev: DragEvent, name: string) => {
  if (name !== dragArea.value) {
    var data = ev.dataTransfer!.getData("text/plain");
    let index = model.value[dragArea.value]!.indexOf(data);
    model.value[dragArea.value].splice(index, 1);

    //输入框里包含有data数据时，不做push
    if (!model.value[name].includes(data)) {
      model.value[name].push(data);
    }
    // 拖动结束后，拖动输入框和放置输入框都重新触发表单校验
    form.value!.validateField(name);
    form.value!.validateField(dragArea.value);
  }

  ev.preventDefault();
  dragArea.value = "";
};

const dragend = () => {
  //拖动结束后，隐藏所有输入框的背景色遮罩
  document.querySelectorAll(".inputShade").forEach((i) => {
    i.classList.remove("block");
    i.classList.add("hidden");
    i.classList.remove("opacity-10");
    i.classList.add("opacity-0");
  });

  //拖动时结束后，恢复subject输入框和content输入框的鼠标事件
  document
    .querySelector(".contentFormItem")
    ?.classList.remove("pointer-events-none");

  document
    .querySelector(".subjectInput")
    ?.classList.remove("pointer-events-none");
};
watch(
  () => [model.value.to, model.value.bcc, model.value.cc],
  (n) => {
    if (n?.length) {
      nextTick().then(() => {
        document
          .querySelectorAll(".el-hover-title .el-tag__content")
          .forEach((i) => {
            i.setAttribute("title", (i as HTMLElement).innerText);
          });

        document
          .querySelectorAll(
            ".el-tag.is-closable.el-tag--info.el-tag--default.el-tag--light"
          )
          .forEach((i: any) => {
            i.addEventListener("dragend", dragend);
          });
      });
    }
  },
  {
    deep: true,
  }
);

watch(
  () => model.value.to,
  () => {
    nextTick().then(() => {
      document
        .getElementById("to")!
        .querySelectorAll(
          ".el-tag.is-closable.el-tag--info.el-tag--default.el-tag--light"
        )
        .forEach((i: any) => {
          i.setAttribute("draggable", "true");
          i.addEventListener("dragstart", ($event: any) => drag($event, "to"));
        });

      const toInputShadeDom = document.getElementById("toInputShade");
      toInputShadeDom!.addEventListener("dragenter", () => {
        toInputShadeDom!.classList.remove("opacity-0");
        toInputShadeDom!.classList.add("opacity-10");
      });
      toInputShadeDom!.addEventListener("dragleave", () => {
        toInputShadeDom!.classList.remove("opacity-10");
        toInputShadeDom!.classList.add("opacity-0");
      });
    });
  },
  {
    deep: true,
  }
);

watch(
  () => model.value.cc,
  () => {
    if (!model.value.cc) return;
    nextTick().then(() => {
      document
        .getElementById("cc")!
        .querySelectorAll(
          ".el-tag.is-closable.el-tag--info.el-tag--default.el-tag--light"
        )
        .forEach((i: any) => {
          i.setAttribute("draggable", "true");
          i.addEventListener("dragstart", ($event: any) => drag($event, "cc"));
        });

      const ccInputShadeDom = document.getElementById("ccInputShade");
      ccInputShadeDom!.addEventListener("dragenter", () => {
        ccInputShadeDom!.classList.remove("opacity-0");
        ccInputShadeDom!.classList.add("opacity-10");
      });
      ccInputShadeDom!.addEventListener("dragleave", () => {
        ccInputShadeDom!.classList.remove("opacity-10");
        ccInputShadeDom!.classList.add("opacity-0");
      });
    });
  },
  {
    deep: true,
  }
);

watch(
  () => model.value.bcc,
  () => {
    if (!model.value.bcc) return;
    nextTick().then(() => {
      document
        .getElementById("bcc")!
        .querySelectorAll(
          ".el-tag.is-closable.el-tag--info.el-tag--default.el-tag--light"
        )
        .forEach((i: any) => {
          i.setAttribute("draggable", "true");
          i.addEventListener("dragstart", ($event: any) => drag($event, "bcc"));
        });

      const bccInputShadeDom = document.getElementById("bccInputShade");
      bccInputShadeDom!.addEventListener("dragenter", () => {
        bccInputShadeDom!.classList.remove("opacity-0");
        bccInputShadeDom!.classList.add("opacity-10");
      });
      bccInputShadeDom!.addEventListener("dragleave", () => {
        bccInputShadeDom!.classList.remove("opacity-10");
        bccInputShadeDom!.classList.add("opacity-0");
      });
    });
  },
  {
    deep: true,
  }
);

// 从 转发/回复/编辑页 点击写邮件页面时，初始化内容
watch(
  () => route.query.type,
  () => {
    if (route.query.type) {
      load();
      model.value.html = "";
    }
  }
);

// 浏览器离开提示
watch(
  () => model.value,
  () => {
    if (initialValue === undefined) return false;
    showBeforeUnload = initialValue !== JSON.stringify(model.value);
  },
  {
    deep: true,
  }
);
onUnmounted(() => {
  clearInterval(timer!);
  // 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
  emailStore.firstActiveMenu = "";
  // 移除监听浏览器关闭的beforeunload事件
  window.removeEventListener("beforeunload", beforeunloadAction);
});

timer = setInterval(() => {
  if (initialValue !== JSON.stringify(model.value)) {
    saveDraft();
  }
}, 20000);

async function load() {
  const sourceId = getQueryString("sourceId");

  if (type === "Drafts" && sourceId) {
    model.value = await compose(sourceId);
  } else if (type === "Forward" && sourceId) {
    model.value = await forward(sourceId, getCurrentTimeZone());
  } else if (type === "Reply" && sourceId) {
    model.value = await reply(sourceId, getCurrentTimeZone());
  } else if (type === "EditAgain" && sourceId) {
    model.value = await reEdit(sourceId);
  } else if (type === "ReplyAll" && sourceId) {
    model.value = await reply(sourceId, getCurrentTimeZone(), "all");
  } else {
    if (!addressFromList.value) return;
    let from;
    let defaultAddress;
    model.value = await compose();
    let allowSentAddressList = addressFromList.value.map((i: any) => i.address);
    if (
      route.query.address &&
      allowSentAddressList.indexOf(route.query.address as string) >= 0
    ) {
      from = addressFromList.value.filter(
        (i: any) => i.address == route.query.address?.toString()
      );
      model.value.from = from[0]?.id;
      model.value.fromAddress = from[0]?.address;
    } else {
      defaultAddress = addressFromList.value.find((f) => f.isDefault);
      model.value.from = defaultAddress
        ? defaultAddress.id
        : addressFromList.value[0]?.id;
      model.value.fromAddress = addressFromList.value[0]?.address;
    }
  }

  if (model.value.from === 0) {
    model.value.from = undefined;
  }
  if (!model.value.cc?.length) {
    model.value.cc = undefined;
  }
  if (!model.value.bcc?.length) {
    model.value.bcc = undefined;
  }
  signature.value = await getSignature(model.value.from!);
  if (type !== "Drafts") {
    changeSignature(["Forward", "Reply", "ReplyAll"].includes(type));
  }
  initialValue = JSON.stringify(model.value);
}

load();

onBeforeRouteLeave((to, from, next) => {
  if (
    initialValue !== JSON.stringify(model.value) &&
    to.name != "login" &&
    checkUnSave.value
  ) {
    showLeaveConfirmDialog.value = true;
    // 离开提示框出现的时候。关闭自动保存草稿的开关
    autoSaveDraftFlag.value = false;

    let toRouteName = to.query?.folder ? to.query.folder : to.name;
    // 从二级菜单的详情页进来时,设置firstActiveMenu的值以便于离开提示出现时激活原来的二级菜单
    emailStore.firstActiveMenu = to.query?.address
      ? toRouteName?.toString() + to.query?.address.toString()
      : to.query?.folderName
      ? toRouteName?.toString() + to.query?.folderName.toString()
      : toRouteName?.toString();
    nextRouter.value.name = to.name as string;
    nextRouter.value.params = to.params;
    nextRouter.value.query = to.query;
    next(false);
  } else if (
    type === "Drafts" &&
    to.path === "/kmail/content" &&
    !to.params.isBreak
  ) {
    to.query.messageId = currentMessageId.value;
    // isBreak设置true,跳出循环
    to.params.isBreak = "true";
    next(to);
  } else {
    next();
  }
});

function closeConfirmDialog() {
  showLeaveConfirmDialog.value = false;
  // 离开提示弹框关闭的时候，重新打开自动保存草稿的开关
  autoSaveDraftFlag.value = true;
  emailStore.firstActiveMenu = route.query?.address
    ? "compose" + route.query?.address
    : "compose";
}

async function addAttachment(e: Event) {
  const file = (e.target as any)?.files[0];
  if (file) {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("filename", file.name);
    // 获取当前已上传的附件的总大小
    let totalSize = model.value.attachments?.reduce(
      (acc, current) => acc + current.size,
      0
    );

    if (totalSize + file.size >= 20 * 1024 * 1024) {
      ElMessage.error(t("common.totalAttachmentsSizeLimitTips"));
      return;
    } else {
      const fileInfo = await postAttachment(formData);
      if (model.value.attachments) {
        model.value.attachments.push(fileInfo);
      } else {
        model.value.attachments = [fileInfo];
      }
    }
  }
}

async function removeAttachment(fileName: string) {
  await deleteAttachment(fileName);
  model.value.attachments = model.value.attachments?.filter(
    (item) => item.fileName !== fileName
  );
}

async function send() {
  if (model.value.from) {
    (model.value as any).fromAddress = addressFromList.value?.find(
      (i: any) => i.id === model.value.from
    )?.address as any;
  }
  if (
    model.value.to.length !== 0 ||
    (model.value.cc?.length && model.value.cc?.length !== 0) ||
    (model.value.bcc?.length && model.value.bcc?.length !== 0)
  ) {
    const valid = await form.value?.validate();

    if (valid) {
      // 防止点击发送邮件的同时自动保存草稿
      autoSaveDraftFlag.value = false;

      model.value.html = model.value.html.replaceAll("<!--signature -->", "");
      // 去掉给签名加上的read-only属性
      model.value.html = model.value.html.replace(
        /<div class="signature"([^>]*)style=["'][^"']*["']([^>]*)>/gi,
        '<div class="signature"$1$2>'
      );
      await sendEmail(model.value);

      // 接口请求完后重新打开自动保存草稿的开关
      autoSaveDraftFlag.value = true;
    }
    // 发送邮件时不进入未保存判断
    checkUnSave.value = false;
    router.replace({
      name: "sent",
      query: {
        address: model.value.fromAddress,
      },
    });
  } else {
    ElMessage.error(t("common.pleaseFillRecipientTips"));
  }
}

async function saveDraft(option?: {
  isLeave?: boolean; //判断保存后是否需要跳转路由，用于离开提示弹框的保存草稿并退出按钮
  showLoading?: boolean; //判断是否需要loading遮罩，自动保存草稿不需要loading动画
}) {
  // 这是为了防止自动保存草稿和手动保存草稿同时发生
  if (!autoSaveDraftFlag.value) return;
  // autoSaveDraftFlag设为false，防止同时触发了自动保存草稿
  autoSaveDraftFlag.value = false;

  const valid = await form.value?.validateField("from");
  if (option?.showLoading) {
    loadingInstance = ElLoading.service({
      background: "rgba(0, 0, 0, 0.5)",
    });
  }

  let id;
  if (valid) {
    id = await postEmail(model.value);
    // 接口请求完后重新打开自动保存草稿的开关
    autoSaveDraftFlag.value = true;

    if (option?.showLoading) {
      loadingInstance?.close();
      ElMessage.success(t("common.saveSuccess"));
    }
    model.value.messageId = id;
    if (type === "Drafts") {
      currentMessageId.value = id;
    }
  }

  // 编辑草稿处保存草稿时，更新url上的id
  if (type === "Drafts") {
    const qs = new URLSearchParams(location.search);
    qs.set("messageId", model.value.messageId!.toString());
    qs.set("sourceId", model.value.messageId!.toString());
    history.replaceState(null, "", location.pathname + "?" + qs.toString());
  }
  initialValue = JSON.stringify(model.value);
  form.value?.clearValidate();
  const time = new Date();
  saveTime.value = time.toString();
  if (option?.isLeave) {
    nextRouter.value.query.messageId = id;
    router.push({
      name: nextRouter.value.name,
      params: nextRouter.value.params,
      query: nextRouter.value.query,
    });
  }
}

function cancelSaveDraft() {
  initialValue = JSON.stringify(model.value);
  router.push({
    name: nextRouter.value.name,
    params: nextRouter.value.params,
    query: nextRouter.value.query,
  });
}

async function cancel() {
  // router.back();
  if (Object.getOwnPropertyNames(route.query).length) {
    delete route.query.type;
    delete route.query.sourceId;
    //如果从二级菜单进入的compose页面，则后退时不传activeMenu路由参数，防止激活了一级菜单
    if (route.query.messageId) {
      if (route.query.address || route.query.oldFolderName) {
        delete route.query.activeMenu;
        router.push({
          name: "mail-content",
          query: {
            ...router.currentRoute.value.query,
            folderName: route.query.oldFolderName ?? undefined,
            folder: route.query.oldActiveMenu,
            messageId: currentMessageId.value,
          },
        });
      } else {
        router.push({
          name: "mail-content",
          query: {
            ...router.currentRoute.value.query,
            folder: route.query.oldActiveMenu,
            activeMenu: route.query.oldActiveMenu,
            messageId: currentMessageId.value,
          },
        });
      }
    } else {
      router.back();
    }
  } else {
    router.push({
      name: "inbox",
    });
  }
}

function alert(options: any) {
  Object.assign(alertDialog.value, options);
  alertDialog.value.show = true;
}

const darkCallbacks: ((value: boolean) => void)[] = [];

const context = {
  send,
  saveDraft,
  cancel,
  alert,
  get dark() {
    return dark.value;
  },

  set dark(value: boolean) {
    toggleDark(value);
  },
  onDark(callback: (value: boolean) => void) {
    darkCallbacks.push(callback);
  },
  get subject() {
    return model.value.subject;
  },
  set subject(value: string) {
    model.value.subject = value;
  },

  get to() {
    return model.value.to;
  },
  set to(value: string[]) {
    model.value.to = value;
  },
  get html() {
    return model.value.html;
  },
  set html(value: string) {
    model.value.html = value;
  },
  get from() {
    return addressFromList.value?.find((f) => f.id == model.value.from)
      ?.address;
  },
  set from(address: string | undefined) {
    const id = addressFromList.value?.find((f) => f.address == address)?.id;
    if (id) model.value.from = id;
  },
};

watch(
  dark,
  async (dark) => {
    for (const callback of darkCallbacks) {
      try {
        callback(dark);
      } catch (error) {
        //
      }
    }
  },
  {
    immediate: true,
  }
);
</script>

<!-- select-input里的tag输入框超出省略号  -->
<style scoped>
:deep(.el-input__inner) {
  @apply ellipsis;
}

:deep(.el-select .el-select__tags-text) {
  display: block;
}

:deep(span.el-tag__content) {
  @apply max-w-300px;
}
</style>
