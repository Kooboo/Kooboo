import { Operation } from "./models/Operation";
class OperationManager {
  readonly operations: Array<Operation> = [];
  readonly backupOperations: Array<Operation> = [];

  previous() {
    let operation = this.operations.pop();
    if (operation) {
      operation.undo();
      this.backupOperations.push(operation);
    }
  }

  next() {
    let operation = this.backupOperations.pop();
    if (operation) {
      operation.undo();
      this.operations.push(operation);
    }
  }

  add(operation: Operation) {
    this.operations.push(operation);
    this.backupOperations.splice(0, this.backupOperations.length);
  }
}

export default new OperationManager();
