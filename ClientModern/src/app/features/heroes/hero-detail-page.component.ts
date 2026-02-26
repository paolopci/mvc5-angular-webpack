import { CommonModule, Location } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AppApiError } from '../../core/api/api-contracts';
import { normalizeApiError } from '../../core/interceptors/problem-details.interceptor';
import { HeroesApiService } from './heroes-api.service';
import { HeroesHubNavComponent } from './heroes-hub-nav.component';
import { HeroDetailViewModel } from './heroes.models';

@Component({
  selector: 'sg-hero-detail-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, HeroesHubNavComponent],
  template: `
    <section class="card">
      <header class="card__header">
        <p class="eyebrow">Module2 Parity Route</p>
        <h2>Hero Detail</h2>
      </header>

      <p class="hint">Route dedicata di dettaglio con flow <code>save/back</code> in stile legacy.</p>

      <sg-heroes-hub-nav />

      @if (loading()) {
        <p class="state">Caricamento hero...</p>
      } @else if (error()) {
        <div class="error-box" role="alert">
          <strong>{{ error()!.title }}</strong>
          <p>{{ error()!.detail }}</p>
          <div class="actions">
            <button type="button" (click)="load()">Riprova</button>
            <a routerLink="/heroes">Torna alla lista</a>
          </div>
        </div>
      } @else if (hero()) {
        <form class="detail-card" (ngSubmit)="save()" novalidate>
          <h3>{{ hero()!.name | uppercase }} Details</h3>

          <dl>
            <div>
              <dt>id</dt>
              <dd>{{ hero()!.id }}</dd>
            </div>
            <div>
              <dt>name</dt>
              <dd>
                <input
                  type="text"
                  [(ngModel)]="editName"
                  name="editName"
                  autocomplete="off"
                  [disabled]="saving()"
                  (keydown.escape)="resetName()"
                />
              </dd>
            </div>
          </dl>

          @if (saveSuccess()) {
            <p class="success-box" role="status">{{ saveSuccess() }}</p>
          }
          @if (saveError()) {
            <div class="error-box" role="alert">
              <strong>{{ saveError()!.title }}</strong>
              <p>{{ saveError()!.detail }}</p>
            </div>
          }

          <div class="actions">
            <button type="submit" [disabled]="saving() || !editName.trim()">Save</button>
            <button type="button" class="secondary" (click)="goBack()" [disabled]="saving()">Back</button>
            <button type="button" class="secondary" (click)="resetName()" [disabled]="saving()">Reset</button>
          </div>
        </form>
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
    .detail-card {
      border: 1px solid var(--border-color);
      background: var(--surface-2);
      border-radius: 14px;
      padding: 1rem;
    }
    .detail-card h3 { margin-top: 0; }
    dl { margin: 0; display: grid; gap: 0.75rem; }
    dl > div {
      border: 1px solid var(--border-color);
      border-radius: 10px;
      background: white;
      padding: 0.65rem;
    }
    dt { color: var(--muted-color); font-size: 0.85rem; text-transform: lowercase; }
    dd { margin: 0.3rem 0 0; font-weight: 600; }
    input {
      width: 100%;
      border: 1px solid var(--border-color);
      border-radius: 8px;
      padding: 0.45rem 0.55rem;
      font: inherit;
    }
    .actions { margin-top: 0.8rem; display: flex; flex-wrap: wrap; gap: 0.5rem; align-items: center; }
    button {
      border: 1px solid var(--accent-color);
      background: var(--accent-color);
      color: white;
      border-radius: 10px;
      padding: 0.45rem 0.75rem;
      cursor: pointer;
    }
    .secondary { background: white; color: var(--accent-color); }
    a {
      text-decoration: none;
      color: var(--accent-color);
      border: 1px solid var(--border-color);
      border-radius: 10px;
      padding: 0.35rem 0.65rem;
      background: white;
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
      margin-top: 0.75rem;
    }
  `
})
export class HeroDetailPageComponent {
  private readonly api = inject(HeroesApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly location = inject(Location);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal<AppApiError | null>(null);
  protected readonly saveError = signal<AppApiError | null>(null);
  protected readonly saveSuccess = signal<string | null>(null);
  protected readonly hero = signal<HeroDetailViewModel | null>(null);

  protected editName = '';

  constructor() {
    this.load();
  }

  protected load(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(id) || id <= 0) {
      this.error.set({
        status: 400,
        title: 'Invalid route id',
        detail: 'Hero id in route must be a positive number.'
      });
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.saveError.set(null);
    this.saveSuccess.set(null);

    this.api.getHeroById(id).subscribe({
      next: (hero) => {
        this.hero.set(hero);
        this.editName = hero.name;
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(normalizeApiError(error));
        this.hero.set(null);
        this.loading.set(false);
      }
    });
  }

  protected save(): void {
    const current = this.hero();
    const name = this.editName.trim();
    if (!current || !name) {
      return;
    }

    this.saving.set(true);
    this.saveError.set(null);
    this.saveSuccess.set(null);

    this.api.updateHero(current.id, { id: current.id, name }).subscribe({
      next: (updated) => {
        this.hero.set(updated);
        this.editName = updated.name;
        this.saveSuccess.set(`Hero salvato: #${updated.id} ${updated.name}`);
        this.saving.set(false);
      },
      error: (error) => {
        this.saveError.set(normalizeApiError(error));
        this.saving.set(false);
      }
    });
  }

  protected resetName(): void {
    this.editName = this.hero()?.name ?? '';
    this.saveError.set(null);
    this.saveSuccess.set(null);
  }

  protected goBack(): void {
    if (window.history.length > 1) {
      this.location.back();
      return;
    }

    void this.router.navigateByUrl('/heroes/dashboard');
  }
}
