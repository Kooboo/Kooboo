import { createImagePreview } from "@/components/common/imagePreview";

describe("imagePreview", () => {
  it("setImage", () => {
    let preview = createImagePreview();
    let div = preview.imagePreview.children.item(0) as HTMLDivElement;
    expect(div.style.backgroundImage).not.ok;
    preview.setImage("1.jpg");
    expect(div.style.backgroundImage).equal(`url("1.jpg")`);
  });

  it("delete", () => {
    let preview = createImagePreview(true, () => preview.setImage("none"));
    let deleteButton = preview.imagePreview.children.item(1) as HTMLButtonElement;
    expect(deleteButton.style.visibility).equal("visible");
    deleteButton.click();
    expect(deleteButton.style.backgroundImage).equal("");
  });
});
