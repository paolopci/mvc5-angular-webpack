import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'health'
  },
  {
    path: 'health',
    loadComponent: () =>
      import('./features/health/health-page.component').then((m) => m.HealthPageComponent)
  },
  {
    path: 'modules',
    loadComponent: () =>
      import('./features/home-modules/home-modules-page.component').then(
        (m) => m.HomeModulesPageComponent
      )
  },
  {
    path: '**',
    redirectTo: 'health'
  }
];
