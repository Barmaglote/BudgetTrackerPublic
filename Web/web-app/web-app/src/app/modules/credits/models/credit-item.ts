import ObjectID from "bson-objectid";
import { Payment } from "./payment";

export interface CreditItem {
  id: ObjectID;
  idString: string;
	category: string;
	quantity: number;
  months: number;
  rate: number;
	date:     Date;
	comment:  string;
  accountId: string;
  isActive: boolean;
  mandatory: number;
  isIncluded: boolean;
  plan: Payment[] | undefined;
}
