export type AddressType = "Normal" | "Wildcard" | "Group" | "Forward";

export interface PostAddress {
  local: string;
  addressType: AddressType | Address;
  domain: string;
  forwardAddress?: string;
  name: string;
}

export interface Address {
  name: string;
  address: string;
  id: number;
  userId: string;
  orgId: string;
  forwardAddress: string;
  addressType: AddressType;
  count: number;
  unRead: number;
  displayName: string;
  isDefault: boolean;
}

/* EmailDraft */
export interface EmailDraft {
  [key: string]: any;
  subject: string;
  from?: number;
  fromAddress: string;
  to: string[];
  cc?: string[];
  bcc?: string[];
  attachments?: Attachment[];
  html: string;
  date: string;
  messageId?: number;
}

/* EmailMessage */

export interface EmailInfo {
  name: string;
  address: string;
}

export interface EmailMessageCalendar {
  start: string;
  end: string;
  summary: string;
  description: string;
  isExpired: boolean;
  organizer: string;
  attendees: string[];
  uid: string;
  location: string;
}
export interface EmailMessage {
  id: number;
  subject: string;
  from: EmailInfo;
  to: EmailInfo[];
  cc: EmailInfo[];
  bcc: EmailInfo[];
  attachments?: Attachment[];
  html: string;
  date: string;
  folderName: string;
  downloadAttachment: string;
  inviteConfirm: number;
  calendar: EmailMessageCalendar[];
}

export interface DeliveryLog {
  to: string;
  isSuccess: boolean;
  isSending: boolean;
  log: string;
  deliveryTime: string;
}

// 邮件列表
export interface EmailItem {
  id: number;
  smtpMessageId: null;
  userId: string;
  addressId: number;
  outGoing: boolean;
  folderId: number;
  folderName: string;
  addressName: string;
  mailFrom: null;
  rcptTo: null;
  from: { name: string; address: string };
  to: { name: string; address: string }[];
  cc: null;
  bcc: null;
  subject: string;
  bodyPosition: number;
  summary: null;
  size: number;
  read: boolean;
  answered: boolean;
  deleted: boolean;
  flagged: boolean;
  recent: boolean;
  creationTime: string;
  creationTimeTick: number;
  date: string;
  draft: boolean;
  attachments: Attachment[];
  hasAttachment: boolean;
  el: any;
  deliveryLog?: {
    isSuccess: boolean;
    isSending: boolean;
    items: DeliveryLog[];
  };
}

export interface Emails {
  data: EmailItem[];
}

/* EmailAttachment */
export interface Attachment {
  fileName: string;
  size: number;
  subType: string | null;
  type: string | null;
  downloadUrl: string;
}

export interface MailModule {
  id: string;
  name: string;
  settings: string;
  taskJs: string;
  online: string;
}

export interface ScheduleAPI {
  id?: string;
  calendarTitle: string;
  start: string;
  end: string;
  mark: string;
  location: string;
  contact: string[];
  organizer: string;
  isOrganizer?: boolean;
  isNotify: boolean;
  addContact?: string[];
  removeContact?: string[];
  acceptCount?: number;
  awaitingCount?: number;
  tentativeCount?: number;
  totalCount?: number;
  declineCount?: number;
  attendeeStatus?: { address: string; participationStatus: number }[];
}

export interface Schedule {
  id?: string;
  calendarTitle: string;
  startDate: string;
  startTime: string;
  endDate: string;
  endTime: string;
  mark: string;
  location: string;
  contact: string[];
  organizer: string;
  isNotify: boolean;
  isOrganizer: boolean;
}

export interface Contact {
  id?: string;
  name: string;
  address: string;
  fullAddress: string;
  userId: string;
}

export interface SearchResult {
  packageId: string;
  title: string;
  description: string;
  loading?: boolean;
  installed?: boolean;
}

export interface EmailFolder {
  id: number;
  name: string;
  count: number;
  displayName: string;
  unRead: number;
  floor?: number;
  items?: EmailFolder[];
}

export interface EmailMigration {
  id?: string;
  active?: boolean;
  name: string;
  emailAddress: string;
  host: string;
  forceSSL: boolean;
  port: number;
  password?: string;
  startIndex?: number;
  addressId?: number;
  errorMessage?: string;
}

export interface EmailSearchModel {
  keyword: string;
  position: string;
  from: string;
  to: string;
  dateType: number;
  startDate: string;
  endDate: string;
  searchFolder: string;
  readOrUnread: number;
}

export interface SelectedEmail {
  id: number;
  read: boolean;
  folderName?: string;
}

export interface Smpt {
  server: string;
  userName: string;
  password: string;
  port: number;
}
