const { parse, stringify } = require("yaml");
const { sortBy } = require("lodash");

const { readFileSync, readdirSync, writeFileSync } = require("fs");
const { resolve } = require("path");

const dir = resolve(__dirname, "../locales");

function sortObject(obj) {
  if (typeof obj !== "object") {
    return obj;
  }
  const temp = {};
  for (const key of sortBy(Object.keys(obj), (k) => k.toLowerCase())) {
    temp[key] = sortObject(obj[key]);
  }
  return temp;
}

const files = readdirSync(dir);
for (const file of files) {
  const target = resolve(dir, file);
  const yml = readFileSync(target).toString();
  const json = parse(yml);
  const sorted = sortObject(json);
  const sortedYml = stringify(sorted);
  writeFileSync(target, sortedYml, { encoding: "utf8" });
}
