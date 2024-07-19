import { promises as fs } from "fs";
import fg from "fast-glob";
import MagicString from "magic-string";

// console.log(fg.sync("src/**/*.vue"));
const vuePaths = fg.sync("src/**/*.vue");

const __IMPORT__ = '\nimport { useI18n } from "vue-i18n";';
const __DEFINE_T__ = "\nconst { t } = useI18n();";

const addIfPos = (a: number, b: number) => (a < 0 ? a : a + b);
const infIfNeg = (x: number) => (x < 0 ? Infinity : x);

(async () => {
  for (const path of vuePaths) {
    const content = await fs.readFile(path, "utf8");
    if (!content.includes("$t")) continue;

    const str = new MagicString(content);

    const importIdx = Math.max(
      addIfPos(
        content.indexOf(";\n", infIfNeg(content.lastIndexOf("import {"))),
        2
      ),
      addIfPos(
        content.indexOf(";\n", infIfNeg(content.lastIndexOf("import "))),
        2
      ),
      content.indexOf("\n", content.lastIndexOf("<script "))
    );
    const lineAfterPropsDefIdx = addIfPos(
      content.indexOf("();\n", infIfNeg(content.lastIndexOf("defineProps"))),
      3
    );
    const lineAfterEmitsDefIdx = addIfPos(
      content.indexOf("();\n", infIfNeg(content.lastIndexOf("defineEmits"))),
      3
    );

    str.appendRight(importIdx, __IMPORT__);
    str.appendRight(
      Math.max(importIdx, lineAfterPropsDefIdx, lineAfterEmitsDefIdx),
      __DEFINE_T__
    );

    str.replace(/\$t/g, "t");

    console.log("writingFile:", path);
    fs.writeFile(path, str.toString());
  }
})();
