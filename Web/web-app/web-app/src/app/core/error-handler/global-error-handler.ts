import { ErrorHandler, Injectable, NgZone, OnDestroy, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { AppError } from '../models';
import { Subject, Subscription, debounceTime, takeUntil } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class GlobalErrorHandler implements ErrorHandler, OnDestroy {
  private errorSubject = new Subject<AppError>();
  private destroy$ = new Subject<void>();
  private _errorSubjectSubscription: Subscription | undefined;

  constructor(private messageService: MessageService, private zone: NgZone) {
    this.setupErrorHandling();
  }

  private setupErrorHandling() {

    this._errorSubjectSubscription = this.errorSubject.pipe(debounceTime(2000))
      .pipe(
        takeUntil(this.destroy$)
      )
      .subscribe((error: AppError) => {
        this.zone.run(() => {
          this.messageService.clear();
          this.messageService.add({ ...error.pMessage, life: 2000 });
          if (error.vibrate) window.navigator.vibrate(500);

          setTimeout(() => {
            this.messageService.clear();
          }, 3000);
        });
      });
  }

  handleError(error: any): void {
    if (!(error instanceof AppError)) throw error;

    this.errorSubject.next(error);
    console.log('GlobalErrorHandler:', error);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    if (this._errorSubjectSubscription) {
      this._errorSubjectSubscription.unsubscribe();
    }
  }
}
