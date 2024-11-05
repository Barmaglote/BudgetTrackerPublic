import { FilterMetadata } from "primeng/api";

export interface Filter {
  [s: string]: FilterMetadata | FilterMetadata[] | undefined
}
