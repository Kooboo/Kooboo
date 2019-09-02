import "./assets/kb-style.css";
import "@simonwep/pickr/dist/themes/nano.min.css";
import { createActionBar } from "./components/actionBar";
import { createFloatMenu } from "./components/floatMenu";
import { createHoverBorder, createSelectedBorder } from "./components/selectorBorder";
import { createEditorShade } from "./components/editorShade";
import { emitHoverEvent } from "./dom/events";
import { initElements } from "./common/utils";
import { parentBody } from "./kooboo/outsideInterfaces";
import { createElementNav } from "./components/elementNav";
import context from "./common/context";
import { shareStyle } from "./dom/utils";

shareStyle(parentBody);
shareStyle(context.container);
initElements();

context.container.appendChild(createHoverBorder());
context.container.appendChild(createSelectedBorder());
context.container.appendChild(createFloatMenu());
context.container.appendChild(createEditorShade());
context.container.appendChild(createActionBar());
context.container.appendChild(createElementNav());

if (document.body) emitHoverEvent(document.body);
