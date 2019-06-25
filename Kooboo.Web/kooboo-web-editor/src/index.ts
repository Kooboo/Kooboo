import { createActionBar } from "./components/actionBar";
import { registerInlineEditor } from "./components/richEditor";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder } from "./components/selectorBorder";
import { createContainer } from "./components/container";
import { listenDomEvents } from "./dom/listeners";
import { createEditorShade } from "./components/editorShade";

listenDomEvents();

const container = createContainer();
container.appendChild(createHoverBorder());
container.appendChild(createFloatMenu());
container.appendChild(createEditorShade());
createActionBar(document);
registerInlineEditor();
