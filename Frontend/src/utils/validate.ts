import type { ComputedRef, Ref } from "vue";
import type { DatabaseColumn, DatabaseType } from "@/api/database";

import type { Domain } from "@/api/transfer/types";
import type { KeyValue } from "@/global/types";
import type { Property } from "@/global/control-type";
import type { Rule } from "async-validator";
import { isUniqueName as addressIsUniqueName } from "@/api/mail";
import { isUniqueName as authorizeNameIsUniqueName } from "@/api/openapi";
import { checkDomainBindingAvailable } from "@/api/transfer";
import { i18n } from "@/modules/i18n";
import { isUniqueEmailName } from "@/api/user/index";
import { isUniqueName } from "@/api/binding";
import { isUniqueTableName } from "@/api/database";
import validator from "async-validator";
import { getEmailAddress } from "./get-email-address";

const $t = i18n.global.t;

export function requiredRule(message: string, trim = true) {
  return {
    required: true,
    validator: (
      rule: any,
      value: any,
      callback: any,
      source: any,
      options: any
    ) => {
      if (trim && typeof value === "string") {
        value = value.trim();
      }
      validator.validators.required(rule, value, callback, source, options);
    },
    message: () => message,
    trigger: "blur",
  };
}

export function loginRequiredRule(message: string, trim = true) {
  return {
    required: true,
    validator: (
      rule: any,
      value: any,
      callback: any,
      source: any,
      options: any
    ) => {
      if (trim && typeof value === "string") {
        value = value.trim();
      }
      validator.validators.required(rule, value, callback, source, options);
    },
    message: () => message,
    trigger: "change",
  };
}

export function rangeRule(min: number, max: number) {
  return {
    min: min,
    max: max,
    message: $t("common.rangeLengthTips", { min: min, max: max }),
  };
}

export const frontEmailRule = {
  pattern: /^[a-zA-Z0-9.+_-]{1,}$/g,
  message: $t("common.inputCorrectEmailTips"),
  trigger: "blur",
};

export const wildcardEmailRule = {
  pattern: /^[A-Za-z0-9.+_-]*\*?[A-Za-z0-9.+_-]*$/,
  message: $t("common.inputCorrectEmailTips"),
  trigger: "blur",
};

export function emailRangeRule(max: number) {
  return {
    min: 1,
    max: max,
    message: $t("common.emailRangeTips", { max }),
    trigger: ["change", "blur"],
  };
}

export function simpleNameRule(
  message = $t("common.onlyLetterAndDigitsAllowedTips")
) {
  return {
    pattern: /^([a-zA-Z\d\-\.\_])*$/,
    message: () => message,
  };
}

export function domainSearchRule(message = $t("common.domainSearchTips")) {
  return {
    pattern: /^([a-zA-Z\d\-\.])*$/,
    message: () => message,
  };
}

export function notAllowMultilevelDomain(
  message = $t("common.atMostOnePointCharacter")
) {
  return {
    pattern: /^[A-Za-z0-9-]*\.?[A-Za-z0-9-]*$/,
    message: () => message,
  };
}

export function letterAndDigitStartRule(
  message = $t("common.letterAndDigitsAllowedToStarting")
) {
  return {
    pattern: /^([a-zA-Z])([a-zA-Z\d\_])*$/,
    message: () => message,
  };
}

export function urlRule(message: string) {
  return {
    pattern:
      /[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/,
    message: () => message,
    trigger: "blur",
  };
}

export function urlAndIpRule(message: string) {
  return {
    pattern: /[-a-zA-Z0-9@:%._\+~#=]{2,256}\.([-a-zA-Z0-9@:%_\+.~#?&//=]*)/,
    message: () => message,
  };
}

export function putIntegerNumberRule() {
  return {
    pattern: /^[0-9]*$/,
    message: () => $t("common.enterIntegerTips"),
  };
}

export function expiresInRule(message: string) {
  return {
    pattern: /^[1-9]*$/,
    message: () => message,
  };
}

export const urlPathRule = {
  pattern:
    /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\[|\]|\;|\:|\"|\'|\,|\<|\>|\?]*$/,
  message: $t("common.urlInvalid"),
};

export const passwordRule = {
  pattern: /^(?=(?:.*[a-zA-Z\d]))(?=(?:.*[a-zA-Z\W_]))(?=(?:.*[\d\W_])).{3,}$/,
  message: $t("common.passwordMustContainTwoOfLettersNumbersAndSymbols"),
  trigger: "blur",
};

export const passwordLengthRule = {
  min: 8,
  max: 30,
  message: $t("common.passwordLengthLimitTips"),
  trigger: "blur",
};

export const subDomainRule = {
  pattern: /^([A-Za-z0-9][A-Za-z0-9\-]{0,})*[A-Za-z0-9]$/,
  message: $t("common.subDomainInvalidTips"),
};

export const DomainRule = {
  pattern: /^[A-Za-z][\w\-]*$/,
  message: $t("common.domainInvalid"),
};

export const hostRecordRule = {
  pattern: /^(?!\.)(?!.*\.$)[a-zA-Z0-9-_.]+$/,
  message: $t("common.domainInvalid"),
};

export const HttpCodeRule = {
  pattern: /^[12345][0-9]{2}$/,
  message: $t("common.httpCodeInvalidTips"),
};

export function portRule() {
  return {
    min: 0,
    max: 65535,
    // pattern: /^[0-9]*$/,
    type: "number",
    message: $t("common.portRangeTips"),
  };
}

export function frequenceUnitRule() {
  return {
    min: 1,
    type: "number",
    message: $t("common.inputIntegerValueTips"),
  };
}

export const usernameRule = {
  pattern: /^[a-zA-Z0-9-]{0,}$/,
  message: $t("common.simpleNameTips"),
  trigger: "blur",
};

export const usernameStartAndEndRule = {
  pattern: /^[a-zA-Z0-9].*[a-zA-Z0-9]$/,
  message: $t("common.usernameMustStartAndEndWithALetterOrDigit"),
  trigger: "blur",
};

export const nameLengthRule = {
  min: 5,
  max: 30,
  message: $t("common.usernameRuleTips"),
  trigger: "blur",
};

export const usernameRules = [
  nameLengthRule,
  usernameRule,
  usernameStartAndEndRule,
  requiredRule($t("common.inputUsernameTips")),
];

export const emailRule = {
  pattern: /^[a-zA-Z0-9.+_-]{1,}@[a-z0-9-]{1,}[a-z0-9](\.[a-z]{1,})+$/g,
  message: $t("common.inputCorrectEmailTips"),
  trigger: "blur",
};

export const phoneRule = {
  pattern: /^\d*$/,
  message: $t("common.inputCorrectPhoneTips"),
  trigger: "blur",
} as Rule;

export const codeRule = {
  pattern: /^\d{4}$/,
  message: $t("common.inputCorrectSecurityCodeTips"),
  trigger: "blur",
} as Rule;

export const folderNameRule = {
  pattern: /^([a-zA-Z\d\-\.\_\u4e00-\u9fa5])*$/,
  message: $t("common.folderNameRuleTips"),
};

export const confirmPasswordRule = (model: {
  password: string;
  confirmPassword: string;
}) => {
  return {
    required: true,
    validator(
      validator: string,
      value: string,
      callback: (msg?: string) => void
    ) {
      if (model.confirmPassword.length > 0) {
        if (value !== model.password) {
          callback($t("common.enteredPasswordsDiffer"));
        }
        callback();
      }
    },
    trigger: "blur",
  };
};

export const folderIsUniqueNameRule = (folders: Ref<{ key: string }[]>) => {
  return {
    validator(_: string, value: string, callback: (error?: Error) => void) {
      if (
        folders.value.some((s) => s.key.toUpperCase() === value.toUpperCase())
      ) {
        callback(new Error($t("common.folderNameExistsTips")));
      } else {
        callback();
      }
    },
    trigger: "blur",
  };
};

export const isUniqueCommerceNameRule = (
  action: (name: string, id?: string) => Promise<unknown>,
  message: string,
  id?: string
) => {
  return {
    async asyncValidator(
      _: any,
      value: string,
      callback: (arg0: Error) => void
    ) {
      try {
        await action(value, id);
      } catch (error) {
        callback(Error(message));
      }
    },
    trigger: "blur",
  };
};

export const isUniqueNameRule = (
  action: (name: string) => Promise<unknown>,
  message: string
) => {
  return {
    async asyncValidator(
      _: string,
      value: string,
      callback: (arg0: Error) => void
    ) {
      try {
        await action(value);
      } catch (error) {
        callback(Error(message));
      }
    },
    trigger: "blur",
  };
};

export const addressIsUniqueNameRule = (model: any) => {
  return {
    async asyncValidator(
      _rule: unknown,
      value: string,
      callback: (error?: Error) => void
    ) {
      try {
        await addressIsUniqueName(model);
        callback();
      } catch (error) {
        callback(Error($t("common.emailAddressExistsTips")));
      }
    },
    trigger: "blur",
  };
};

export const authorizeNameIsUniqueNameRule = (name: string) => {
  return {
    async asyncValidator(
      _rule: unknown,
      value: string,
      callback: (error?: Error) => void
    ) {
      try {
        await authorizeNameIsUniqueName(name);
        callback();
      } catch (error) {
        callback(Error($t("common.authorizeNameExistsTips")));
      }
    },
    trigger: "blur",
  };
};

export const checkDomainIsUniqueNameRule = (model: Domain) => {
  return {
    async asyncValidator(
      _: string,
      value: string,
      callback: (arg0: Error) => void
    ) {
      try {
        await checkDomainBindingAvailable(model);
      } catch (error) {
        callback(Error($t("common.domainAlreadyExists")));
      }
    },
    trigger: "blur",
  };
};

export const tableIsUniqueNameRule = (dbType: DatabaseType) => {
  return {
    async asyncValidator(
      _rule: unknown,
      value: string,
      callback: (error?: Error) => void
    ) {
      try {
        await isUniqueTableName(dbType, value);
        callback();
      } catch (error) {
        callback(Error($t("common.valueHasBeenTakenTips")));
      }
    },
    trigger: "blur",
  };
};

export const domainBindingIsUniqueNameRule = (model: {
  subDomain: string;
  rootDomain: string;
  port?: number;
  defaultBinding?: boolean;
}) => {
  return {
    async asyncValidator(
      _rule: unknown,
      value: string,
      callback: (error?: Error) => void
    ) {
      try {
        await isUniqueName(model.subDomain + "." + model.rootDomain);
        callback();
      } catch (error) {
        callback(Error($t("common.siteIsBoundTips")));
      }
    },
    trigger: "blur",
  };
};

export const editColumIsUniqueNameRule = (
  isNew: ComputedRef<boolean>,
  isOld: Ref<Property[]> | Ref<DatabaseColumn[]>
) => {
  return {
    validator(_rule: unknown, value: string) {
      if (!isNew.value) return true;
      return !isOld.value.some(
        (f: { name: string }) => f.name.toLowerCase() === value.toLowerCase()
      );
    },
    message: () => $t("common.valueHasBeenTakenTips"),
  };
};

export const toEmailRule = () => {
  return {
    validator(
      validator: string,
      value: string[],
      callback: (msg?: string) => void
    ) {
      for (const i of value) {
        if (
          !/^[\"]{0,}[\s\S]{1,}[\"]{0,}[\<]{0,}[a-zA-Z0-9.+_-]{0,}@[A-Za-z0-9-_]{1,}(\.[a-z-_]{1,}[\>]{0,})+$/g.test(
            i
          )
        ) {
          callback($t("common.inputCorrectEmailTips"));
          return;
        }
      }
      callback();
    },
    trigger: "change",
  };
};

export const emailAddressLengthRule = (
  model: {
    local: string;
    domain: string;
  },
  max: number
) => {
  return {
    required: true,
    validator(
      validator: string,
      value: string,
      callback: (msg?: string) => void
    ) {
      if (
        (model.local + "@" + model.domain).length > max &&
        model.local.length < 64
      ) {
        callback($t("common.addressMaximumLength", { max: max }));
        return;
      }
      callback();
    },
    trigger: ["change", "blur"],
  };
};

export const commerceSelectInputLengthRule = () => {
  return {
    validator(
      validator: string,
      value: KeyValue[] | any[],
      callback: (msg?: string) => void
    ) {
      for (const i of value) {
        if (i.value.length > 30) {
          callback($t(`common.rangeLengthTips`, { min: 1, max: 30 }));
          return;
        }
      }
      callback();
    },
    trigger: "change",
  };
};

export function commerceValueRangeRule(
  value: string,
  min: number,
  max: number
) {
  return {
    value,
    min,
    max,
    message: $t(`common.commonValueLengthTips`, { value, min, max }),
    trigger: "blur",
  };
}

export const isUniquePhoneRule = (regionCode: string, phone: string) => {
  return {
    async asyncValidator(
      _rule: unknown,
      value: string,
      callback: (error?: Error) => void
    ) {
      if (regionCode + value === phone) {
        callback(
          Error($t("common.phoneNumberCannotBeTheSameAsTheCurrentBound"))
        );
      } else {
        callback();
      }
    },
    trigger: ["change", "blur"],
  };
};

export const isUniqueEmailRule = (email: string) => {
  return {
    async asyncValidator(
      _rule: unknown,
      value: string,
      callback: (error?: Error) => void
    ) {
      if (value === email) {
        callback(Error($t("common.emailCannotBeTheSameAsTheCurrentBound")));
      } else {
        try {
          await isUniqueEmailName(value);
          callback();
        } catch {
          callback(Error($t("common.theEmailAlreadyExists")));
        }
      }
    },
    trigger: ["change", "blur"],
  };
};

export function cvcRangeRule() {
  return {
    min: 3,
    max: 3,
    message: $t(`common.pleaseEnterTheCorrectFormat`),
    trigger: "blur",
  };
}

export function rechargeRule() {
  return {
    min: 10,
    type: "number",
    message: $t("common.minimumOfTopupAmount"),
    trigger: "blur",
  };
}

export const availableInviteEmailAddressRule = (addresses: string[]) => {
  return {
    validator(_: string, value: string[], callback: (error?: Error) => void) {
      let valid = true;
      value.forEach((f) => {
        if (
          addresses.some(
            (s) =>
              s.toLocaleLowerCase() === getEmailAddress(f).toLocaleLowerCase()
          )
        ) {
          valid = false;
        }
      });
      if (valid) {
        callback();
      } else {
        callback(
          new Error($t("common.participantShouldNotBelongToThisAccount"))
        );
      }
    },
    trigger: "change",
  };
};

export const contactUniqueAddressRule = (list: any[], msg: string) => {
  return {
    validator(_: string, value: string, callback: (arg0?: Error) => void) {
      if (list.some((s) => s.toLowerCase() === value.toLowerCase())) {
        callback(new Error(msg));
      } else {
        callback();
      }
    },
    trigger: "blur",
  };
};
