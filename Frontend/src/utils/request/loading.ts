import { ElLoading } from "element-plus";

let count = 0;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
let instance: any;

export default {
  open(): void {
    if (count === 0) {
      instance = ElLoading.service({
        background: "rgba(0, 0, 0, 0.5)",
      });
    }
    count++;
  },
  close(): void {
    count--;
    if (count <= 0) {
      count = 0;
      if (instance) {
        instance.close();
        instance = null;
      }
    }
  },
};
