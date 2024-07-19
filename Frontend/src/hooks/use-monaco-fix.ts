let clearInterval: any;

export function useMonacoHoverWidgetOverflowFix() {
  if (!clearInterval) {
    clearInterval = setInterval(() => {
      const els = document.querySelectorAll(".monaco-hover");

      for (const el of Array.from(els) as HTMLElement[]) {
        if (el?.style.top.startsWith("-")) {
          el.style.top = "0px";
        }
      }
    }, 500);
  }

  return () => clearInterval(clearInterval);
}
