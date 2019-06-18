export default function(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}
