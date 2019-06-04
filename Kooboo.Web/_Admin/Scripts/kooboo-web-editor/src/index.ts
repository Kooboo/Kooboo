import hoverShowBorder from "./effects/hoverShowBorder";
import clickEdit from "./effects/clickEdit";
import listenSelectDomChange from "./dom/listenSelectDomChange";
import editingShowShade from "./effects/editingShowShade";
import { createActionBar } from "./components/floatActionBar";

createActionBar(document);
listenSelectDomChange(document);
hoverShowBorder(document);
editingShowShade(document);
clickEdit(document);
