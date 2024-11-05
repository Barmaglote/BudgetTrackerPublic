import { CreditItem } from "./credit-item";

export interface CreditsResponse {
  items: CreditItem[];
  totalCount: number;
}
