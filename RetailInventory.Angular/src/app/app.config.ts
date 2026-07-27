import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners
} from '@angular/core';

import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';

import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

import { authInterceptor } from './core/interceptors/auth.interceptor';

import { RuntimeConfigService } from './core/configuration/runtime-config.service';

export const appConfig: ApplicationConfig = {

  providers: [

    provideBrowserGlobalErrorListeners(),

    provideRouter(routes),

    provideHttpClient(
      withInterceptors([
        authInterceptor
      ])
    ),

    provideAppInitializer(() => {

      const runtimeConfig = inject(RuntimeConfigService);

      return runtimeConfig.load();

    })

  ]

};