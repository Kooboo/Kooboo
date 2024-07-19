import type { Operation } from ".";
import { getElementById } from ".";
import type { Change } from "../change";

export function createDomOperation<T extends Change>(
  id: string,
  originContent: string
): Operation<T> {
  let newContent: string;

  const undo = () => {
    const el = getElementById(id);

    if (el) {
      newContent = el.innerHTML;
      el.innerHTML = originContent;
    }
  };

  const redo = () => {
    const el = getElementById(id);

    if (el) {
      el.innerHTML = newContent;
    }
  };

  return {
    changes: [],
    undo,
    redo,
  };
}
