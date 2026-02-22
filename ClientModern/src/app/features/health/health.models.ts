export interface HealthStatusDto {
  status: string;
  service: string;
  environment: string;
  utcTimestamp: string;
}

export interface HealthStatusViewModel {
  status: string;
  service: string;
  environment: string;
  utcTimestamp: string;
}
