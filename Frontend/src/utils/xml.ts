export interface Model {
  name: string;
  attributes?: Record<string, string>;
  children?: Model[] | string;
}

const startDivIndex = "<div".length;
const endDivIndex = "></div>".length;

export function modelToXml(model: Model) {
  const el = document.createElement("div");
  for (const key in model.attributes) {
    const value = model.attributes[key] ?? "";
    el.setAttribute(key, value);
  }
  const html = el.outerHTML;
  const attrs = html.substring(startDivIndex, html.length - endDivIndex);
  let result = `<${model.name}${attrs}>`;

  if (!model.children) {
    result += `</${model.name}>`;
  } else {
    if (typeof model.children === "string") {
      result += model.children;
    } else {
      for (const i of model.children) {
        result += `${modelToXml(i)}`;
      }
    }
    result += `</${model.name}>`;
  }

  return result;
}
