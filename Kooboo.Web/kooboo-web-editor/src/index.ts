import "./assets/kb-style.css";
import "@simonwep/pickr/dist/themes/nano.min.css";
import { createActionBar } from "./components/actionBar";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder, createSelectedBorder } from "./components/selectorBorder";
import { createEditorShade } from "./components/editorShade";
import { emitHoverEvent } from "./dom/events";
import { initElements, browserCheck } from "./common/utils";
import { shareStyle } from "./kooboo/outsideInterfaces";
import { createElementNav } from "./components/elementNav";
import context from "./common/context";

browserCheck();
shareStyle();
initElements();

context.container.appendChild(createHoverBorder());
context.container.appendChild(createSelectedBorder());
context.container.appendChild(createFloatMenu());
context.container.appendChild(createEditorShade());
context.container.appendChild(createActionBar());
context.container.appendChild(createElementNav());

if (document.body) emitHoverEvent(document.body);
