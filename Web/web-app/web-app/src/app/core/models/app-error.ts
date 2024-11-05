import { Message } from 'primeng/api';

const DEFAULT_WHERROR_PMESSAGE: Message = {
  severity: 'warn',
  sticky: false,
  closable: true,
  summary: '',
  life: 5000,
};

export class AppError implements Error {
  name: string;
  message: string;
  vibrate: boolean;
  pMessage: Message;

  constructor();
  constructor(message?: string);
  constructor(message?: string, vibrate?: boolean);
  constructor(message?: string, vibrate?: boolean, pMessage?: Message);
  constructor(message?: string, vibrate?: boolean, pMessage?: Message, name?: string) {
    this.name = name ?? 'AppError';
    this.message = message ?? 'Unspecified error.';
    this.vibrate = vibrate ?? false;
    this.pMessage = pMessage ?? DEFAULT_WHERROR_PMESSAGE;
    if (pMessage?.severity === 'error') this.pMessage.sticky = true;
    if (!pMessage?.detail) this.pMessage.detail = this.message;
  }
}
