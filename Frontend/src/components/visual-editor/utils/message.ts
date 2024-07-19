import type {
  InUpdateDomContextType,
  MessageIframe,
  MessageWidget,
  VeDraggableElement,
  VeWidgetType,
} from "../types";
import { off, on } from "@/utils/dom";

import { cloneDeep } from "lodash-es";

export function postIFrameMessage(data: MessageIframe<any>, selector?: string) {
  const ifr: HTMLIFrameElement = document.querySelector(
    selector ?? "#ve-iframe"
  )!;
  if (!ifr) {
    throw "Iframe not found";
  }

  ifr.contentWindow!.postMessage(cloneDeep(data), "*");
}

export function postInUpdateDomMessage(ctx: InUpdateDomContextType) {
  const updateDomData: MessageIframe<InUpdateDomContextType> = {
    type: "in-update-dom",
    group: ctx.group ?? "",
    context: ctx,
  };
  postIFrameMessage(updateDomData);
}

export function postInDragEnd() {
  const data: MessageIframe<void> = {
    type: "in-reset-drag",
    context: undefined,
  };
  postIFrameMessage(data);
}

export function postInDragStart(ctx: {
  item: VeDraggableElement;
  originalEvent: DragEvent;
}) {
  const item: VeWidgetType | undefined = ctx.item?.__draggable_context?.element;
  const data: MessageIframe<VeWidgetType> = {
    type: "in-dragging-item",
    context: item,
  };
  postIFrameMessage(data);
}

const eventSets: Record<string, Set<(m: MessageWidget, e: Event) => void>> = {};

const messagePrefix = "ve-";
export function handleMessage(
  events: Record<string, (m: MessageWidget, e: Event) => void>
) {
  for (const key in events) {
    if (Object.prototype.hasOwnProperty.call(events, key)) {
      if (!eventSets[key]) {
        eventSets[key] = new Set();
      }
      eventSets[key].add(events[key]);
    }
  }
}

export function disposeMessages() {
  off(window, "message", eventHandler);
}

export function startHandleMessages() {
  on(window, "message", eventHandler);
}

const eventHandler = (e: Event) => {
  const ctx = e as MessageEvent<MessageWidget>;
  const data = ctx.data;
  const dataType = data?.type;

  if (!dataType?.startsWith(messagePrefix)) {
    return;
  }
  const targetEvent = dataType.substring(messagePrefix.length);
  const invokers = eventSets[targetEvent];
  if (!invokers) {
    return;
  }

  invokers.forEach((invoker) => {
    if (typeof invoker === "function") {
      invoker(data, e);
    }
  });
};
