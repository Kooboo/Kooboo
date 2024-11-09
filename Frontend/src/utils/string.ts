/**
 * check if two equals, ignore case
 * @param input1 string1
 * @param input2 string2
 * @returns boolean
 */
export function ignoreCaseEqual(input1: string, input2: string) {
  return (
    input1?.toString()?.toLowerCase() === input2?.toString()?.toLowerCase()
  );
}

export function ignoreCaseContains(inputs: string[], match: string) {
  return inputs?.some((it) => ignoreCaseEqual(it, match));
}

export function getValueIgnoreCase(input: any, name: string) {
  if (!input || !name) {
    return null;
  }
  for (const key in input) {
    if (
      Object.prototype.hasOwnProperty.call(input, key) &&
      ignoreCaseEqual(key, name)
    ) {
      return input[key];
    }
  }

  return null;
}

export function camelCase(str: string) {
  if (str?.length) {
    return str.charAt(0).toLowerCase() + str.substring(1);
  }
  return str;
}
