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
    path: 'heroes/dashboard',
    loadComponent: () =>
      import('./features/heroes/heroes-dashboard-page.component').then(
        (m) => m.HeroesDashboardPageComponent
      )
  },
  {
    path: 'heroes/search',
    loadComponent: () =>
      import('./features/heroes/heroes-search-page.component').then((m) => m.HeroesSearchPageComponent)
  },
  {
    path: 'heroes',
    loadComponent: () =>
      import('./features/heroes/heroes-page.component').then((m) => m.HeroesPageComponent)
  },
  {
    path: 'heroes/:id',
    loadComponent: () =>
      import('./features/heroes/hero-detail-page.component').then((m) => m.HeroDetailPageComponent)
  },
  {
    path: '**',
    redirectTo: 'health'
  }
];
