import {
  SelectedDomEventArgs,
  SelectedDomEvent
} from "../events/SelectedDomEvent";
import { TinymceInputEvent } from "../events/TinymceEvent";
import { OperationEvent } from "../events/OperationEvent";
import { EditableEvent } from "../events/EditableEvent";
import { FloatMenuClickEvent } from "../events/FloatMenuClickEvent";
import { HoverDomEvent, HoverDomEventArgs } from "../events/HoverDomEvent";
import { operationManager } from "@/operation/Manager";

class Context {
  private _editing: boolean = false;
  set editing(value: boolean) {
    this._editing = value;
    this.editableEvent.emit(value);
  }
  get editing() {
    return this._editing;
  }

  operationManager: operationManager = new operationManager();
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
