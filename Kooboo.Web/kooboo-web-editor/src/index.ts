import "./assets/kb-style.css";
import { createActionBar } from "./components/actionBar";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder } from "./components/selectorBorder";
import { createContainer } from "./components/container";
import { createEditorShade } from "./components/editorShade";
import { listenHover, listenClick } from "./dom/events";
import { stopLinkElementClick } from "./common/utils";
import { addParentStyle } from "./kooboo/outsideInterfaces";

addParentStyle();
listenHover();
listenClick();
stopLinkElementClick();

const container = createContainer();
container.appendChild(createHoverBorder());
container.appendChild(createFloatMenu());
container.appendChild(createEditorShade());
createActionBar(document);
