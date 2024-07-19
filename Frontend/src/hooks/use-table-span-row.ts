import { ref } from "vue";

export function spanTableRow() {
  const spanArr = ref<number[]>([]);
  const pos = ref();
  const getSpanArr = (data: any[], name: string) => {
    spanArr.value = [];
    pos.value = 0;
    for (let i = 0; i < data?.length; i++) {
      if (i === 0) {
        spanArr.value.push(1);
        pos.value = 0;
      } else {
        if (data[i][name] === data[i - 1][name]) {
          spanArr.value[pos.value] += 1;
          spanArr.value.push(0);
        } else {
          spanArr.value.push(1);
          pos.value = i;
        }
      }
    }
    return data;
  };
  const objectSpanMethod = ({ A }: any, columnIndexArray: number[]) => {
    if (columnIndexArray.includes(A.columnIndex)) {
      const _row = spanArr.value[A.rowIndex];
      const _col = _row > 0 ? 1 : 0;
      return {
        rowspan: _row,
        colspan: _col,
      };
    }
  };
  return {
    getSpanArr,
    objectSpanMethod,
  };
}
