import { kvInfo } from "@/common/kvInfo";

export class Log {
  source: string;
  infos: kvInfo[];

  /**
   *
   */
  constructor(infos: kvInfo[]) {
    this.infos = infos;
    this.source = infos.find(f => f.key == "source")!.value!;
  }
}
