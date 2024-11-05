import { TransferItem } from "src/app/shared/models/transfer-item";

export interface TransfersResponse {
  items: TransferItem[];
  totalCount: number;
}
