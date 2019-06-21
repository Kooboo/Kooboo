import { createActionBar } from "./components/actionBar";
import { registerInlineEditor } from "./components/richEditor";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder } from "./components/selectorBorder";
import { createContainer } from "./components/container";
import { listenDomEvents } from "./dom/listeners";
import { createEditorShade } from "./components/editorShade";
import { createModal } from "./components/modal";

listenDomEvents();

const container = createContainer();
container.appendChild(createHoverBorder());
container.appendChild(createFloatMenu());
container.appendChild(createEditorShade());
// container.appendChild(
//   createModal({
//     title: "编辑",
//     body: document.createElement("div")
//   })
// );

createActionBar(document);
registerInlineEditor();
