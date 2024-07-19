interface AlertOptions {
  title: string;
  body: string;
  width: string;
}

interface CurrentMail {
  send: () => void;
  saveDraft: () => void;
  cancel: () => void;
  alert: (options: AlertOptions) => void;
  subject: string;
  to: string;
  html: string;
  from: string;
  dark: boolean;
  onDark: (value: boolean) => void;
}

namespace KScript {
  declare interface k {
    currentMail: CurrentMail;
  }
}

// declare const currentMail: CurrentMail;
