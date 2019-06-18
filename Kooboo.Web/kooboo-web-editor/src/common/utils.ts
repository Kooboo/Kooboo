export function delay(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}
