import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';
import { ApiResponse } from '../../core/api/api-contracts';
import { APP_ENVIRONMENT } from '../../core/config/app-environment.token';
import {
  HomeInfoDto,
  HomeModuleCard,
  HomeModulesCatalogViewModel,
  ModuleInfoDto
} from './home-modules.models';

@Injectable({ providedIn: 'root' })
export class HomeModulesApiService {
  private readonly http = inject(HttpClient);
  private readonly env = inject(APP_ENVIRONMENT);

  getCatalog(): Observable<HomeModulesCatalogViewModel> {
    return forkJoin({
      homeInfo: this.http.get<ApiResponse<HomeInfoDto>>(`${this.env.apiBaseUrl}/api/home`),
      modules: this.http.get<ApiResponse<ModuleInfoDto[]>>(`${this.env.apiBaseUrl}/api/home/modules`)
    }).pipe(
      map(({ homeInfo, modules }) => {
        const home = unwrapApiResponse(homeInfo, '/api/home');
        const moduleList = unwrapApiResponse(modules, '/api/home/modules');

        return mapCatalog(home, moduleList);
      })
    );
  }
}

function unwrapApiResponse<T>(response: ApiResponse<T>, endpoint: string): T {
  if (!response?.success || response.data == null) {
    throw new Error(`Invalid API wrapper returned by ${endpoint}.`);
  }

  return response.data;
}

function mapCatalog(homeInfo: HomeInfoDto, modules: ModuleInfoDto[]): HomeModulesCatalogViewModel {
  const cards: HomeModuleCard[] = modules.map((module) => ({
    key: module.routeKey,
    title: module.title,
    rootElementTag: module.rootElementTag,
    loadingText: module.loadingText,
    clientBundleName: module.clientBundleName,
    usesLegacyBundles: module.usesPrebuiltNg2Bundles,
    isDefault: homeInfo.defaultModule === module.routeKey
  }));

  return {
    defaultModule: homeInfo.defaultModule,
    availableModules: homeInfo.availableModules,
    legacyController: homeInfo.legacyController,
    modules: cards
  };
}
