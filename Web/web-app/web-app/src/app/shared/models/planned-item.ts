import ObjectID from "bson-objectid";

export interface PlannedItem {
  id: ObjectID;
  idString: string;
	quantity: number;
  accountId: string;
  comment: string;
  isPaid: boolean;
  currency: string;
  date: Date;
  area: string;
}
