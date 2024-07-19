import type { MonacoEventType, MonacoEvent } from "monaco-recorder";
import { Monitor } from "monaco-recorder";
import type { editor } from "@/components/monaco-editor/userWorker";
import { useSiteStore } from "@/store/site";

export function createRecorder(
  id: string,
  monaco?: editor.IStandaloneCodeEditor,
  lang?: string
) {
  let index = 0;
  let events: MonacoEvent[] = [];
  let monitor: Monitor | null = null;
  const siteStore = useSiteStore();

  if (id === "") {
    // 新建文件的情况下，使用临时id
    id = Date.now().toString();
  }

  // on之前 如果没有传入monaco和lang 可以在调用这个方法传递
  function bindMonaco(_monaco: editor.IStandaloneCodeEditor, _lang: string) {
    monaco = _monaco;
    lang = _lang;

    on();
  }

  // 开始记录
  function on() {
    if (!siteStore.site.recordSiteLogVideo) return;
    if (!monaco || !lang) return;

    index = 0;
    events = [];
    monitor = new Monitor(id, monaco, lang);

    // 绑定监听
    monitor.bindStoreCallback((event) => {
      event.order = index++; // 替换为当前tab的单独排序
      events.push(event);
      // console.log(event);
    });

    // 记录初始化数据
    monitor.open();

    // 开始监听
    monitor.on();
  }

  // 结束记录
  function off() {
    monitor?.dispose();
    monitor = null;
    return events;
  }

  // 保存, 不会销毁监听器，将已有的事件先上传，然后重置
  function save() {
    const result = events;
    index = 0;
    events = [];

    // 保存后，重新写入打开事件
    monitor?.open();
    // 返回要保存的事件 与 代码一起存储
    if (result.length) {
      const initialValue = result.find(
        (i) => i.type === ("Open" as MonacoEventType.Open)
      )?.text;

      // 没有Open事件
      if (typeof initialValue !== "string") {
        return "";
      }

      // 未修改
      if (initialValue === monaco?.getValue()) {
        return "";
      }

      return JSON.stringify(result);
    } else {
      return "";
    }
  }

  return {
    bindMonaco,
    on,
    off,
    save,
  };
}
