import "./assets/kb-style.css";
import { createActionBar } from "./components/actionBar";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder, createSelectedBorder } from "./components/selectorBorder";
import { createEditorShade } from "./components/editorShade";
import { listenHover, emitHoverEvent } from "./dom/events";
import { setElementClick } from "./common/utils";
import { addParentStyle } from "./kooboo/outsideInterfaces";
import { createContainer } from "./components/common/editorContainer";
import { createElementNav } from "./components/elementNav";

addParentStyle();
listenHover();
setElementClick();

const container = createContainer();
container.appendChild(createHoverBorder());
container.appendChild(createSelectedBorder());
container.appendChild(createFloatMenu());
container.appendChild(createEditorShade());
container.appendChild(createActionBar());
container.appendChild(createElementNav());

if (document.body) emitHoverEvent(document.body);
