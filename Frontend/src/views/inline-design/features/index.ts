function getFeatures() {
  const files = import.meta.globEager("@/views/inline-design/features/*.ts");
  const result = [];

  for (const key in files) {
    result.push(files[key]);
  }

  return result.sort((a, b) => a.order - b.order);
}

export const features: any[] = getFeatures();
