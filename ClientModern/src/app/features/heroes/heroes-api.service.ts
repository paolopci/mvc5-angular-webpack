import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResponse } from '../../core/api/api-contracts';
import { APP_ENVIRONMENT } from '../../core/config/app-environment.token';
import { HeroDetailViewModel, HeroDto, HeroListItemViewModel, HeroMutationPayload } from './heroes.models';

@Injectable({ providedIn: 'root' })
export class HeroesApiService {
  private readonly http = inject(HttpClient);
  private readonly env = inject(APP_ENVIRONMENT);

  getHeroes(searchTerm?: string): Observable<HeroListItemViewModel[]> {
    let params = new HttpParams();
    if (searchTerm && searchTerm.trim()) {
      params = params.set('name', searchTerm.trim());
    }

    return this.http
      .get<ApiResponse<HeroDto[]>>(`${this.env.apiBaseUrl}/api/heroes`, { params })
      .pipe(map((response) => unwrap(response).map(mapHeroListItem)));
  }

  getTopHeroes(limit = 4): Observable<HeroListItemViewModel[]> {
    return this.getHeroes().pipe(map((heroes) => heroes.slice(1, 1 + limit)));
  }

  getHeroById(id: number): Observable<HeroDetailViewModel> {
    return this.http
      .get<ApiResponse<HeroDto>>(`${this.env.apiBaseUrl}/api/heroes/${id}`)
      .pipe(map((response) => mapHeroDetail(unwrap(response))));
  }

  createHero(payload: HeroMutationPayload): Observable<HeroDetailViewModel> {
    return this.http
      .post<ApiResponse<HeroDto>>(`${this.env.apiBaseUrl}/api/heroes`, payload)
      .pipe(map((response) => mapHeroDetail(unwrap(response))));
  }

  updateHero(id: number, payload: HeroMutationPayload): Observable<HeroDetailViewModel> {
    return this.http
      .put<ApiResponse<HeroDto>>(`${this.env.apiBaseUrl}/api/heroes/${id}`, payload)
      .pipe(map((response) => mapHeroDetail(unwrap(response))));
  }

  deleteHero(id: number): Observable<HeroDetailViewModel> {
    return this.http
      .delete<ApiResponse<HeroDto>>(`${this.env.apiBaseUrl}/api/heroes/${id}`)
      .pipe(map((response) => mapHeroDetail(unwrap(response))));
  }
}

function unwrap<T>(response: ApiResponse<T>): T {
  if (!response?.success || response.data == null) {
    throw new Error('Invalid API wrapper returned by heroes endpoint.');
  }

  return response.data;
}

function mapHeroListItem(dto: HeroDto): HeroListItemViewModel {
  return { id: dto.id, name: dto.name };
}

function mapHeroDetail(dto: HeroDto): HeroDetailViewModel {
  return { id: dto.id, name: dto.name };
}
