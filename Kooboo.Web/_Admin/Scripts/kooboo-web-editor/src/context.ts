import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "./events/selectedDomEvent";
import { TinymceEvent } from "./events/tinymceEvent";

class Context {
  private _editing: boolean = false;
  set editing(value: boolean) {
    this._editing = value;
    this.tinymceEvent.emit(value);
  }
  get editing() {
    return this._editing;
  }

  lastSelectedDomEventArgs: SelectedDomEventArgs | undefined;
  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
  tinymceEvent: TinymceEvent = new TinymceEvent();
}

export default new Context();
