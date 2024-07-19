export function getPaths(e: any): HTMLElement[] {
  return e.path || [];
}

export function getTopPath(e: Event) {
  return getPaths(e)[0];
}
