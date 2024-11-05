import { AuthenticationService } from './authentication.service';
import { LanguageSettingsService } from './language-settings.service';
import { LoaderService } from './loader.service';
import { TranslocoHttpLoader } from './transloco.loader';

export const SERVICES  =  [
  AuthenticationService,
  LanguageSettingsService,
  LoaderService,
  TranslocoHttpLoader
]

export * from './authentication.service';
export * from './language-settings.service';
export * from './loader.service';
export * from './transloco.loader';
