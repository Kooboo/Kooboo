import { KoobooComment } from "@/kooboo/KoobooComment";

export abstract class Log {
  constructor(public comment: KoobooComment, public koobooId: string) {}
  abstract getCommitObject(): object;
}
