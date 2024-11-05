import { SocialAuthServiceConfig } from '@abacritt/angularx-social-login';
import {
  GoogleLoginProvider,
  //FacebookLoginProvider
} from '@abacritt/angularx-social-login';

export const SOCIAL_LOGIN_PROVIDER = {
  provide: 'SocialAuthServiceConfig',
  useValue: {
    autoLogin: false,
    providers: [
      {
        id: GoogleLoginProvider.PROVIDER_ID,
        provider: new GoogleLoginProvider(
          'dsfdsfsdfÄöalskdfäaölskdfäaölskdfälsdkfäödlaskfsadf.apps.googleusercontent.com'
        )
      },
      // {
      //   id: FacebookLoginProvider.PROVIDER_ID,
      //   provider: new FacebookLoginProvider('clientId')
      // }
    ],
    onError: (err) => {
      console.error(err);
    }
  } as SocialAuthServiceConfig,
}
