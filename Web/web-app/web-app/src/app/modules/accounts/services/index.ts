import { StatisticsService } from "../../item/services";
import { TransferService } from "./transfer.service";

export const SERVICES = [
  TransferService,
  StatisticsService
];

export * from "./transfer.service";
export * from "../../item/services";
