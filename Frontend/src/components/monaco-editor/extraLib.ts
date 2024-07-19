import { monaco } from "./userWorker";

export function addExtraLib(content: string, path: string) {
  monaco.languages.typescript.javascriptDefaults.addExtraLib(content, path);
  monaco.languages.typescript.typescriptDefaults.addExtraLib(content, path);
}
