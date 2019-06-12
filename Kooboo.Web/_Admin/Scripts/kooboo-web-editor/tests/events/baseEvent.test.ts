import { BaseEvent } from "../../src/events/BaseEvent";

test("add event listener", () => {
  let event = new BaseEvent<void>();

  event.addEventListener(() => null);
  expect(event.handlers.length).toBe(1);

  event.addEventListener(() => null);
  event.addEventListener(() => null);
  expect(event.handlers.length).toBe(3);
});

test("emit event", () => {
  let event = new BaseEvent<void>();
  let value: string = "";
  event.addEventListener(() => (value = "hello"));

  event.emit();

  expect(value).toBe("hello");
});
