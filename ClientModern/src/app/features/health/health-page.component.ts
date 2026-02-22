import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { HealthApiService } from './health-api.service';
import { normalizeApiError } from '../../core/interceptors/problem-details.interceptor';
import { AppApiError } from '../../core/api/api-contracts';
import { HealthStatusViewModel } from './health.models';

@Component({
  selector: 'sg-health-page',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="card">
      <header class="card__header">
        <p class="eyebrow">Health / Status</p>
        <h2>WebCore9 API Connectivity</h2>
      </header>

      <p class="hint">
        Verifica il bootstrap del client moderno e la connessione a
        <code>/api/health</code>.
      </p>

      @if (loading()) {
        <p class="state">Caricamento stato API...</p>
      } @else if (error()) {
        <div class="error-box" role="alert">
          <strong>{{ error()!.title }}</strong>
          <p>{{ error()!.detail }}</p>
          <p class="error-box__meta">HTTP status: {{ error()!.status || 'n/a' }}</p>
          <button type="button" (click)="reload()">Riprova</button>
        </div>
      } @else if (health()) {
        <dl class="health-grid">
          <div>
            <dt>Status</dt>
            <dd>{{ health()!.status }}</dd>
          </div>
          <div>
            <dt>Service</dt>
            <dd>{{ health()!.service }}</dd>
          </div>
          <div>
            <dt>Environment</dt>
            <dd>{{ health()!.environment }}</dd>
          </div>
          <div>
            <dt>UTC Timestamp</dt>
            <dd>{{ health()!.utcTimestamp }}</dd>
          </div>
        </dl>
        <button type="button" (click)="reload()">Aggiorna</button>
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

    .health-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 0.75rem;
      margin: 1rem 0;
    }

    .health-grid > div {
      background: var(--surface-2);
      border-radius: 12px;
      border: 1px solid var(--border-color);
      padding: 0.75rem;
    }

    dt {
      color: var(--muted-color);
      font-size: 0.85rem;
    }

    dd {
      margin: 0.35rem 0 0;
      font-weight: 600;
      word-break: break-word;
    }

    button {
      border: 1px solid var(--accent-color);
      background: var(--accent-color);
      color: white;
      border-radius: 10px;
      padding: 0.5rem 0.8rem;
      cursor: pointer;
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
  `
})
export class HealthPageComponent {
  private readonly healthApi = inject(HealthApiService);

  protected readonly loading = signal(false);
  protected readonly health = signal<HealthStatusViewModel | null>(null);
  protected readonly error = signal<AppApiError | null>(null);

  constructor() {
    this.reload();
  }

  protected reload(): void {
    this.loading.set(true);
    this.error.set(null);

    this.healthApi.getHealthStatus().subscribe({
      next: (status) => {
        this.health.set(status);
        this.loading.set(false);
      },
      error: (error) => {
        this.health.set(null);
        this.error.set(normalizeApiError(error));
        this.loading.set(false);
      }
    });
  }
}
