import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject, signal } from '@angular/core';
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
        <p class="eyebrow">M3.A+ Slice</p>
        <h2>Heroes (CRUD + search/detail)</h2>
      </header>

      <p class="hint">
        Slice evoluta di parity <code>Module2</code>: search/detail + mutation flow (add/edit/delete)
        con endpoint reali <code>/api/heroes</code> su <code>WebCore9.Api</code>.
      </p>

      <form class="create-form" (ngSubmit)="createHero()" novalidate>
        <label>
          Aggiungi hero
          <input
            type="text"
            [(ngModel)]="newHeroName"
            name="newHeroName"
            placeholder="Es. Windstorm"
            autocomplete="off"
            [disabled]="mutationBusy()"
          />
        </label>
        <button type="submit" [disabled]="mutationBusy() || !newHeroName.trim()">Aggiungi</button>
      </form>

      <form class="search-form" (ngSubmit)="search()" novalidate>
        <label>
          Cerca hero per nome
          <input
            type="search"
            [(ngModel)]="searchTerm"
            (input)="onSearchInputChanged()"
            name="searchTerm"
            placeholder="Es. tor"
            autocomplete="off"
            [disabled]="listLoading() || mutationBusy()"
          />
        </label>
        <div class="search-form__actions">
          <button type="submit" [disabled]="listLoading() || mutationBusy()">Cerca</button>
          <button type="button" class="secondary" (click)="clearSearch()" [disabled]="listLoading() || mutationBusy()">
            Reset
          </button>
          <span class="search-form__hint">Ricerca automatica con debounce (350ms)</span>
        </div>
      </form>

      @if (mutationSuccess()) {
        <p class="success-box" role="status">{{ mutationSuccess() }}</p>
      }
      @if (mutationError()) {
        <div class="error-box" role="alert">
          <strong>{{ mutationError()!.title }}</strong>
          <p>{{ mutationError()!.detail }}</p>
        </div>
      }

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
              <form class="edit-form" (ngSubmit)="saveSelectedHero()" novalidate>
                <dl class="hero-detail">
                  <div>
                    <dt>Id</dt>
                    <dd>{{ selectedHero()!.id }}</dd>
                  </div>
                  <div>
                    <dt>Name</dt>
                    <dd>
                      <input
                        type="text"
                        [(ngModel)]="editHeroName"
                        name="editHeroName"
                        autocomplete="off"
                        [disabled]="mutationBusy()"
                      />
                    </dd>
                  </div>
                </dl>

                <div class="detail-actions">
                  <button type="submit" [disabled]="mutationBusy() || !editHeroName.trim()">Salva</button>
                  <button type="button" class="secondary" (click)="resetEditName()" [disabled]="mutationBusy()">
                    Annulla
                  </button>
                  <button type="button" class="danger" (click)="deleteSelectedHero()" [disabled]="mutationBusy()">
                    Elimina
                  </button>
                </div>
              </form>
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
    .create-form, .search-form { display: grid; gap: 0.75rem; margin: 1rem 0; }
    .create-form { grid-template-columns: 1fr auto; align-items: end; }
    .create-form label, .search-form label { display: grid; gap: 0.35rem; font-weight: 600; }
    .create-form input, .search-form input {
      border: 1px solid var(--border-color);
      border-radius: 10px;
      padding: 0.55rem 0.7rem;
      font: inherit;
      width: 100%;
      background: white;
    }
    .search-form__actions { display: flex; flex-wrap: wrap; gap: 0.5rem; align-items: center; }
    .search-form__hint { color: var(--muted-color); font-size: 0.8rem; }
    button {
      border: 1px solid var(--accent-color);
      background: var(--accent-color);
      color: white;
      border-radius: 10px;
      padding: 0.45rem 0.75rem;
      cursor: pointer;
    }
    button[disabled] { opacity: 0.65; cursor: not-allowed; }
    .secondary {
      background: white;
      color: var(--accent-color);
    }
    .danger {
      border-color: #bf2f2f;
      background: #bf2f2f;
      color: white;
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
    .hero-detail input {
      width: 100%;
      border: 1px solid var(--border-color);
      border-radius: 8px;
      padding: 0.45rem 0.55rem;
      font: inherit;
    }
    .detail-actions {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      margin-top: 0.8rem;
    }
    .state { color: var(--muted-color); }
    .error-box {
      border: 1px solid #e88b8b;
      background: #fff2f2;
      color: #7a1b1b;
      border-radius: 12px;
      padding: 0.75rem;
      margin-top: 0.75rem;
    }
    .error-box p { margin: 0.4rem 0; }
    .success-box {
      border: 1px solid #7fcf97;
      background: #effcf3;
      color: #11592c;
      border-radius: 12px;
      padding: 0.65rem 0.75rem;
      margin-top: 0.5rem;
    }
    @media (max-width: 760px) {
      .layout { grid-template-columns: 1fr; }
      .create-form { grid-template-columns: 1fr; }
    }
  `
})
export class HeroesPageComponent implements OnDestroy {
  private readonly api = inject(HeroesApiService);

  protected readonly heroes = signal<HeroListItemViewModel[]>([]);
  protected readonly selectedHeroId = signal<number | null>(null);
  protected readonly selectedHero = signal<HeroDetailViewModel | null>(null);

  protected readonly listLoading = signal(false);
  protected readonly detailLoading = signal(false);
  protected readonly mutationBusy = signal(false);
  protected readonly listError = signal<AppApiError | null>(null);
  protected readonly detailError = signal<AppApiError | null>(null);
  protected readonly mutationError = signal<AppApiError | null>(null);
  protected readonly mutationSuccess = signal<string | null>(null);

  protected searchTerm = '';
  protected newHeroName = '';
  protected editHeroName = '';

  private searchDebounceHandle: number | null = null;

  constructor() {
    this.loadHeroes();
  }

  ngOnDestroy(): void {
    this.clearSearchDebounce();
  }

  protected loadHeroes(preferredSelectedId?: number | null): void {
    this.listLoading.set(true);
    this.listError.set(null);

    this.api.getHeroes(this.searchTerm).subscribe({
      next: (heroes) => {
        this.heroes.set(heroes);
        this.listLoading.set(false);

        if (heroes.length === 0) {
          this.selectedHeroId.set(null);
          this.selectedHero.set(null);
          this.editHeroName = '';
          return;
        }

        const candidateId =
          preferredSelectedId ??
          (heroes.some((h) => h.id === this.selectedHeroId()) ? this.selectedHeroId() : heroes[0]?.id ?? null);

        if (candidateId != null && heroes.some((h) => h.id === candidateId)) {
          this.selectHero(candidateId);
          return;
        }

        this.selectedHeroId.set(null);
        this.selectedHero.set(null);
      },
      error: (error) => {
        this.heroes.set([]);
        this.selectedHeroId.set(null);
        this.selectedHero.set(null);
        this.editHeroName = '';
        this.listError.set(normalizeApiError(error));
        this.listLoading.set(false);
      }
    });
  }

  protected search(): void {
    this.clearSearchDebounce();
    this.loadHeroes();
  }

  protected onSearchInputChanged(): void {
    this.clearSearchDebounce();
    this.searchDebounceHandle = window.setTimeout(() => {
      this.searchDebounceHandle = null;
      this.loadHeroes();
    }, 350);
  }

  protected clearSearch(): void {
    this.clearSearchDebounce();
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
        this.editHeroName = hero.name;
        this.detailLoading.set(false);
      },
      error: (error) => {
        this.selectedHero.set(null);
        this.editHeroName = '';
        this.detailError.set(normalizeApiError(error));
        this.detailLoading.set(false);
      }
    });
  }

  protected createHero(): void {
    const name = this.newHeroName.trim();
    if (!name) {
      return;
    }

    this.beginMutation();
    this.api.createHero({ name }).subscribe({
      next: (hero) => {
        this.newHeroName = '';
        this.mutationSuccess.set(`Hero creato: #${hero.id} ${hero.name}`);
        this.endMutation();
        this.loadHeroes(hero.id);
      },
      error: (error) => {
        this.mutationError.set(normalizeApiError(error));
        this.endMutation();
      }
    });
  }

  protected saveSelectedHero(): void {
    const hero = this.selectedHero();
    if (!hero) {
      return;
    }

    const name = this.editHeroName.trim();
    if (!name) {
      return;
    }

    this.beginMutation();
    this.api.updateHero(hero.id, { id: hero.id, name }).subscribe({
      next: (updated) => {
        this.mutationSuccess.set(`Hero aggiornato: #${updated.id} ${updated.name}`);
        this.selectedHero.set(updated);
        this.editHeroName = updated.name;
        this.endMutation();
        this.loadHeroes(updated.id);
      },
      error: (error) => {
        this.mutationError.set(normalizeApiError(error));
        this.endMutation();
      }
    });
  }

  protected deleteSelectedHero(): void {
    const hero = this.selectedHero();
    if (!hero) {
      return;
    }

    this.beginMutation();
    this.api.deleteHero(hero.id).subscribe({
      next: (deleted) => {
        this.mutationSuccess.set(`Hero eliminato: #${deleted.id} ${deleted.name}`);
        this.selectedHero.set(null);
        this.selectedHeroId.set(null);
        this.editHeroName = '';
        this.endMutation();
        this.loadHeroes();
      },
      error: (error) => {
        this.mutationError.set(normalizeApiError(error));
        this.endMutation();
      }
    });
  }

  protected resetEditName(): void {
    const hero = this.selectedHero();
    this.editHeroName = hero?.name ?? '';
  }

  private beginMutation(): void {
    this.mutationBusy.set(true);
    this.mutationError.set(null);
    this.mutationSuccess.set(null);
  }

  private endMutation(): void {
    this.mutationBusy.set(false);
  }

  private clearSearchDebounce(): void {
    if (this.searchDebounceHandle != null) {
      window.clearTimeout(this.searchDebounceHandle);
      this.searchDebounceHandle = null;
    }
  }
}
