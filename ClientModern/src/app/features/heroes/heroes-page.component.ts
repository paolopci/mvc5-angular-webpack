import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppApiError } from '../../core/api/api-contracts';
import { normalizeApiError } from '../../core/interceptors/problem-details.interceptor';
import { HeroesApiService } from './heroes-api.service';
import { HeroDetailViewModel, HeroListItemViewModel } from './heroes.models';

@Component({
  selector: 'sg-heroes-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="card">
      <header class="card__header">
        <p class="eyebrow">M3.A Slice</p>
        <h2>Heroes (read/search/detail)</h2>
      </header>

      <p class="hint">
        Prima slice successiva migrata dopo M2: usa endpoint reali <code>/api/heroes</code>
        su <code>WebCore9.Api</code> (non piu' mock in-memory del client legacy).
      </p>

      <form class="search-form" (ngSubmit)="search()" novalidate>
        <label>
          Cerca hero per nome
          <input
            type="search"
            [(ngModel)]="searchTerm"
            name="searchTerm"
            placeholder="Es. tor"
            autocomplete="off"
          />
        </label>
        <div class="search-form__actions">
          <button type="submit">Cerca</button>
          <button type="button" class="secondary" (click)="clearSearch()">Reset</button>
        </div>
      </form>

      @if (listLoading()) {
        <p class="state">Caricamento heroes...</p>
      } @else if (listError()) {
        <div class="error-box" role="alert">
          <strong>{{ listError()!.title }}</strong>
          <p>{{ listError()!.detail }}</p>
          <button type="button" (click)="loadHeroes()">Riprova</button>
        </div>
      } @else {
        <div class="layout">
          <section class="list-panel">
            <header class="list-panel__header">
              <h3>Heroes</h3>
              <span>{{ heroes().length }} item(s)</span>
            </header>

            @if (heroes().length === 0) {
              <p class="state">Nessun risultato.</p>
            } @else {
              <ul class="hero-list">
                @for (hero of heroes(); track hero.id) {
                  <li>
                    <button
                      type="button"
                      class="hero-list__item"
                      [class.is-selected]="selectedHeroId() === hero.id"
                      (click)="selectHero(hero.id)"
                    >
                      <span class="hero-list__id">#{{ hero.id }}</span>
                      <span>{{ hero.name }}</span>
                    </button>
                  </li>
                }
              </ul>
            }
          </section>

          <section class="detail-panel">
            <header class="list-panel__header">
              <h3>Detail</h3>
              @if (selectedHeroId()) {
                <span>#{{ selectedHeroId() }}</span>
              }
            </header>

            @if (detailLoading()) {
              <p class="state">Caricamento dettaglio...</p>
            } @else if (detailError()) {
              <div class="error-box" role="alert">
                <strong>{{ detailError()!.title }}</strong>
                <p>{{ detailError()!.detail }}</p>
                @if (selectedHeroId()) {
                  <button type="button" (click)="selectHero(selectedHeroId()!)">Riprova</button>
                }
              </div>
            } @else if (selectedHero()) {
              <dl class="hero-detail">
                <div>
                  <dt>Id</dt>
                  <dd>{{ selectedHero()!.id }}</dd>
                </div>
                <div>
                  <dt>Name</dt>
                  <dd>{{ selectedHero()!.name }}</dd>
                </div>
              </dl>
            } @else {
              <p class="state">Seleziona un hero dalla lista per vedere il dettaglio.</p>
            }
          </section>
        </div>
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
    .card__header h2 { margin: 0.25rem 0 0; font-size: 1.2rem; }
    .eyebrow { margin: 0; font-size: 0.8rem; letter-spacing: 0.08em; text-transform: uppercase; color: var(--muted-color); }
    .hint { color: var(--muted-color); }
    .search-form { display: grid; gap: 0.75rem; margin: 1rem 0; }
    .search-form label { display: grid; gap: 0.35rem; font-weight: 600; }
    .search-form input {
      border: 1px solid var(--border-color);
      border-radius: 10px;
      padding: 0.55rem 0.7rem;
      font: inherit;
    }
    .search-form__actions { display: flex; gap: 0.5rem; }
    button {
      border: 1px solid var(--accent-color);
      background: var(--accent-color);
      color: white;
      border-radius: 10px;
      padding: 0.45rem 0.75rem;
      cursor: pointer;
    }
    .secondary {
      background: white;
      color: var(--accent-color);
    }
    .layout {
      display: grid;
      grid-template-columns: minmax(260px, 1.1fr) minmax(220px, 0.9fr);
      gap: 0.9rem;
    }
    .list-panel, .detail-panel {
      border: 1px solid var(--border-color);
      border-radius: 12px;
      background: var(--surface-2);
      padding: 0.75rem;
    }
    .list-panel__header {
      display: flex;
      justify-content: space-between;
      gap: 0.5rem;
      align-items: baseline;
      margin-bottom: 0.5rem;
    }
    .list-panel__header h3 { margin: 0; font-size: 1rem; }
    .list-panel__header span { color: var(--muted-color); font-size: 0.85rem; }
    .hero-list { list-style: none; margin: 0; padding: 0; display: grid; gap: 0.45rem; }
    .hero-list__item {
      width: 100%;
      display: flex;
      align-items: center;
      gap: 0.6rem;
      text-align: left;
      background: white;
      color: var(--text-color);
      border: 1px solid var(--border-color);
      border-radius: 10px;
      padding: 0.45rem 0.55rem;
    }
    .hero-list__item.is-selected {
      border-color: var(--accent-color);
      background: color-mix(in srgb, var(--accent-color) 8%, white);
    }
    .hero-list__id {
      color: var(--muted-color);
      font-size: 0.85rem;
      min-width: 2.25rem;
    }
    .hero-detail { display: grid; gap: 0.6rem; margin: 0; }
    .hero-detail > div {
      border: 1px solid var(--border-color);
      background: white;
      border-radius: 10px;
      padding: 0.6rem;
    }
    .hero-detail dt { color: var(--muted-color); font-size: 0.85rem; }
    .hero-detail dd { margin: 0.3rem 0 0; font-weight: 600; }
    .state { color: var(--muted-color); }
    .error-box {
      border: 1px solid #e88b8b;
      background: #fff2f2;
      color: #7a1b1b;
      border-radius: 12px;
      padding: 0.75rem;
    }
    .error-box p { margin: 0.4rem 0; }
    @media (max-width: 760px) {
      .layout { grid-template-columns: 1fr; }
    }
  `
})
export class HeroesPageComponent {
  private readonly api = inject(HeroesApiService);

  protected readonly heroes = signal<HeroListItemViewModel[]>([]);
  protected readonly selectedHeroId = signal<number | null>(null);
  protected readonly selectedHero = signal<HeroDetailViewModel | null>(null);

  protected readonly listLoading = signal(false);
  protected readonly detailLoading = signal(false);
  protected readonly listError = signal<AppApiError | null>(null);
  protected readonly detailError = signal<AppApiError | null>(null);

  protected searchTerm = '';

  constructor() {
    this.loadHeroes();
  }

  protected loadHeroes(): void {
    this.listLoading.set(true);
    this.listError.set(null);

    this.api.getHeroes(this.searchTerm).subscribe({
      next: (heroes) => {
        this.heroes.set(heroes);
        this.listLoading.set(false);

        if (heroes.length === 0) {
          this.selectedHeroId.set(null);
          this.selectedHero.set(null);
          return;
        }

        const nextSelectedId = heroes.some((h) => h.id === this.selectedHeroId())
          ? this.selectedHeroId()
          : heroes[0]!.id;

        if (nextSelectedId != null) {
          this.selectHero(nextSelectedId);
        }
      },
      error: (error) => {
        this.heroes.set([]);
        this.selectedHeroId.set(null);
        this.selectedHero.set(null);
        this.listError.set(normalizeApiError(error));
        this.listLoading.set(false);
      }
    });
  }

  protected search(): void {
    this.loadHeroes();
  }

  protected clearSearch(): void {
    this.searchTerm = '';
    this.loadHeroes();
  }

  protected selectHero(id: number): void {
    this.selectedHeroId.set(id);
    this.detailLoading.set(true);
    this.detailError.set(null);

    this.api.getHeroById(id).subscribe({
      next: (hero) => {
        this.selectedHero.set(hero);
        this.detailLoading.set(false);
      },
      error: (error) => {
        this.selectedHero.set(null);
        this.detailError.set(normalizeApiError(error));
        this.detailLoading.set(false);
      }
    });
  }
}
