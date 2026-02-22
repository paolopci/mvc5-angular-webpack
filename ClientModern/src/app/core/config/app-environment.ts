export interface AppEnvironment {
  apiBaseUrl: string;
  environmentName: 'development' | 'test' | 'production';
}

export const appEnvironment: AppEnvironment = {
  apiBaseUrl: 'http://localhost:5021',
  environmentName: 'development'
};
