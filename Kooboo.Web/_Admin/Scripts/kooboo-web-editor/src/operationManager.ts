import { Operation } from "./models/Operation";
import context from "./context";
import { OperationEventArgs } from "./events/OperationEvent";
class OperationManager {
  readonly operations: Array<Operation> = [];
  readonly backupOperations: Array<Operation> = [];

  previous() {
    let operation = this.operations.pop();
    if (operation) {
      operation.undo();
      this.backupOperations.push(operation);
      this.emit();
    }
  }

  next() {
    let operation = this.backupOperations.pop();
    if (operation) {
      operation.redo();
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
    context.operationEvent.emit(args);
  }
}

export default new OperationManager();
