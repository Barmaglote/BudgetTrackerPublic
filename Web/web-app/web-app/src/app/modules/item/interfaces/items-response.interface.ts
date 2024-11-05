import { Item } from "../models";

export interface ItemsResponse {
  items: Item[];
  totalCount: number;
}
