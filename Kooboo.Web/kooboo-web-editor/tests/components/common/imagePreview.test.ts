import { createImagePreview } from "@/components/common/imagePreview";

describe("imagePreview", () => {
  test("setImage", () => {
    let preview = createImagePreview();
    let div = preview.imagePreview.children.item(0) as HTMLDivElement;
    expect(div.style.backgroundImage).toBeFalsy();
    preview.setImage("1.jpg");
    expect(div.style.backgroundImage).toEqual("url(1.jpg)");
  });

  test("delete", () => {
    let preview = createImagePreview(true, () => preview.setImage("none"));
    let deleteButton = preview.imagePreview.children.item(1) as HTMLButtonElement;
    expect(deleteButton.style.visibility).toEqual("visible");
    deleteButton.click();
    expect(deleteButton.style.backgroundImage).toEqual("");
  });
});
