import { AccountItem } from "./account-item";

export interface TransferItem {
  transactionId?: string,
  fromAccount: AccountItem;
  toAccount: AccountItem;
	fromQuantity: number;
  toQuantity: number;
  date: Date;
}

