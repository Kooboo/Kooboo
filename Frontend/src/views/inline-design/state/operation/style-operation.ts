import type { Operation } from ".";
import type { Change } from "../change";

export function createStyleOperation<T extends Change>(
  rule: CSSStyleRule,
  property: string
): Operation<T> {
  const originContent = rule.style.getPropertyValue(property);
  const originContentPriority = rule.style.getPropertyPriority(property);
  let newContent: string;
  let newContentPriority: string;

  const undo = () => {
    newContent = rule.style.getPropertyValue(property);
    newContentPriority = rule.style.getPropertyPriority(property);
    rule.style.setProperty(property, originContent, originContentPriority);
  };

  const redo = () => {
    rule.style.setProperty(property, newContent, newContentPriority);
  };

  return {
    changes: [],
    undo,
    redo,
  };
}
