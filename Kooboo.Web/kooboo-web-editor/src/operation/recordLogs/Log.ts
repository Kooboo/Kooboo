import { KoobooComment } from "@/kooboo/KoobooComment";

export abstract class Log {
  constructor(
    public comment: KoobooComment,
    public koobooId: string,
    public element: HTMLElement
  ) {}
  abstract getCommitObject(): object;
}

// string EditorType { get; }

// string ObjectType { get; set; }

// ActionType Action { get; set; }

// string NameOrId { get; set; }

// string Value { get; set; }
