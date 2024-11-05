import { AccountType } from "../enums";

export interface AccountItem {
  id?: string,
  title: string;
  initial: number;
	quantity: number;
  limit: number;
	comment:  string;
  accountType: AccountType | undefined;
  currency: string;
  isDeleted: boolean;
  goal: number;
}

