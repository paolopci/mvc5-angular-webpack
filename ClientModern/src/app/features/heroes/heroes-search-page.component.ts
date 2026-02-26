import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AppApiError } from '../../core/api/api-contracts';
import { normalizeApiError } from '../../core/interceptors/problem-details.interceptor';
import { HeroesApiService } from './heroes-api.service';
import { HeroesHubNavComponent } from './heroes-hub-nav.component';
import { HeroListItemViewModel } from './heroes.models';

@Component({
  selector: 'sg-heroes-search-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, HeroesHubNavComponent],
  template: `
    <section class="card">
      <header class="card__header">
        <p class="eyebrow">Module2 Parity Route</p>
        <h2>Hero Search</h2>
      </header>

      <p class="hint">Ricerca dedicata stile legacy con debounce live e link al dettaglio.</p>

      <sg-heroes-hub-nav />

      <label class="search-field">
        Search
        <input
          type="search"
          [(ngModel)]="searchTerm"
          (input)="onSearchInput()"
          placeholder="Type a hero name..."
          autocomplete="off"
          (keydown.escape)="clearSearch()"
        />
      </label>

      @if (loading()) {
        <p class="state">Ricerca in corso...</p>
      } @else if (error()) {
        <div class="error-box" role="alert">
          <strong>{{ error()!.title }}</strong>
          <p>{{ error()!.detail }}</p>
        </div>
      } @else if (searchTerm.trim().length === 0) {
        <p class="state">Inizia a digitare per cercare heroes.</p>
      } @else if (results().length === 0) {
        <p class="state">Nessun risultato per "{{ searchTerm.trim() }}".</p>
      } @else {
        <ul class="search-results">
          @for (hero of results(); track hero.id) {
            <li>
              <a [routerLink]="['/heroes', hero.id]">
                <span>#{{ hero.id }}</span>
                <strong>{{ hero.name }}</strong>
              </a>
            </li>
          }
        </ul>
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
    .search-field { display: grid; gap: 0.35rem; font-weight: 600; margin: 0.5rem 0 0.75rem; }
    .search-field input {
      border: 1px solid var(--border-color);
      border-radius: 10px;
      padding: 0.55rem 0.7rem;
      font: inherit;
      background: white;
    }
    .search-results {
      list-style: none;
      margin: 0;
      padding: 0;
      display: grid;
      gap: 0.45rem;
    }
    .search-results a {
      display: flex;
      gap: 0.6rem;
      align-items: center;
      border: 1px solid var(--border-color);
      background: white;
      border-radius: 10px;
      padding: 0.55rem 0.65rem;
      color: var(--text-color);
      text-decoration: none;
    }
    .search-results a:hover, .search-results a:focus-visible {
      border-color: var(--accent-color);
      outline: none;
      background: color-mix(in srgb, var(--accent-color) 4%, white);
    }
    .search-results span { color: var(--muted-color); min-width: 2rem; }
    .state { color: var(--muted-color); }
    .error-box {
      border: 1px solid #e88b8b;
      background: #fff2f2;
      color: #7a1b1b;
      border-radius: 12px;
      padding: 0.75rem;
    }
    .error-box p { margin: 0.4rem 0 0; }
  `
})
export class HeroesSearchPageComponent implements OnDestroy {
  private readonly api = inject(HeroesApiService);

  protected searchTerm = '';
  protected readonly loading = signal(false);
  protected readonly error = signal<AppApiError | null>(null);
  protected readonly results = signal<HeroListItemViewModel[]>([]);

  private debounceHandle: number | null = null;

  ngOnDestroy(): void {
    this.clearDebounce();
  }

  protected onSearchInput(): void {
    this.clearDebounce();
    const term = this.searchTerm.trim();
    if (!term) {
      this.loading.set(false);
      this.error.set(null);
      this.results.set([]);
      return;
    }

    this.debounceHandle = window.setTimeout(() => {
      this.debounceHandle = null;
      this.runSearch(term);
    }, 300);
  }

  protected clearSearch(): void {
    this.clearDebounce();
    this.searchTerm = '';
    this.loading.set(false);
    this.error.set(null);
    this.results.set([]);
  }

  private runSearch(term: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getHeroes(term).subscribe({
      next: (heroes) => {
        this.results.set(heroes);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(normalizeApiError(error));
        this.results.set([]);
        this.loading.set(false);
      }
    });
  }

  private clearDebounce(): void {
    if (this.debounceHandle != null) {
      window.clearTimeout(this.debounceHandle);
      this.debounceHandle = null;
    }
  }
}
