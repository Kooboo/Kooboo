import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "./events/selectedDomEvent";
import { TinymceDisplayEvent, TinymceInputEvent } from "./events/tinymceEvent";

class Context {
  private _editing: boolean = false;
  set editing(value: boolean) {
    this._editing = value;
    this.tinymceDisplayEvent.emit(value);
  }
  get editing() {
    return this._editing;
  }

  lastSelectedDomEventArgs: SelectedDomEventArgs | undefined;
  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
  tinymceDisplayEvent: TinymceDisplayEvent = new TinymceDisplayEvent();
  tinymceInputEvent: TinymceInputEvent = new TinymceInputEvent();
}

export default new Context();
