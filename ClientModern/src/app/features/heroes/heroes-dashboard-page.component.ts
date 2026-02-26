import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppApiError } from '../../core/api/api-contracts';
import { normalizeApiError } from '../../core/interceptors/problem-details.interceptor';
import { HeroesApiService } from './heroes-api.service';
import { HeroesHubNavComponent } from './heroes-hub-nav.component';
import { HeroListItemViewModel } from './heroes.models';

@Component({
  selector: 'sg-heroes-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterLink, HeroesHubNavComponent],
  template: `
    <section class="card">
      <header class="card__header">
        <p class="eyebrow">Module2 Parity Route</p>
        <h2>Heroes Dashboard</h2>
      </header>

      <p class="hint">
        Route dedicata in stile Tour of Heroes: top heroes + navigazione veloce a detail/search.
      </p>

      <sg-heroes-hub-nav />

      @if (loading()) {
        <p class="state">Caricamento top heroes...</p>
      } @else if (error()) {
        <div class="error-box" role="alert">
          <strong>{{ error()!.title }}</strong>
          <p>{{ error()!.detail }}</p>
          <button type="button" (click)="load()">Riprova</button>
        </div>
      } @else {
        <section aria-label="Top heroes">
          <h3>Top Heroes</h3>
          <div class="hero-grid">
            @for (hero of topHeroes(); track hero.id) {
              <a class="hero-card" [routerLink]="['/heroes', hero.id]">
                <span class="hero-card__badge">#{{ hero.id }}</span>
                <strong>{{ hero.name }}</strong>
              </a>
            }
          </div>
        </section>

        <section class="quick-links" aria-label="Quick actions">
          <a routerLink="/heroes/search">Apri Hero Search</a>
          <a routerLink="/heroes">Apri Heroes CRUD</a>
        </section>
      }
    </section>
  `,
  styles: `
    .card {
      border: 1px solid var(--border-color);
      border-radius: 16px;
      background: var(--surface-1);
      padding: 1rem;
      box-shadow: 0 8px 30px rgba(16, 24, 40, 0.08);
    }
    .eyebrow { margin: 0; font-size: 0.8rem; letter-spacing: 0.08em; text-transform: uppercase; color: var(--muted-color); }
    .card__header h2 { margin: 0.25rem 0 0; }
    .hint { color: var(--muted-color); }
    h3 { margin: 0.5rem 0 0.75rem; }
    .hero-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
      gap: 0.75rem;
    }
    .hero-card {
      display: grid;
      gap: 0.4rem;
      text-decoration: none;
      color: var(--text-color);
      background: white;
      border: 1px solid var(--border-color);
      border-radius: 12px;
      padding: 0.8rem;
    }
    .hero-card:hover, .hero-card:focus-visible {
      border-color: var(--accent-color);
      outline: none;
      background: color-mix(in srgb, var(--accent-color) 5%, white);
    }
    .hero-card__badge {
      color: var(--muted-color);
      font-size: 0.85rem;
    }
    .quick-links {
      margin-top: 1rem;
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }
    .quick-links a {
      text-decoration: none;
      border: 1px solid var(--border-color);
      border-radius: 999px;
      padding: 0.35rem 0.7rem;
      color: var(--text-color);
      background: white;
    }
    .quick-links a:hover, .quick-links a:focus-visible {
      border-color: var(--accent-color);
      outline: none;
    }
    .state { color: var(--muted-color); }
    .error-box {
      border: 1px solid #e88b8b;
      background: #fff2f2;
      color: #7a1b1b;
      border-radius: 12px;
      padding: 0.75rem;
    }
    .error-box p { margin: 0.4rem 0; }
    button {
      border: 1px solid var(--accent-color);
      background: var(--accent-color);
      color: white;
      border-radius: 10px;
      padding: 0.4rem 0.7rem;
      cursor: pointer;
    }
  `
})
export class HeroesDashboardPageComponent {
  private readonly api = inject(HeroesApiService);

  protected readonly loading = signal(false);
  protected readonly error = signal<AppApiError | null>(null);
  protected readonly topHeroes = signal<HeroListItemViewModel[]>([]);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getTopHeroes(4).subscribe({
      next: (heroes) => {
        this.topHeroes.set(heroes);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(normalizeApiError(error));
        this.loading.set(false);
      }
    });
  }
}
