import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResponse } from '../../core/api/api-contracts';
import { APP_ENVIRONMENT } from '../../core/config/app-environment.token';
import { HealthStatusDto, HealthStatusViewModel } from './health.models';

@Injectable({ providedIn: 'root' })
export class HealthApiService {
  private readonly http = inject(HttpClient);
  private readonly env = inject(APP_ENVIRONMENT);

  getHealthStatus(): Observable<HealthStatusViewModel> {
    return this.http
      .get<ApiResponse<HealthStatusDto>>(`${this.env.apiBaseUrl}/api/health`)
      .pipe(
        map((response) => {
          if (!response.success || !response.data) {
            throw new Error('API response wrapper is invalid.');
          }

          return mapHealthStatusToViewModel(response.data);
        })
      );
  }
}

function mapHealthStatusToViewModel(dto: HealthStatusDto): HealthStatusViewModel {
  return {
    status: dto.status,
    service: dto.service,
    environment: dto.environment,
    utcTimestamp: dto.utcTimestamp
  };
}
