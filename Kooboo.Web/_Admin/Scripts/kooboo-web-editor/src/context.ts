import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "./models/selectedDomEvent";

class Context {
  lastSelectedDomEventArgs: SelectedDomEventArgs | undefined;
  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
}

export default new Context();
