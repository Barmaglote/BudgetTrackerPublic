import { Payment } from "./payment";

export class CreditCalculator {
  private loanAmount: number;
  private annualInterestRate: number;
  private mandatoryPayment: number;
  private lumpSumPayment: boolean;
  private loanTermMonths: number;
  private startDate: Date;

  constructor(
    loanAmount: number,
    annualInterestRate: number,
    mandatoryPayment: number,
    lumpSumPayment: boolean,
    loanTermMonths: number,
    startDate: Date
  ) {
    this.loanAmount = loanAmount;
    this.annualInterestRate = annualInterestRate;
    this.mandatoryPayment = mandatoryPayment;
    this.lumpSumPayment = lumpSumPayment;
    this.loanTermMonths = loanTermMonths;
    this.startDate = startDate;
  }

  private calculateMonthlyPayment(): number {
    const monthlyInterestRate = this.annualInterestRate / 12 / 100;
    const numerator = Math.pow(1 + monthlyInterestRate, this.loanTermMonths);
    const denominator = numerator - 1;
    const monthlyPayment = (this.loanAmount * monthlyInterestRate * numerator) / denominator;

    return this.lumpSumPayment
      ? monthlyPayment + this.mandatoryPayment
      : monthlyPayment;
  }

  getTotalLoanAmount(): number {
    return this.loanAmount;
  }

  getTotalRepayment(): number {
    const monthlyPayment = this.calculateMonthlyPayment();
    return monthlyPayment * this.loanTermMonths;
  }

  getTotalInterestPaid(): number {
    return this.getTotalRepayment() - this.loanAmount;
  }

  getPaymentSchedule(): Payment[] {
    const monthlyPayment = this.calculateMonthlyPayment();
    const schedule: Payment[] = [];

    let remainingLoanAmount = this.loanAmount;
    let currentDate = new Date(this.startDate);

    for (let month = 1; month <= this.loanTermMonths; month++) {
      const quantity = monthlyPayment;

      schedule.push({ month, quantity, date: new Date(currentDate), isPaid: false });
      remainingLoanAmount -= quantity;

      // If there's a lump sum payment, apply it once
      if (this.lumpSumPayment && month === 1) {
        remainingLoanAmount -= this.mandatoryPayment;
      }

      // Update date for the next month
      currentDate.setMonth(currentDate.getMonth() + 1);
    }

    if (this.lumpSumPayment === false && this.mandatoryPayment > 0) {
      currentDate.setMonth(currentDate.getMonth() + 1);
      schedule.push({ month: this.loanTermMonths+1, quantity: this.mandatoryPayment, date: new Date(currentDate), isPaid: false });
    }

    return schedule;
  }
}
