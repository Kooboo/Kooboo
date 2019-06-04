import hoverShowBorder from "./effects/hoverShowBorder";
import clickEdit from "./effects/clickEdit";
import listenSelectDomChange from "./dom/listenSelectDomChange";
import editingShowShade from "./effects/editingShowShade";

listenSelectDomChange(document);
hoverShowBorder(document);
editingShowShade(document);
clickEdit(document);
