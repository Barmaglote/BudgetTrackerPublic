import { LoginService } from "./login.service";
import { ProductService } from "./products.service";

export const SERVICES = [
  ProductService,
  LoginService
]

export * from "./products.service";
export * from "./login.service";
