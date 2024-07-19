import type { KeyValue } from "./types";

export const colorNameMap = {
  aliceblue: { hex: "F0F8FF", dec: [240, 248, 255] },
  antiquewhite: { hex: "FAEBD7", dec: [250, 235, 215] },
  aqua: { hex: "00FFFF", dec: [0, 255, 255] },
  aquamarine: { hex: "7FFFD4", dec: [127, 255, 212] },
  azure: { hex: "F0FFFF", dec: [240, 255, 255] },
  beige: { hex: "F5F5DC", dec: [245, 245, 220] },
  bisque: { hex: "FFE4C4", dec: [255, 228, 196] },
  black: { hex: "000000", dec: [0, 0, 0] },
  blanchedalmond: { hex: "FFEBCD", dec: [255, 235, 205] },
  blue: { hex: "0000FF", dec: [0, 0, 255] },
  blueviolet: { hex: "8A2BE2", dec: [138, 43, 226] },
  brown: { hex: "A52A2A", dec: [165, 42, 42] },
  burlywood: { hex: "DEB887", dec: [222, 184, 135] },
  cadetblue: { hex: "5F9EA0", dec: [95, 158, 160] },
  chartreuse: { hex: "7FFF00", dec: [127, 255, 0] },
  chocolate: { hex: "D2691E", dec: [210, 105, 30] },
  coral: { hex: "FF7F50", dec: [255, 127, 80] },
  cornflowerblue: { hex: "6495ED", dec: [100, 149, 237] },
  cornsilk: { hex: "FFF8DC", dec: [255, 248, 220] },
  crimson: { hex: "DC143C", dec: [220, 20, 60] },
  cyan: { hex: "00FFFF", dec: [0, 255, 255] },
  darkblue: { hex: "00008B", dec: [0, 0, 139] },
  darkcyan: { hex: "008B8B", dec: [0, 139, 139] },
  darkgoldenrod: { hex: "B8860B", dec: [184, 134, 11] },
  darkgray: { hex: "A9A9A9", dec: [169, 169, 169] },
  darkgreen: { hex: "006400", dec: [0, 100, 0] },
  darkgrey: { hex: "A9A9A9", dec: [169, 169, 169] },
  darkkhaki: { hex: "BDB76B", dec: [189, 183, 107] },
  darkmagenta: { hex: "8B008B", dec: [139, 0, 139] },
  darkolivegreen: { hex: "556B2F", dec: [85, 107, 47] },
  darkorange: { hex: "FF8C00", dec: [255, 140, 0] },
  darkorchid: { hex: "9932CC", dec: [153, 50, 204] },
  darkred: { hex: "8B0000", dec: [139, 0, 0] },
  darksalmon: { hex: "E9967A", dec: [233, 150, 122] },
  darkseagreen: { hex: "8FBC8F", dec: [143, 188, 143] },
  darkslateblue: { hex: "483D8B", dec: [72, 61, 139] },
  darkslategray: { hex: "2F4F4F", dec: [47, 79, 79] },
  darkslategrey: { hex: "2F4F4F", dec: [47, 79, 79] },
  darkturquoise: { hex: "00CED1", dec: [0, 206, 209] },
  darkviolet: { hex: "9400D3", dec: [148, 0, 211] },
  deeppink: { hex: "FF1493", dec: [255, 20, 147] },
  deepskyblue: { hex: "00BFFF", dec: [0, 191, 255] },
  dimgray: { hex: "696969", dec: [105, 105, 105] },
  dimgrey: { hex: "696969", dec: [105, 105, 105] },
  dodgerblue: { hex: "1E90FF", dec: [30, 144, 255] },
  firebrick: { hex: "B22222", dec: [178, 34, 34] },
  floralwhite: { hex: "FFFAF0", dec: [255, 250, 240] },
  forestgreen: { hex: "228B22", dec: [34, 139, 34] },
  fuchsia: { hex: "FF00FF", dec: [255, 0, 255] },
  gainsboro: { hex: "DCDCDC", dec: [220, 220, 220] },
  ghostwhite: { hex: "F8F8FF", dec: [248, 248, 255] },
  gold: { hex: "FFD700", dec: [255, 215, 0] },
  goldenrod: { hex: "DAA520", dec: [218, 165, 32] },
  gray: { hex: "808080", dec: [128, 128, 128] },
  green: { hex: "008000", dec: [0, 128, 0] },
  greenyellow: { hex: "ADFF2F", dec: [173, 255, 47] },
  grey: { hex: "808080", dec: [128, 128, 128] },
  honeydew: { hex: "F0FFF0", dec: [240, 255, 240] },
  hotpink: { hex: "FF69B4", dec: [255, 105, 180] },
  indianred: { hex: "CD5C5C", dec: [205, 92, 92] },
  indigo: { hex: "4B0082", dec: [75, 0, 130] },
  ivory: { hex: "FFFFF0", dec: [255, 255, 240] },
  khaki: { hex: "F0E68C", dec: [240, 230, 140] },
  lavender: { hex: "E6E6FA", dec: [230, 230, 250] },
  lavenderblush: { hex: "FFF0F5", dec: [255, 240, 245] },
  lawngreen: { hex: "7CFC00", dec: [124, 252, 0] },
  lemonchiffon: { hex: "FFFACD", dec: [255, 250, 205] },
  lightblue: { hex: "ADD8E6", dec: [173, 216, 230] },
  lightcoral: { hex: "F08080", dec: [240, 128, 128] },
  lightcyan: { hex: "E0FFFF", dec: [224, 255, 255] },
  lightgoldenrodyellow: { hex: "FAFAD2", dec: [250, 250, 210] },
  lightgray: { hex: "D3D3D3", dec: [211, 211, 211] },
  lightgreen: { hex: "90EE90", dec: [144, 238, 144] },
  lightgrey: { hex: "D3D3D3", dec: [211, 211, 211] },
  lightpink: { hex: "FFB6C1", dec: [255, 182, 193] },
  lightsalmon: { hex: "FFA07A", dec: [255, 160, 122] },
  lightseagreen: { hex: "20B2AA", dec: [32, 178, 170] },
  lightskyblue: { hex: "87CEFA", dec: [135, 206, 250] },
  lightslategray: { hex: "778899", dec: [119, 136, 153] },
  lightslategrey: { hex: "778899", dec: [119, 136, 153] },
  lightsteelblue: { hex: "B0C4DE", dec: [176, 196, 222] },
  lightyellow: { hex: "FFFFE0", dec: [255, 255, 224] },
  lime: { hex: "00FF00", dec: [0, 255, 0] },
  limegreen: { hex: "32CD32", dec: [50, 205, 50] },
  linen: { hex: "FAF0E6", dec: [250, 240, 230] },
  magenta: { hex: "FF00FF", dec: [255, 0, 255] },
  maroon: { hex: "800000", dec: [128, 0, 0] },
  mediumaquamarine: { hex: "66CDAA", dec: [102, 205, 170] },
  mediumblue: { hex: "0000CD", dec: [0, 0, 205] },
  mediumorchid: { hex: "BA55D3", dec: [186, 85, 211] },
  mediumpurple: { hex: "9370DB", dec: [147, 112, 219] },
  mediumseagreen: { hex: "3CB371", dec: [60, 179, 113] },
  mediumslateblue: { hex: "7B68EE", dec: [123, 104, 238] },
  mediumspringgreen: { hex: "00FA9A", dec: [0, 250, 154] },
  mediumturquoise: { hex: "48D1CC", dec: [72, 209, 204] },
  mediumvioletred: { hex: "C71585", dec: [199, 21, 133] },
  midnightblue: { hex: "191970", dec: [25, 25, 112] },
  mintcream: { hex: "F5FFFA", dec: [245, 255, 250] },
  mistyrose: { hex: "FFE4E1", dec: [255, 228, 225] },
  moccasin: { hex: "FFE4B5", dec: [255, 228, 181] },
  navajowhite: { hex: "FFDEAD", dec: [255, 222, 173] },
  navy: { hex: "000080", dec: [0, 0, 128] },
  oldlace: { hex: "FDF5E6", dec: [253, 245, 230] },
  olive: { hex: "808000", dec: [128, 128, 0] },
  olivedrab: { hex: "6B8E23", dec: [107, 142, 35] },
  orange: { hex: "FFA500", dec: [255, 165, 0] },
  orangered: { hex: "FF4500", dec: [255, 69, 0] },
  orchid: { hex: "DA70D6", dec: [218, 112, 214] },
  palegoldenrod: { hex: "EEE8AA", dec: [238, 232, 170] },
  palegreen: { hex: "98FB98", dec: [152, 251, 152] },
  paleturquoise: { hex: "AFEEEE", dec: [175, 238, 238] },
  palevioletred: { hex: "DB7093", dec: [219, 112, 147] },
  papayawhip: { hex: "FFEFD5", dec: [255, 239, 213] },
  peachpuff: { hex: "FFDAB9", dec: [255, 218, 185] },
  peru: { hex: "CD853F", dec: [205, 133, 63] },
  pink: { hex: "FFC0CB", dec: [255, 192, 203] },
  plum: { hex: "DDA0DD", dec: [221, 160, 221] },
  powderblue: { hex: "B0E0E6", dec: [176, 224, 230] },
  purple: { hex: "800080", dec: [128, 0, 128] },
  rebeccapurple: { hex: "663399", dec: [102, 51, 153] },
  red: { hex: "FF0000", dec: [255, 0, 0] },
  rosybrown: { hex: "BC8F8F", dec: [188, 143, 143] },
  royalblue: { hex: "4169E1", dec: [65, 105, 225] },
  saddlebrown: { hex: "8B4513", dec: [139, 69, 19] },
  salmon: { hex: "FA8072", dec: [250, 128, 114] },
  sandybrown: { hex: "F4A460", dec: [244, 164, 96] },
  seagreen: { hex: "2E8B57", dec: [46, 139, 87] },
  seashell: { hex: "FFF5EE", dec: [255, 245, 238] },
  sienna: { hex: "A0522D", dec: [160, 82, 45] },
  silver: { hex: "C0C0C0", dec: [192, 192, 192] },
  skyblue: { hex: "87CEEB", dec: [135, 206, 235] },
  slateblue: { hex: "6A5ACD", dec: [106, 90, 205] },
  slategray: { hex: "708090", dec: [112, 128, 144] },
  slategrey: { hex: "708090", dec: [112, 128, 144] },
  snow: { hex: "FFFAFA", dec: [255, 250, 250] },
  springgreen: { hex: "00FF7F", dec: [0, 255, 127] },
  steelblue: { hex: "4682B4", dec: [70, 130, 180] },
  tan: { hex: "D2B48C", dec: [210, 180, 140] },
  teal: { hex: "008080", dec: [0, 128, 128] },
  thistle: { hex: "D8BFD8", dec: [216, 191, 216] },
  tomato: { hex: "FF6347", dec: [255, 99, 71] },
  turquoise: { hex: "40E0D0", dec: [64, 224, 208] },
  violet: { hex: "EE82EE", dec: [238, 130, 238] },
  wheat: { hex: "F5DEB3", dec: [245, 222, 179] },
  white: { hex: "FFFFFF", dec: [255, 255, 255] },
  whitesmoke: { hex: "F5F5F5", dec: [245, 245, 245] },
  yellow: { hex: "FFFF00", dec: [255, 255, 0] },
  yellowgreen: { hex: "9ACD32", dec: [154, 205, 50] },
};

export const colorExtractors = [
  {
    name: "color",
    extract: (value: string) => exactExtract("color", value),
  },
  {
    name: "background",
    extract: (value: string) => includeExtract("background-color", value),
  },
  {
    name: "background-color",
    extract: (value: string) => exactExtract("background-color", value),
  },
  {
    name: "border",
    extract: (value: string) => includeExtract("border-color", value),
  },
  {
    name: "border-color",
    extract(value: string) {
      const result: KeyValue[] = [];
      if (value) {
        const colors = extractColorFromString(value);
        if (colors.length == 1) {
          result.push({ key: "border-color", value: colors[0] });
        } else if (colors.length == 2) {
          result.push({ key: "border-top-color", value: colors[0] });
          result.push({ key: "border-bottom-color", value: colors[0] });
          result.push({ key: "border-left-color", value: colors[1] });
          result.push({ key: "border-right-color", value: colors[1] });
        } else if (colors.length == 3) {
          result.push({ key: "border-top-color", value: colors[0] });
          result.push({ key: "border-bottom-color", value: colors[2] });
          result.push({ key: "border-left-color", value: colors[1] });
          result.push({ key: "border-right-color", value: colors[1] });
        } else if (colors.length == 4) {
          result.push({ key: "border-top-color", value: colors[0] });
          result.push({ key: "border-right-color", value: colors[1] });
          result.push({ key: "border-bottom-color", value: colors[2] });
          result.push({ key: "border-left-color", value: colors[3] });
        }
      }
      return result;
    },
  },
  {
    name: "border-top",
    extract: (value: string) => includeExtract("border-top-color", value),
  },
  {
    name: "border-right",
    extract: (value: string) => includeExtract("border-right-color", value),
  },
  {
    name: "border-bottom",
    extract: (value: string) => includeExtract("border-bottom-color", value),
  },
  {
    name: "border-left",
    extract: (value: string) => includeExtract("border-left-color", value),
  },
  {
    name: "border-top-color",
    extract: (value: string) => exactExtract("border-top-color", value),
  },
  {
    name: "border-right-color",
    extract: (value: string) => exactExtract("border-right-color", value),
  },
  {
    name: "border-bottom-color",
    extract: (value: string) => exactExtract("border-bottom-color", value),
  },
  {
    name: "border-left-color",
    extract: (value: string) => exactExtract("border-left-color", value),
  },
  {
    name: "outline",
    extract: (value: string) => includeExtract("outline-color", value),
  },
  {
    name: "outline-color",
    extract: (value: string) => exactExtract("outline-color", value),
  },
];

function exactExtract(name: string, value: string) {
  const result: KeyValue[] = [];
  if (value) {
    result.push({ key: name, value: value });
  }
  return result;
}

function includeExtract(name: string, value: string) {
  const result: KeyValue[] = [];
  if (value) {
    const colors = extractColorFromString(value);
    if (colors.length) {
      result.push({ key: name, value: colors[0] });
    }
  }
  return result;
}

const regExp = RegExp(
  `\\s${Object.keys(colorNameMap)
    .map((m) => "(" + m + ")")
    .join(
      "|"
    )}|(rgb(a)?\\(\\s*\\d+\\s*,\\s*\\d+\\s*,\\s*\\d+\\s*(,\\s*\\d*\\.?\\d*\\s*)?)\\)|(#[\\d\\w]{3,8})\\s`,
  "gim"
);

function extractColorFromString(value: string) {
  value = ` ${value} `;
  const result: string[] = [];
  const arr = value.matchAll(regExp);

  for (const i of arr) {
    result.push(i[0].trim());
  }

  return result;
}

export function colorNameToValue(name: string) {
  if (name in colorNameMap) {
    const dec = (colorNameMap as any)[name].dec;
    return `rgb(${dec[0]},${dec[1]},${dec[2]})`;
  }

  return name;
}
