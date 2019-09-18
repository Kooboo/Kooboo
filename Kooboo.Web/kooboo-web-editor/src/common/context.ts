import { operationManager } from "@/operation/Manager";
import { SelectedDomEventArgs, SelectedDomEvent } from "@/events/SelectedDomEvent";
import { HoverDomEventArgs, HoverDomEvent } from "@/events/HoverDomEvent";
import { EditableEvent } from "@/events/EditableEvent";
import { TinymceInputEvent } from "@/events/TinymceEvent";
import { OperationEvent } from "@/events/OperationEvent";
import { CloseEditingEvent } from "@/events/CloseEditingEvent";
import { createContainer } from "./utils";

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
  container: HTMLElement = createContainer();
  lastSelectedDomEventArgs!: SelectedDomEventArgs;
  lastHoverDomEventArgs!: HoverDomEventArgs;
  lastMouseEventArg!: MouseEvent;
  floatMenuClosing: boolean = false;

  domChangeEvent: SelectedDomEvent = new SelectedDomEvent();
  editableEvent: EditableEvent = new EditableEvent();
  tinymceInputEvent: TinymceInputEvent = new TinymceInputEvent();
  operationEvent: OperationEvent = new OperationEvent();
  hoverDomEvent: HoverDomEvent = new HoverDomEvent();
  closeEditingEvent: CloseEditingEvent = new CloseEditingEvent();
}

export default new Context();
