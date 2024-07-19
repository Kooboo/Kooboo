export function setOverlay() {
  const aux: HTMLElement | null =
    document.body.querySelector(".tox-tinymce-aux");
  if (aux) {
    const zIndex = Array.from<HTMLElement>(
      document.querySelectorAll(".el-overlay")
    ).at(-1)?.style.zIndex;
    aux.style.setProperty("z-index", zIndex);
  }
}

export function restoreOverlay() {
  const aux: HTMLElement | null =
    document.body.querySelector(".tox-tinymce-aux");
  if (aux) {
    aux.style.removeProperty("z-index");
  }
}
