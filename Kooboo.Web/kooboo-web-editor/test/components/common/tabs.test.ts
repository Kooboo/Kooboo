import { createTabs } from "@/components/common/tabs";

describe("tabs", () => {
  it("tab click", () => {
    let tabs = createTabs([
      {
        title: "a",
        selected: true,
        panel: document.createElement("div")
      },
      {
        title: "b",
        panel: document.createElement("div")
      }
    ]);

    let bar = tabs.children.item(0) as HTMLElement;
    let panel = tabs.children.item(1) as HTMLElement;
    expect((panel.children.item(0) as HTMLElement).style.display).equal("block");
    expect((panel.children.item(1) as HTMLElement).style.display).equal("none");
    (bar.children.item(1) as HTMLElement).click();
    expect((panel.children.item(0) as HTMLElement).style.display).equal("none");
    expect((panel.children.item(1) as HTMLElement).style.display).equal("block");
  });
});
