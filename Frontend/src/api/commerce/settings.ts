import type { FieldValidation } from "@/global/control-type";
import type { KeyValue } from "@/global/types";
import type { fieldTypes } from "@/views/commerce/custom-data/custom-field";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import type { EarnPointSettings, RedeemPointSettings } from "./loyalty";

const $t = i18n.global.t;

export interface Currency {
  name: string;
  symbol: string;
  symbolNative: string;
  code: string;
  namePlural: string;
}

export interface CustomField {
  name: string;
  displayName: string;
  type: keyof typeof fieldTypes;
  editable: boolean;
  multilingual: boolean;
  multiple?: boolean;
  allowRepetition?: boolean;
  contentFolder?: string;
  validations: FieldValidation[];
  selectionOptions: KeyValue[];
  isSystemField: boolean;
  isSummaryField: boolean;
  options?: Record<string, any>;
}

export interface EmailNotification {
  event: string;
  subjectTemplate: string;
  bodyTemplate: string;
  sendToCustomer: boolean;
  sendToAddresses: string[];
}

export interface Webhook {
  event: string;
  url: string;
}

export interface WebhookEvent {
  name: string;
  display: string;
  description: string;
}

export interface EmailEvent extends WebhookEvent {
  mailSubjectTemplate: string;
  mailBodyTemplate: string;
}

export interface SmtpSetting {
  server: string;
  port: string;
  ssl: boolean;
  userName: string;
  password: string;
}

export interface Settings {
  currencyCode: string;
  currencySymbol: string;
  shippingCost: number;
  weightUnit: string;
  payments: string[];
  productCustomFields: CustomField[];
  categoryCustomFields: CustomField[];
  enableEmailNotification: boolean;
  koobooEmailAddress: string;
  customMailServer?: SmtpSetting;
  mailServerType: string;
  emailNotifications: [];
  enableWebhook: boolean;
  webhooks: [];
  webhookSecret?: string;
  earnPoint: EarnPointSettings;
  redeemPoint: RedeemPointSettings;
}

export const getCurrencies = () =>
  request.get<Currency[]>(useUrlSiteId("CommerceSettings/currencies"));

export const getPayments = () =>
  request.get<KeyValue[]>(useUrlSiteId("CommerceSettings/payments"));

export const getEmailEvents = () =>
  request.get<EmailEvent[]>(useUrlSiteId("CommerceSettings/EmailEvents"));

export const getWebhookEvents = () =>
  request.get<WebhookEvent[]>(useUrlSiteId("CommerceSettings/WebhookEvents"));

export const getEmailLogs = (params: any) =>
  request.get<any>(useUrlSiteId("CommerceSettings/EmailLogs"), params);

export const getWebhookLogs = (params: any) =>
  request.get<any>(useUrlSiteId("CommerceSettings/WebhookLogs"), params);

export const emailPreview = (body: unknown) =>
  request.post<EmailNotification>(
    useUrlSiteId("CommerceSettings/EmailPreview"),
    body
  );

export const getSettings = () =>
  request.get<Settings>(useUrlSiteId("CommerceSettings/get"));

export const saveSettings = (settings: Settings) =>
  request.post(useUrlSiteId("CommerceSettings/edit"), settings, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
