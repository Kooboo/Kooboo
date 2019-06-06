import registerHoverBorder from "./components/selectorBorder";
import listenSelectDomChange from "./dom/listenSelectDomChange";
import { registerEditorShade } from "./components/editorShade";
import { createActionBar } from "./components/floatActionBar";
import { registerInlineEditor } from "./components/richEditor";
import { registerMenu } from "./components/floatMenu";

createActionBar(document);
listenSelectDomChange(document);
registerHoverBorder(document);
registerEditorShade(document);
// registerInlineEditor(document);
registerMenu(document);
