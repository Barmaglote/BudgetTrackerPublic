import { createActionGroup, emptyProps } from '@ngrx/store';

export const LoginActions  = createActionGroup({
  source: 'Login',
  events: {
    'Clean Store': emptyProps(),
    'Sign Up': (title: string, email: string, password: string, recaptcha: string) => ({title, email, password, recaptcha}),
    'Failure': (text: string, error: any) => ({text, error}),
    'Show Sign Up Confirmation': emptyProps(),
    'Confirmation': (login: string, token: string | null | undefined) => ({login, token}),
    'Sign In': (login: string, password: string, recaptcha: string) => ({login, password, recaptcha}),
    'Show Start Page': emptyProps(),
    'Restore Request': (login: string, recaptcha: string) => ({login, recaptcha}),
    'Show Restore Request Confirmation': emptyProps(),
    'Show Restore Confirmation': emptyProps(),
    'Restore': (password: string, token: string, login: string) => ({login, password, token}),
    'Change Password Request': (login: string, password: string, newpassword: string) => ({login, password, newpassword}),
    'Change Password Request Confirmation': emptyProps(),
  },
});
