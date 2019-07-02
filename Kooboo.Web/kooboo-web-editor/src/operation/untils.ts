import { Log } from "./recordLogs/Log";

export function getMargedLogs(logs: Log[]) {
  let result: Log[] = [];
  logs.forEach(o => {
    var exist = result.filter(
      f =>
        typeof f == typeof o &&
        f.comment.nameorid == o.comment.nameorid &&
        f.comment.objecttype == o.comment.objecttype &&
        f.koobooId &&
        o.koobooId &&
        f.koobooId.startsWith(o.koobooId)
    );
    if (exist.length > 0) {
      result = result.filter(f => exist.some(s => s != f));
    }
    result.push(o);
  });

  return result;
}
