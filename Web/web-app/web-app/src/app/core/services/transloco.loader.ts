import { TRANSLOCO_LOADER, TranslocoLoader } from '@ngneat/transloco';
import { Injectable } from '@angular/core';
import { from } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TranslocoHttpLoader implements TranslocoLoader {

  getTranslation(langPath: string) {
    console.log("langPath", langPath);
    return from(import(`./../../../assets/i18n/${langPath}.json`));
  }
}

export const translocoLoader = { provide: TRANSLOCO_LOADER, useClass: TranslocoHttpLoader };

