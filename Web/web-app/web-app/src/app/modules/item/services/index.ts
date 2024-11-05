import { HealthService } from "./health.service";
import { ItemService } from "./item.service";
import { StatisticsService } from "./statistics.service";

export const SERVICES = [
  HealthService,
  ItemService,
  StatisticsService
]

export * from "./item.service";
export * from "./health.service";
export * from "./statistics.service";
