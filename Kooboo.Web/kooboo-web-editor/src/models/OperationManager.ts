import { Operation } from "./Operation";
import context from "../common/context";
import { OperationEventArgs } from "../events/OperationEvent";
import { cleanKoobooInfo } from "../common/koobooInfo";
import { OperationLogItem } from "./OperationLog";
import { OBJECT_TYPE, EDITOR_TYPE } from "../common/constants";
export class OperationManager {
  readonly operations: Array<Operation> = [];
  readonly backupOperations: Array<Operation> = [];

  previous(document: Document) {
    let operation = this.operations.pop();
    if (operation) {
      operation.undo(document);
      this.backupOperations.push(operation);
      this.emit();
    }
  }

  next(document: Document) {
    let operation = this.backupOperations.pop();
    if (operation) {
      operation.redo(document);
      this.operations.push(operation);
      this.emit();
    }
  }

  add(operation: Operation) {
    this.operations.push(operation);
    this.backupOperations.splice(0, this.backupOperations.length);
    this.emit();
  }

  private emit() {
    let args = new OperationEventArgs(
      this.operations.length,
      this.backupOperations.length
    );
    console.log(this.operations, this.backupOperations);
    context.operationEvent.emit(args);
  }

  get operationLogs() {
    var logs = this.getMargedOperations().map(m => {
      let editorType = m.editorType;
      let objecttype = m.koobooComment.objecttype;

      if (objecttype && objecttype == OBJECT_TYPE.label) {
        editorType = EDITOR_TYPE.label;
      } else if (objecttype && objecttype == OBJECT_TYPE.content) {
        editorType = EDITOR_TYPE.content;
      } else if (objecttype && objecttype == OBJECT_TYPE.style) {
        editorType = EDITOR_TYPE.attribute;
      }

      let nameOrId = m.koobooComment.nameorid
        ? m.koobooComment.nameorid
        : m.koobooComment.bindingvalue;

      let log = new OperationLogItem();
      log.action = m.actionType;
      log.editorType = editorType;
      log.koobooId = m.koobooId!;
      log.nameOrId = nameOrId!;
      log.objectType = objecttype!;
      log.value = cleanKoobooInfo(m.commit);
      log.fieldName = m.koobooComment.fieldname!;
      log.attributeName = m.koobooComment.attributename!;
      return log;
    });
    return logs;
  }

  private getMargedOperations() {
    let result: Array<Operation> = [];
    this.operations.forEach(o => {
      var exist = result.filter(
        f =>
          f.actionType == o.actionType &&
          f.koobooId &&
          o.koobooId &&
          f.koobooId.startsWith(o.koobooId)
      );
      if (exist.length > 0) {
        result = result.filter(f => exist.some(s => s != f));
      }
      result.push(o);
    });

    return result;
  }
}
