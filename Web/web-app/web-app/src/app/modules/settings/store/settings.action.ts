import { createActionGroup, emptyProps } from '@ngrx/store';
import { UserSettings } from 'src/app/models/user-settings';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TemplateItem } from 'src/app/shared/models/template-item';

export const SettingsActions  = createActionGroup({
  source: 'Settings',
  events: {
    'Clean Store': emptyProps(),
    'Get Settings': emptyProps(),
    'Get Settings Success': (userSettings: UserSettings) => ({userSettings}),
    'Raise Exception': (reason: string, error: any) => ({reason, error}),
    'NotifyPerson': (reason: string, severity: string) => ({reason, severity}),
    'Save Categories': (categories?: Record<string, string[]> | null) => ({categories}),
    'Save Templates': (templates?: Record<string, TemplateItem[]> | null) => ({templates}),
    'Save Accounts': (accounts?: AccountItem[] | null) => ({accounts}),
    'Save Account': (account: AccountItem | null) => ({account}),
    'Save Settings': (userSettings: UserSettings) => ({userSettings}),
    'Save Settings Success': (userSettings: UserSettings) => ({userSettings}),
    'Delete Account': (id: string) => ({id}),
    'Update Language': (language: string) => ({language}),
    'Update Locale': (locale: string) => ({locale}),
    'Set Locale': (locale: string) => ({locale}),
  },
});
