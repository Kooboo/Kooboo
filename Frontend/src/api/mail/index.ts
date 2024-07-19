import type {
  Address,
  Attachment,
  Contact,
  EmailDraft,
  EmailItem,
  EmailMessage,
  EmailMessageCalendar,
  EmailMigration,
  EmailSearchModel,
  PostAddress,
  ScheduleAPI,
  Smpt,
} from "./types";

import type { EmailFolder } from "./types";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";

const $t = i18n.global.t;
const FolderName: {
  inbox: string;
  sent: string;
  trash: string;
  drafts: string;
  spam: string;
  folder: string;
  [key: string]: string;
} = {
  inbox: $t("common.inbox"),
  sent: $t("mail.sent"),
  drafts: $t("common.drafts"),
  trash: $t("common.trash"),
  spam: $t("common.spam"),
  folder: $t("common.folders"),
};

const returnFolderName = (name: string): string => {
  const lastPart = name.split("/").pop()!;
  return FolderName[lastPart] || lastPart;
};
/* EmailAddress */

export const isUniqueName = (params: any) =>
  request.get(
    "/EmailAddress/isUniqueName",
    {
      local: params.local,
      addressType: params.addressType,
      domain: params.domain,
    },
    { hiddenError: true, hiddenLoading: true }
  );

export const getAddressList = (folder?: string, hiddenLoading?: boolean) =>
  request.get<Address[]>(
    "EmailAddress/list",
    { folder },
    hiddenLoading ? { hiddenLoading: true } : undefined
  );

export const getFromList = (messageId: string) =>
  request.get<Address[]>("EmailAddress/fromlist", { messageId });

export const getCompleteAddressList = (part: string) =>
  request.get<string[]>(
    "EmailAddress/AddressComplete",
    { part },
    { hiddenLoading: true }
  );

export const postAddress = (body: PostAddress) =>
  request.post("/EmailAddress/post", body, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const updateName = (id: number, name: string) =>
  request.post("/EmailAddress/updateName", { id, name }, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deleteAddress = (ids: number[]) =>
  request.post("/EmailAddress/Deletes", { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

/* EmailDraft */
// 返回收件人建议列表
export const getTargetAddresses = () =>
  request.get<string[]>("/EmailDraft/TargetAddresses");

// compose
export const compose = (messageId?: number | string) =>
  request.get<EmailDraft>("/EmailDraft/Compose", { messageId });

//reEdit
export const reEdit = (messageId?: number | string) =>
  request.get<EmailDraft>("/EmailMessage/ReEdit", { messageId });

// reply
export const reply = (
  sourceId: number | string,
  timeZoneId: string,
  type?: string
) =>
  request.get<EmailDraft>(
    "EmailMessage/Reply",
    { sourceId, timeZoneId, type },
    { hiddenError: true }
  );

// post
export const postEmail = (body: EmailDraft) =>
  request.post<number>("/EmailDraft/Post", body, undefined, {
    hiddenLoading: true,
  });

// forward
export const forward = (sourceId: number | string, timeZoneId: string) =>
  request.get<EmailDraft>(
    "/EmailMessage/Forward",
    { sourceId, timeZoneId },
    { hiddenError: true }
  );

export const updateForward = (id: number | string, forwardAddress: string) =>
  request.post("/EmailAddress/UpdateForward", {
    id,
    forwardAddress,
  });

export const memberList = (addressId: number | string) =>
  request.post<string[]>("/EmailAddress/MemberList", {
    addressId,
  });

export const memberPost = (addressId: number | string, MemberAddress: string) =>
  request.post(
    "/EmailAddress/MemberPost",
    {
      addressId,
      MemberAddress,
    },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );

export const MemberDelete = (
  addressId: number | string,
  memberAddress: string
) =>
  request.post<EmailDraft>("/EmailAddress/MemberDelete", {
    addressId,
    memberAddress,
  });
/* EmailMessage */
export const sendEmail = (body: EmailDraft) =>
  request.post("/EmailMessage/Send", body, undefined, {
    successMessage: $t("common.sendSuccess"),
  });

export const deleteEmail = (ids: number[]) =>
  request.post("/EmailMessage/Deletes", { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getList = (folder: string, address = "") =>
  request.get<EmailItem[]>("/EmailMessage/list", { folder, address });
export const getMore = (
  folder: string,
  address = "",
  messageId: number,
  keyword: string,
  hiddenLoading?: boolean
) =>
  request.get<EmailItem[]>(
    "/EmailMessage/more",
    {
      folder: encodeURIComponent(folder),
      address: encodeURIComponent(address),
      messageId,
      keyword,
    },
    hiddenLoading
      ? {
          hiddenLoading: true,
        }
      : undefined
  );

export const advancedSearchMail = (
  messageId: number,
  model: EmailSearchModel
) =>
  request.post<EmailItem[]>("/EmailMessage/AdvancedSearch", {
    messageId,
    model,
  });

export const LatestMsgId = () =>
  request.get<number>("/EmailMessage/LatestMsgId", undefined, {
    hiddenLoading: true,
    hiddenError: true,
  });

export const getContent = (messageId: number | string) =>
  request.get<EmailMessage>(
    "/EmailMessage/Content",
    { messageId },
    {
      errorMessage: $t("common.emailNotFound"),
    }
  );

export const viewSource = (messageId: number | string) =>
  request.get<EmailMessage>("/EmailMessage/viewsource", { messageId });

export const markReads = (ids: number[], value: boolean) =>
  request.post("/EmailMessage/MarkReads", { ids, value });

export const moveEmail = (ids: number[] | number, folder: string) =>
  request.post("/EmailMessage/Moves", { ids, folder }, undefined, {
    successMessage:
      folder === "trash"
        ? $t("common.deleteSuccess")
        : $t("common.haveMoveToFolder", {
            folder: returnFolderName(folder),
          }),
  });

/* EmailAttachment */
export const postAttachment = (body: unknown) =>
  request.post<Attachment>("/EmailAttachment/AttachmentPost", body, undefined, {
    headers: { "Content-Type": "multipart/form-data" },
  });

export const deleteAttachment = (filename: string) =>
  request.post("/EmailAttachment/DeleteAttachment", { filename });

export const dmarcReport = () => request.post<any[]>("/mailreport/DmarcList");

export const getImapSetting = () => request.get<any[]>("/mailsetting/imap");
export const getBimi = () => request.get<any[]>("/mailsetting/bimi");

export const updateLogoApi = (url: string) =>
  request.post("/mailsetting/updateLogo", { url }, undefined, {
    successMessage: $t("common.updateSuccess"),
  });

export const updateVmcApi = (url: string) =>
  request.post("/mailsetting/updateVMC", { url }, undefined, {
    successMessage: $t("common.updateSuccess"),
  });

export const getEmailCalendar = (start: string, end?: string) =>
  request.get<ScheduleAPI[]>("/EmailCalendar/CalendarSchedules", {
    start,
    end,
  });
export const addScheduleApi = (body: ScheduleAPI) =>
  request.post("/EmailCalendar/SaveSchedule", body, undefined, {
    successMessage: $t("common.addSuccess"),
    hiddenError: true,
  });

export const deleteScheduleApi = (id: string, organizer: string) =>
  request.post(
    "/EmailCalendar/DeleteSchedule",
    {
      id,
      organizer,
      isNotify: true,
    },
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );
export const editScheduleApi = (body: ScheduleAPI) =>
  request.post("/EmailCalendar/EditSchedule", body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const inviteDealingAPI = (
  id: number,
  inviteConfirm: number,
  calendar: EmailMessageCalendar[],
  sender: string
) =>
  request.post("/EmailCalendar/InviteDealing", {
    id,
    inviteConfirm,
    calendar,
    sender,
  });

export const getContactListApi = () =>
  request.get<Contact[]>("/EmailAddress/ContactList");

export const addContactApi = (body: any) =>
  request.post<Contact[]>("/EmailAddress/ContactPost", body, undefined, {
    successMessage: $t("common.addSuccess"),
  });

export const updateContactApi = (body: any) =>
  request.post<Contact[]>("/EmailAddress/ContactUpdate", body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deleteContactApi = (data: { ids: string[] }) =>
  request.post("/EmailAddress/ContactDeletes", data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const setDefaultSender = (address: string) =>
  request.post("/EmailAddress/SetDefaultSender", { address }, undefined, {
    successMessage: $t("common.theDefaultAddressUpdated"),
  });

export const getSignature = (addressId: number) =>
  request.get("/EmailAddress/GetSignature", { addressId });

export const updateSignature = (addressId: number, signature: string) =>
  request.post(
    "/EmailAddress/updateSignature",
    { addressId, signature },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );

export const setAuthorizationCode = (address: string) =>
  request.post("EmailAddress/SetAuthorizationCode", { address });

export const getEmailFolderList = () =>
  request.get<EmailFolder[]>("EmailFolder/list");
export const newEmailFolder = (folderName: string) =>
  request.post("EmailFolder/create", { folderName }, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
export const renameEmailFolder = (folderName: string, newName: string) =>
  request.post("EmailFolder/rename", { folderName, newName }, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
export const deleteEmailFolder = (folderId: number) =>
  request.post("EmailFolder/delete", { folderId }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const addEmailMigrationJob = (job: EmailMigration) =>
  request.post("EmailMigration/AddJob", job, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getEmailMigrationJobs = (hiddenLoading = false) =>
  request.get<EmailMigration[]>("EmailMigration/GetJobs", undefined, {
    hiddenLoading,
  });
export const runJob = (id: string) =>
  request.post("EmailMigration/RunJob", { id });

export const cancelJob = (id: string) =>
  request.post("EmailMigration/CancelJob", { id });
export const deleteJob = (id: string) =>
  request.post("EmailMigration/deleteJob", { id }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const updateEmailMigrationJob = (id: string, body: EmailMigration) =>
  request.post("EmailMigration/UpdateJob?id=" + id, body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getSmtpSetting = () => request.get<Smpt>("mailsetting/smtpget");
export const updateSmtpSetting = (body: any) =>
  request.post<Smpt>("mailsetting/smtpupdate", { ...body }, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
