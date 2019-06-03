import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "./models/selectedDomEvent";

class Context {
  lastSelectedDomEventArgs: SelectedDomEventArgs | undefined;
  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
  editing: boolean = false;
}

export default new Context();
