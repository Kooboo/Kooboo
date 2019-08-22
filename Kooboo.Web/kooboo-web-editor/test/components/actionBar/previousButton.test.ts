import context from "@/common/context";
import { operationManager } from "@/operation/Manager";
import { OperationEventArgs } from "@/events/OperationEvent";
import { createPreviousButton } from "@/components/actionBar/previousButton";

describe("previousButton", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
    context.operationManager = new operationManager();
  });

  // 点击时，期望触发上一步动作
  it("createPreviousButton_click", () => {
    let isPrevious = false;
    context.operationManager.previous = () => {
      isPrevious = true;
    };

    let element = createPreviousButton();
    let event = document.createEvent("MouseEvent");
    event.initEvent("click", true);
    element.dispatchEvent(event);

    expect(isPrevious).equal(true);
  });

  it("createPreviousButton_eventListene", () => {
    let element = createPreviousButton();

    context.operationEvent.emit(new OperationEventArgs(1, 0));
  });
});
