import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'sg-heroes-hub-nav',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <nav class="heroes-hub-nav" aria-label="Heroes module navigation">
      <a routerLink="/heroes/dashboard" routerLinkActive="is-active">Dashboard</a>
      <a routerLink="/heroes" [routerLinkActiveOptions]="{ exact: true }" routerLinkActive="is-active">
        Heroes CRUD
      </a>
      <a routerLink="/heroes/search" routerLinkActive="is-active">Search</a>
    </nav>
  `,
  styles: `
    .heroes-hub-nav {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      margin: 0.75rem 0 1rem;
    }
    .heroes-hub-nav a {
      text-decoration: none;
      color: var(--text-color);
      border: 1px solid var(--border-color);
      border-radius: 999px;
      padding: 0.35rem 0.7rem;
      background: white;
      font-size: 0.9rem;
    }
    .heroes-hub-nav a.is-active {
      background: color-mix(in srgb, var(--accent-color) 12%, white);
      border-color: var(--accent-color);
    }
  `
})
export class HeroesHubNavComponent {}
