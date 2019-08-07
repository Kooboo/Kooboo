import { Background } from "./Background";
import { BoxShadow } from "./BoxShadow";
import { Border } from "./Border";
import { Outline } from "./Outline";

export interface ColorProp {
  prop: string;
  isChild: boolean;
  parent: string;
  replaceColor: (oldValue: string, newColor: string) => string;
}

export const colorProps: ColorProp[] = [
  {
    prop: "color",
    isChild: false,
    parent: "color",
    replaceColor: (o, n) => n
  },
  {
    prop: "background-color",
    isChild: true,
    parent: "background",
    replaceColor: (o, n) => n
  },
  {
    prop: "background",
    isChild: false,
    parent: "background",
    replaceColor: (o, n) => {
      let obj = new Background(o);
      obj.color = n;
      return obj.toString();
    }
  },
  {
    prop: "box-shadow",
    isChild: false,
    parent: "box-shadow",
    replaceColor: (o, n) => {
      let obj = new BoxShadow(o);
      obj.color = n;
      return obj.toString();
    }
  },
  {
    prop: "border",
    isChild: false,
    parent: "border",
    replaceColor: (o, n) => {
      let obj = new Border(o);
      obj.color = n;
      return obj.toString();
    }
  },
  {
    prop: "boder-color",
    isChild: true,
    parent: "border",
    replaceColor: (o, n) => n
  },
  {
    prop: "outline",
    isChild: false,
    parent: "outline",
    replaceColor: (o, n) => {
      let obj = new Outline(o);
      obj.color = n;
      return obj.toString();
    }
  },
  {
    prop: "outline-color",
    isChild: false,
    parent: "outline",
    replaceColor: (o, n) => n
  }
];
