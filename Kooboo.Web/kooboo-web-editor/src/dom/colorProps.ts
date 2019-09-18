import { BoxShadow } from "./BoxShadow";
import { TextShadow } from "./TextShadow";

export interface ColorProp {
  prop: string;
  isChild: boolean;
  parent: string;
  replaceColor: (oldValue: string, newColor: string) => string;
  getColor: (value: string) => string;
}

export const colorProps: ColorProp[] = [
  {
    prop: "color",
    isChild: false,
    parent: "color",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "background-color",
    isChild: true,
    parent: "background",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "box-shadow",
    isChild: false,
    parent: "box-shadow",
    replaceColor: (o, n) => {
      let obj = new BoxShadow(o);
      obj.color = n;
      return obj.toString();
    },
    getColor: o => new BoxShadow(o).color
  },
  {
    prop: "text-shadow",
    isChild: false,
    parent: "text-shadow",
    replaceColor: (o, n) => {
      let obj = new TextShadow(o);
      obj.color = n;
      return obj.toString();
    },
    getColor: o => new TextShadow(o).color
  },
  {
    prop: "border-color",
    isChild: true,
    parent: "border",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "border-left-color",
    isChild: true,
    parent: "border-left",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "border-top-color",
    isChild: true,
    parent: "border-top",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "border-right-color",
    isChild: true,
    parent: "border-right",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "border-bottom-color",
    isChild: true,
    parent: "border-bottom",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "outline-color",
    isChild: false,
    parent: "outline",
    replaceColor: (o, n) => n,
    getColor: o => o
  },
  {
    prop: "column-rule-color",
    isChild: false,
    parent: "column-rule",
    replaceColor: (o, n) => n,
    getColor: o => o
  }
];
