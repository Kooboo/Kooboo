import { BaseEvent } from "../src/events/BaseEvent";

test("init event", () => {
  let event = new BaseEvent<void>("baseEvent");

  expect(event.name).toBe("baseEvent");
});

test("add event listener", () => {
  let event = new BaseEvent<void>("baseEvent");

  event.addEventListener(() => null);
  expect(event.handlers.length).toBe(1);

  event.addEventListener(() => null);
  event.addEventListener(() => null);
  expect(event.handlers.length).toBe(3);
});

test("emit event", () => {
  let event = new BaseEvent<void>("baseEvent");
  let value: string = "";
  event.addEventListener(() => (value = "hello"));

  event.emit();

  expect(value).toBe("hello");
});
