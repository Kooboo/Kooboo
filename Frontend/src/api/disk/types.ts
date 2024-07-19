export interface Disk {
  repositoryEditCount: number;
  repositorySize: {
    itemCount: number;
    length: number;
    name: string;
    size: string;
  }[];
  total: number;
  totalSize: string;
}
