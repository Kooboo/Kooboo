import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "./events/selectedDomEvent";
import { TinymceEvent } from "./events/tinymceEvent";

class Context {
  lastSelectedDomEventArgs: SelectedDomEventArgs | undefined;
  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
  tinymceEvent: TinymceEvent = new TinymceEvent();
  editing: boolean = false;
}

export default new Context();
