import {
  KOOBOO_ID,
  K_BINDING,
  K_BINDING_ATTRIBUTE,
  K_BINDING_CONTENT,
  K_COMMENT,
} from "./constants";
import { isComment, isElement } from "@/utils/dom";

import { newGuid } from "@/utils/guid";
import { tryGetPureParent } from "./state";

export const htmlblockSources = ["htmlblock"];
export const domSources = [
  "view",
  "Page",
  "Layout",
  "form",
  ...htmlblockSources,
];
export const contentSources = ["textcontent"];
export const commerceSources = ["commerce"];
export const configSources = ["kconfig"];
export const labelSources = ["Label"];
export const keyValueSources = ["keyvalue"];
export const dbSources = ["indexdb", "sqlite", "mysql", "sqlserver", "mongo"];

export const skipTags = ["html", "body"];

export interface KoobooBinding {
  source: string;
  id: string;
  references?: Node[];
  reference?: Element;
  table?: string;
  fieldName?: string;
  format?: "html" | "text" | "attribute";
  type?: string;
  loop?: "Array" | "Object";
  key?: string;
  group: string;
  target?: "attribute" | "content";
  attribute?: string;
  Expression?: string;
  selfClosed?: boolean;
}

export interface KoobooComment {
  id: string;
  type: "start" | "end";
  bindings?: string;
}

export function getKoobooBindings(el: Element) {
  let current: Node | null = el;
  let skipElement = false;
  const excludes: string[] = [];
  const bindings: KoobooBinding[] = [];

  while (current) {
    if (isComment(current)) {
      const comment = getCommentBinding(current as Comment);

      if (comment) {
        if (comment.type == "start" && !excludes.includes(comment.id)) {
          const commentBindings = JSON.parse(comment.bindings!);
          const references: Node[] = [];
          let currentReference: Node | null = current;

          while (currentReference) {
            references.push(currentReference);
            if (isComment(currentReference)) {
              const commentReference = getCommentBinding(
                currentReference as Comment
              );

              if (commentReference && commentReference.type == "end") {
                if (commentReference.id == comment.id) break;
              }
            }
            currentReference = currentReference.nextSibling;
          }

          const group = newGuid();

          commentBindings.forEach((b: KoobooBinding) => {
            b.references = references;
            b.group = group;
          });

          bindings.push(...commentBindings);
        }

        if (comment.type == "end") {
          excludes.push(comment.id);
        }
      }
    }

    if (isElement(current)) {
      const currentElement = current as Element;

      if (!skipElement) {
        const elementBindings = getElementBinding(currentElement);
        const group = newGuid();

        elementBindings.forEach((b) => {
          b.reference = currentElement;
          b.group = group;
        });

        bindings.push(...elementBindings);
      }
    }

    let next: Node | null = current.previousSibling;

    if (!next) {
      next = current.parentElement;
      skipElement = false;
    } else {
      skipElement = true;
    }

    current = next;
  }

  return bindings;
}

export function isDesignerWidget(el?: Element | null) {
  if (!el) {
    return false;
  }
  if (
    el.getAttribute("data-designer-type") ||
    el.id === "designer-page-style"
  ) {
    return true;
  }
  const bindings = getKoobooBindings(el);
  return bindings[0]?.source === "designer";
}

export function getKoobooId(el: Element) {
  return el.getAttribute(KOOBOO_ID);
}

export function getCommentBinding(comment: Comment) {
  if (!comment.textContent) return;
  if (!comment.textContent.startsWith(K_COMMENT)) return;
  let content = comment.textContent!.substring(K_COMMENT.length + 2);
  const kComment = {} as KoobooComment;

  if (content.startsWith("start")) {
    kComment.type = "start";
  } else if (content.startsWith("end")) {
    kComment.type = "end";
  } else return;

  content = content.substring(content.indexOf("-") + 2);

  if (kComment.type == "start") {
    kComment.id = content.substring(0, content.indexOf("[") - 2);
    kComment.bindings = content.substring(content.indexOf("["));
  } else {
    kComment.id = content.substring(0);
  }

  return kComment;
}

export function getElementBinding(element: Element) {
  const result: KoobooBinding[] = [];

  if (element.hasAttribute(K_BINDING)) {
    result.push(...JSON.parse(element.getAttribute(K_BINDING)!));
  }

  if (element.hasAttribute(K_BINDING_CONTENT)) {
    const list = JSON.parse(element.getAttribute(K_BINDING_CONTENT)!);
    list.forEach((f: KoobooBinding) => (f.target = "content"));
    result.push(...list);
  }

  if (element.hasAttribute(K_BINDING_ATTRIBUTE)) {
    const list = JSON.parse(element.getAttribute(K_BINDING_ATTRIBUTE)!);
    list.forEach((f: KoobooBinding) => (f.target = "attribute"));
    result.push(...list);
  }

  return result;
}

export function getFirstBinding(
  bindings: KoobooBinding[],
  names: string[],
  group = true
) {
  if (!bindings) return;
  if (group) {
    const last = bindings[0];
    if (!last) return;
    const bindingGroup = bindings.filter((f) => f.group == last.group);
    return bindingGroup.find((f) => names.includes(f.source));
  } else {
    return bindings.find((f) => names.includes(f.source));
  }
}

export function getBinding(
  bindings: KoobooBinding[],
  names: string[],
  attribute?: string,
  target?: "attribute" | "content"
) {
  if (attribute) target = "attribute";

  return bindings.find((f) => {
    if (!names.includes(f.source)) return false;
    if (target && f.target != target) return false;

    if (f.attribute == attribute) {
      return true;
    } else {
      return f.target != "attribute";
    }
  });
}

export function containBinding(el: Element, includeSelf?: boolean) {
  const content = includeSelf ? el.outerHTML : el.innerHTML;
  return content.includes(K_BINDING) || content.includes(K_COMMENT);
}

export function isEditableLabel(bindings: KoobooBinding[], el?: HTMLElement) {
  const binding = getBinding(bindings, labelSources);
  if (!binding) return false;
  if (el && binding.reference == el) return false;
  return true;
}

export function isEditableKeyValue(
  bindings: KoobooBinding[],
  el?: Element,
  attribute?: string
) {
  const binding = getBinding(bindings, keyValueSources, attribute);
  if (!binding) return false;
  if (el && binding.reference == el) return false;
  if (attribute && binding.reference != el) return false;
  return true;
}

export function isEditableContent(
  bindings: KoobooBinding[],
  el?: Element,
  attribute?: string
) {
  const binding = getBinding(bindings, contentSources, attribute);
  if (!binding || !binding.id || !binding.fieldName) return false;
  if (el && binding.reference == el) return false;
  if (attribute && binding.reference != el) return false;
  return true;
}

export function isEditableDb(
  bindings: KoobooBinding[],
  el?: Element,
  attribute?: string
) {
  const binding = getBinding(bindings, dbSources, attribute);
  if (!binding || !binding.id || !binding.fieldName) return false;
  if (el && binding.reference == el) return false;
  if (attribute && binding.reference != el) return false;
  return true;
}

export function isEditableConfig(
  bindings: KoobooBinding[],
  el?: Element,
  attribute?: string
) {
  const binding = getBinding(bindings, configSources, attribute ?? "innerHtml");
  attribute == attribute ?? "innerHtml";
  if (!binding || !binding.key) return false;
  if (attribute == "innerHtml") return true;
  if (el && binding.reference == el) return false;
  if (attribute && binding.reference != el) return false;
  return true;
}

export function isEditableDomContent(
  bindings: KoobooBinding[],
  el: HTMLElement
) {
  if (skipTags.includes(el.tagName?.toLowerCase())) return false;
  const binding = getBinding(bindings, domSources);
  if (!binding) return false;
  const pureElement = tryGetPureParent(el, binding);
  if (!pureElement) return false;
  if (containBinding(pureElement.element)) return false;
  const koobooId = getKoobooId(pureElement.element);
  if (!koobooId) return false;
  return true;
}

export function isEditableDomAttribute(
  bindings: KoobooBinding[],
  el: HTMLElement
) {
  if (skipTags.includes(el.tagName?.toLowerCase())) return false;
  const binding = getBinding(bindings, domSources);
  if (!binding) return false;
  const pureElement = tryGetPureParent(el, binding);
  if (!pureElement) return false;

  if (!pureElement.isSelf && containBinding(pureElement.element)) {
    return false;
  }

  const koobooId = getKoobooId(pureElement.element);
  if (!koobooId) return false;
  return true;
}

export function isDynamicContent(bindings: KoobooBinding[], el?: HTMLElement) {
  if (isEditableLabel(bindings, el)) return true;
  if (isEditableKeyValue(bindings, el)) return true;
  if (isEditableContent(bindings, el)) return true;
  if (isEditableDb(bindings, el)) return true;
  if (isEditableConfig(bindings, el)) return true;
}

export function isDynamicAttribute(
  bindings: KoobooBinding[],
  el: Element,
  attribute: string
) {
  if (isEditableKeyValue(bindings, el, attribute)) return true;
  if (isEditableContent(bindings, el, attribute)) return true;
  if (isEditableDb(bindings, el, attribute)) return true;
  if (isEditableConfig(bindings, el, attribute)) return true;
}

export function hasStringFormat(bindings: KoobooBinding[], attribute?: string) {
  const firstGroup = bindings[0]?.group;

  const stringFormatBinding = bindings.find((f) => {
    if (f.source != "Binding" || f.group != firstGroup) return false;
    if (f.Expression?.startsWith("{") && f.Expression?.endsWith("}")) {
      return false;
    }

    if (f.attribute != attribute) return false;
    return true;
  });

  return !!stringFormatBinding;
}

export function isSelfClosed(bindings: KoobooBinding[]) {
  return bindings.some((b) => b.selfClosed);
}
