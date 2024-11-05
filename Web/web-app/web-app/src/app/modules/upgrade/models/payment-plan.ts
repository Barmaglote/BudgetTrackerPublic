export interface PaymentPlan {
  planId: string;
  price: number; // cents. Min 100
  currency: string;
  title: string;
  description: string;
}
