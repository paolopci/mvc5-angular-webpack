import { Component } from '@angular/core';

@Component({
  selector: 'sg-home-modules-page',
  standalone: true,
  template: `
    <section class="card">
      <p class="eyebrow">M2.D Preview</p>
      <h2>Home Modules Catalog</h2>
      <p>
        Questa route e' il placeholder del prossimo step M2.D. Qui verra' collegata la
        UI ai contratti <code>/api/home</code> e <code>/api/home/modules</code>.
      </p>
      <ul>
        <li>Adapter layer per <code>ApiResponse&lt;T&gt;</code></li>
        <li>View models per metadata modulo</li>
        <li>Loading/error state coerenti con il client moderno</li>
      </ul>
    </section>
  `,
  styles: `
    .card {
      border: 1px solid var(--border-color);
      border-radius: 16px;
      background: var(--surface-1);
      padding: 1rem;
    }

    .eyebrow {
      margin: 0;
      font-size: 0.8rem;
      letter-spacing: 0.08em;
      text-transform: uppercase;
      color: var(--muted-color);
    }

    h2 {
      margin-top: 0.25rem;
    }
  `
})
export class HomeModulesPageComponent {}
