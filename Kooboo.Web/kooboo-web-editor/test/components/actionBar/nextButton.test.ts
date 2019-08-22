import { createNextButton } from "@/components/actionBar/nextButton";
import context from "@/common/context";
import { operationManager } from "@/operation/Manager";
import { OperationEventArgs } from "@/events/OperationEvent";

describe("nextButton", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
    context.operationManager = new operationManager();
  });

  // 点击时，期望触发下一步动作
  it("createNextButton_click", () => {
    let isNext = false;
    context.operationManager.next = () => {
      isNext = true;
    };

    let element = createNextButton();
    let event = document.createEvent("MouseEvent");
    event.initEvent("click", true);
    element.dispatchEvent(event);
    expect(isNext).equal(true);
  });

  it("createNextButton_eventListene", () => {
    let element = createNextButton();

    context.operationEvent.emit(new OperationEventArgs(0, 1));
  });
});
