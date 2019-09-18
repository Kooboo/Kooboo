import { createDiv } from "@/dom/element";

const border = "2px solid rgb(32, 122, 183)";

interface tabOptions {
  title: string;
  panel: HTMLElement;
  selected?: boolean;
}

export function createTabs(options: tabOptions[]) {
  let el = createDiv();
  let bar = createDiv();
  bar.style.marginBottom = "15px";
  bar.style.marginRight = "-15px";
  bar.style.marginLeft = "-15px";
  let body = createDiv();
  options.forEach(i => {
    let tab = createTab(i.title);
    tab.style.width = 100 / options.length + "%";
    tab.style.borderBottom = i.selected ? border : "2px solid #eee";
    i.panel.style.display = i.selected ? "block" : "none";
    tab.onclick = () => {
      switchTab(bar, tab);
      switchPanel(body, i.panel);
      options.forEach(o => (o.selected = o == i));
    };
    bar.appendChild(tab);
    body.appendChild(i.panel);
  });
  el.appendChild(bar);
  el.appendChild(body);
  return el;
}

function switchTab(bar: HTMLElement, tab: HTMLElement) {
  for (let j = 0; j < bar.children.length; j++) {
    let element = bar.children.item(j) as HTMLElement;
    element.style.borderBottom = element == tab ? border : "2px solid #eee";
  }
}

function switchPanel(body: HTMLElement, panel: HTMLElement) {
  for (let k = 0; k < body.children.length; k++) {
    let element = body.children.item(k) as HTMLElement;
    element.style.display = element == panel ? "block" : "none";
  }
}

function createTab(label: string) {
  let el = createDiv();
  el.style.height = "32px";
  el.style.textAlign = "center";
  el.innerText = label;
  el.style.display = "inline-block";
  el.style.fontSize = "15px";
  el.style.cursor = "pointer";
  return el;
}
