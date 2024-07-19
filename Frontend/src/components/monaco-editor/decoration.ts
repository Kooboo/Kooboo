import { monaco } from "./userWorker";

export function createDecorations(
  breakpoints: number[],
  executing?: number,
  hoverLine?: number
) {
  const list = [];

  for (const breakpoint of breakpoints) {
    list.push({
      range: new monaco.Range(breakpoint, 1, breakpoint, 1),
      options: {
        glyphMarginClassName: "kooboo-debug-breakpoint",
      },
    });
  }

  if (executing !== undefined) {
    list.push({
      range: new monaco.Range(executing, 1, executing, 1),
      options: {
        isWholeLine: true,
        className: "kooboo-debug-executing",
      },
    });
  }

  if (hoverLine !== undefined) {
    list.push({
      range: new monaco.Range(hoverLine, 1, hoverLine, 1),
      options: {
        glyphMarginClassName: "kooboo-debug-hover",
      },
    });
  }

  return list;
}
