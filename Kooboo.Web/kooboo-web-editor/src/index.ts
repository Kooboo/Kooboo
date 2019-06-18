import registerHoverBorder from "./components/selectorBorder";
import { registerEditorShade } from "./components/editorShade";
import { createActionBar } from "./components/floatActionBar";
import { registerInlineEditor } from "./components/richEditor";
import { registerMenu } from "./components/floatMenu";
import { registerDomSelector } from "./components/domSelector";

createActionBar(document);
registerDomSelector(document);
registerHoverBorder(document);
registerEditorShade(document);
registerInlineEditor();
registerMenu(document);
