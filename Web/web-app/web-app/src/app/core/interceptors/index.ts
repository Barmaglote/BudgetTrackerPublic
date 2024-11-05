import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpErrorInterceptor } from './http-error.interceptor';
import { LanguageInterceptor } from './lang.interceptor';
import { JwtInterceptor } from './jwt.interceptor';
import { LoaderInterceptor } from './loader.interceptor';
import { CSRFInterceptor } from './csrf.interceptor';

export const HTTP_INTERCEPTORS_PROVIDERS = [
  { provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: LanguageInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: CSRFInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ];

export * from './http-error.interceptor';
export * from './lang.interceptor';
export * from './jwt.interceptor';
export * from './loader.interceptor';
export * from './csrf.interceptor';
