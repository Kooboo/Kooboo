import { BaseEvent } from "@/events/BaseEvent";

describe("BaseEevent", () => {
  it("add event listener", () => {
    let event = new BaseEvent<void>();

    event.addEventListener(() => null);
    expect(event.handlers.length).to.eq(1);

    event.addEventListener(() => null);
    event.addEventListener(() => null);
    expect(event.handlers.length).to.eq(3);
  });

  it("emit event", () => {
    let event = new BaseEvent<void>();
    let value: string = "";
    event.addEventListener(() => (value = "hello"));

    event.emit();

    expect(value).to.eq("hello");
  });
});
