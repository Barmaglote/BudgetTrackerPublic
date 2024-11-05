
import { AccountCardComponent } from "./account-card/account-card.component";
import { AccountStructureComponent } from "./account-structure/account-structure.component";
import { AddTransferComponent } from "./add-transfer/add-transfer.component";
import { ItemsListComponent } from "./items-list/items-list.component";
import { TransactionsTableComponent } from "./transactions-table/transactions-table.component";

export const COMPONENTS = [
  AccountStructureComponent,
  AddTransferComponent,
  AccountCardComponent,
  ItemsListComponent,
  TransactionsTableComponent
];

export * from "./account-card/account-card.component";
export * from "./account-structure/account-structure.component";
export * from "./add-transfer/add-transfer.component"
export * from "./items-list/items-list.component";
export * from "./transactions-table/transactions-table.component";
