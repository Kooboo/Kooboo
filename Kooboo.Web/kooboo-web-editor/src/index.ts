import "./assets/kb-style.css";
import { createActionBar } from "./components/actionBar";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder, createSelectedBorder } from "./components/selectorBorder";
import { createEditorShade } from "./components/editorShade";
import { listenHover, listenClick } from "./dom/events";
import { stopLinkElementClick } from "./common/utils";
import { addParentStyle } from "./kooboo/outsideInterfaces";
import { createContainer } from "./components/common";
import { createElementNav } from "./components/elementNav";

addParentStyle();
listenHover();
listenClick();
stopLinkElementClick();

const container = createContainer();
container.appendChild(createHoverBorder());
container.appendChild(createSelectedBorder());
container.appendChild(createFloatMenu());
container.appendChild(createEditorShade());
container.appendChild(createActionBar());
container.appendChild(createElementNav());
