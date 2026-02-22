export interface HomeInfoDto {
  defaultModule: string;
  availableModules: string[];
  legacyController: string;
}

export interface ModuleInfoDto {
  routeKey: string;
  title: string;
  legacyView: string;
  clientBundleName: string;
  scriptFiles: string[];
  rootElementTag: string;
  loadingText: string;
  usesPrebuiltNg2Bundles: boolean;
}

export interface HomeModuleCard {
  key: string;
  title: string;
  rootElementTag: string;
  loadingText: string;
  clientBundleName: string;
  usesLegacyBundles: boolean;
  isDefault: boolean;
}

export interface HomeModulesCatalogViewModel {
  defaultModule: string;
  availableModules: string[];
  legacyController: string;
  modules: HomeModuleCard[];
}
