import { Payment } from "./payment";

export interface PaymentInfo {
  payment: Payment;
  accountId: string;
  creditTitle: string;
  creditId: string;
}
