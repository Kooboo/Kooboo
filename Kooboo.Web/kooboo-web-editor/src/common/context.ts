import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "../events/SelectedDomEvent";
import { TinymceInputEvent } from "../events/TinymceEvent";
import { OperationEvent } from "../events/OperationEvent";
import { EditableEvent } from "../events/EditableEvent";
import { OperationManager } from "../models/OperationManager";
import { FloatMenuClickEvent } from "../events/FloatMenuClickEvent";
import { HoverDomEvent, HoverDomEventArgs } from "../events/HoverDomEvent";

class Context {
  private _editing: boolean = false;
  set editing(value: boolean) {
    this._editing = value;
    this.editableEvent.emit(value);
  }
  get editing() {
    return this._editing;
  }

  operationManager: OperationManager = new OperationManager();
  lastSelectedDomEventArgs!: SelectedDomEventArgs;
  lastHoverDomEventArgs!: HoverDomEventArgs;
  lastMouseEventArg!: MouseEvent;

  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
  editableEvent: EditableEvent = new EditableEvent();
  tinymceInputEvent: TinymceInputEvent = new TinymceInputEvent();
  operationEvent: OperationEvent = new OperationEvent();
  floatMenuClickEvent: FloatMenuClickEvent = new FloatMenuClickEvent();
  hoverDomEvent: HoverDomEvent = new HoverDomEvent();
}

export default new Context();
