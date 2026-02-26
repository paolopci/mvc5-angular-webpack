import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { APP_ENVIRONMENT } from './core/config/app-environment.token';
import { appEnvironment } from './core/config/app-environment';
import { problemDetailsInterceptor } from './core/interceptors/problem-details.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([problemDetailsInterceptor])),
    { provide: APP_ENVIRONMENT, useValue: appEnvironment }
  ]
};
