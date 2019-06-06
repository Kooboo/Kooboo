import hoverShowBorder from "./components/selectorBorder";
import listenSelectDomChange from "./dom/listenSelectDomChange";
import { registerEditorShade } from "./components/editorShade";
import { createActionBar } from "./components/floatActionBar";
import { registerInlineEditor } from "./components/richEditor";

createActionBar(document);
listenSelectDomChange(document);
hoverShowBorder(document);
registerEditorShade(document);
registerInlineEditor(document);
