import ObjectID from "bson-objectid";

export class Item {
  id: ObjectID;
  idString: string;
	category: string;
	quantity: number;
	date:     Date;
	comment:  string;
  isRegular: boolean;
  accountId: string;
  transactionId: string;

  public get month() {
    return new Date(this.date).toISOString().slice(0, 7);
  }

  public get day() {
    return new Date(this.date).getTime();
  }

  constructor(category: string, quantity: number, date: Date, comment: string, id: ObjectID, idString: string, isRegular: boolean, accountId: string, transactionId: string) {
    this.category = category;
    this.quantity = quantity;
    this.date = date;
    this.comment = comment;
    this.id = id;
    this.idString = idString;
    this.isRegular = isRegular;
    this.accountId = accountId;
    this.transactionId = transactionId;
  }
}
