import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "./events/SelectedDomEvent";
import { TinymceDisplayEvent, TinymceInputEvent } from "./events/TinymceEvent";
import { OperationEvent } from "./events/OperationEvent";

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
  operationEvent: OperationEvent = new OperationEvent();
}

export default new Context();
