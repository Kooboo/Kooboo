import { Operation } from "./Operation";
import context from "../context";
import { OperationEventArgs } from "../events/OperationEvent";
import { cleanKoobooInfo } from "../common/koobooInfo";
import { OperationLogItem } from "./OperationLog";
import { OBJECT_TYPE } from "../constants";
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
      let types = [OBJECT_TYPE.label, OBJECT_TYPE.content];
      let objecttype = m.koobooComment.objecttype;
      if (objecttype && types.some(s => s == objecttype!.toLowerCase())) {
        objecttype = objecttype.toLowerCase();
      } else {
        objecttype = OBJECT_TYPE.dom;
      }

      let nameOrId = m.koobooComment.nameorid
        ? m.koobooComment.nameorid
        : m.koobooComment.bindingvalue;

      let log = new OperationLogItem();
      log.action = m.actionType;
      log.editorType = objecttype;
      log.koobooId = m.koobooId!;
      log.nameOrId = nameOrId!;
      log.objectType = m.koobooComment.objecttype!;
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
