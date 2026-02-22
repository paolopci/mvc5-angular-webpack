import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { AppApiError } from '../../core/api/api-contracts';
import { normalizeApiError } from '../../core/interceptors/problem-details.interceptor';
import { HomeModulesApiService } from './home-modules-api.service';
import { HomeModulesCatalogViewModel } from './home-modules.models';

@Component({
  selector: 'sg-home-modules-page',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="card">
      <header class="card__header">
        <p class="eyebrow">M2.D Pilot</p>
        <h2>Home Modules Catalog</h2>
      </header>

      <p class="hint">
        Feature pilota end-to-end che usa i contratti <code>/api/home</code> e
        <code>/api/home/modules</code>.
      </p>

      @if (loading()) {
        <p class="state">Caricamento catalogo moduli...</p>
      } @else if (error()) {
        <div class="error-box" role="alert">
          <strong>{{ error()!.title }}</strong>
          <p>{{ error()!.detail }}</p>
          <p class="error-box__meta">HTTP status: {{ error()!.status || 'n/a' }}</p>
          <button type="button" (click)="reload()">Riprova</button>
        </div>
      } @else if (catalog()) {
        <section class="summary-grid" aria-label="Catalog summary">
          <div>
            <dt>Default module</dt>
            <dd>{{ catalog()!.defaultModule }}</dd>
          </div>
          <div>
            <dt>Available modules</dt>
            <dd>{{ catalog()!.availableModules.join(', ') }}</dd>
          </div>
          <div>
            <dt>Legacy controller</dt>
            <dd>{{ catalog()!.legacyController }}</dd>
          </div>
        </section>

        <div class="modules-grid">
          @for (module of catalog()!.modules; track module.key) {
            <article class="module-card" [class.module-card--default]="module.isDefault">
              <header class="module-card__header">
                <h3>{{ module.title }}</h3>
                @if (module.isDefault) {
                  <span class="chip">Default</span>
                }
              </header>

              <dl>
                <div>
                  <dt>Route key</dt>
                  <dd><code>{{ module.key }}</code></dd>
                </div>
                <div>
                  <dt>Bundle</dt>
                  <dd>{{ module.clientBundleName }}</dd>
                </div>
                <div>
                  <dt>Root tag</dt>
                  <dd><code>{{ module.rootElementTag }}</code></dd>
                </div>
                <div>
                  <dt>Loading text</dt>
                  <dd>{{ module.loadingText }}</dd>
                </div>
                <div>
                  <dt>Legacy bundles</dt>
                  <dd>{{ module.usesLegacyBundles ? 'Yes' : 'No' }}</dd>
                </div>
              </dl>
            </article>
          }
        </div>

        <button type="button" (click)="reload()">Aggiorna catalogo</button>
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

    .card__header h2 {
      margin: 0.25rem 0 0;
      font-size: 1.2rem;
    }

    .eyebrow {
      margin: 0;
      font-size: 0.8rem;
      letter-spacing: 0.08em;
      text-transform: uppercase;
      color: var(--muted-color);
    }

    .hint {
      color: var(--muted-color);
    }

    .state {
      margin: 0.75rem 0 0;
    }

    .summary-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 0.75rem;
      margin: 1rem 0;
    }

    .summary-grid > div {
      background: var(--surface-2);
      border-radius: 12px;
      border: 1px solid var(--border-color);
      padding: 0.75rem;
    }

    .summary-grid dt,
    .module-card dt {
      color: var(--muted-color);
      font-size: 0.85rem;
    }

    .summary-grid dd,
    .module-card dd {
      margin: 0.3rem 0 0;
      font-weight: 600;
      word-break: break-word;
    }

    .modules-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
      gap: 0.9rem;
      margin: 1rem 0;
    }

    .module-card {
      border: 1px solid var(--border-color);
      border-radius: 14px;
      background: var(--surface-1);
      padding: 0.85rem;
    }

    .module-card--default {
      border-color: var(--accent-color);
      box-shadow: 0 0 0 1px color-mix(in srgb, var(--accent-color) 50%, white);
    }

    .module-card__header {
      display: flex;
      justify-content: space-between;
      gap: 0.5rem;
      align-items: center;
      margin-bottom: 0.5rem;
    }

    .module-card__header h3 {
      margin: 0;
      font-size: 1rem;
    }

    .module-card dl {
      margin: 0;
      display: grid;
      gap: 0.45rem;
    }

    .chip {
      border-radius: 999px;
      padding: 0.15rem 0.5rem;
      font-size: 0.75rem;
      background: color-mix(in srgb, var(--accent-color) 14%, white);
      color: var(--accent-color);
      border: 1px solid color-mix(in srgb, var(--accent-color) 35%, white);
      font-weight: 700;
    }

    .error-box {
      border: 1px solid #e88b8b;
      background: #fff2f2;
      color: #7a1b1b;
      border-radius: 12px;
      padding: 0.75rem;
    }

    .error-box p {
      margin: 0.4rem 0;
    }

    .error-box__meta {
      font-size: 0.85rem;
      opacity: 0.9;
    }

    button {
      border: 1px solid var(--accent-color);
      background: var(--accent-color);
      color: white;
      border-radius: 10px;
      padding: 0.5rem 0.8rem;
      cursor: pointer;
    }
  `
})
export class HomeModulesPageComponent {
  private readonly service = inject(HomeModulesApiService);

  protected readonly loading = signal(false);
  protected readonly catalog = signal<HomeModulesCatalogViewModel | null>(null);
  protected readonly error = signal<AppApiError | null>(null);

  constructor() {
    this.reload();
  }

  protected reload(): void {
    this.loading.set(true);
    this.error.set(null);

    this.service.getCatalog().subscribe({
      next: (catalog) => {
        this.catalog.set(catalog);
        this.loading.set(false);
      },
      error: (error) => {
        this.catalog.set(null);
        this.error.set(normalizeApiError(error));
        this.loading.set(false);
      }
    });
  }
}
