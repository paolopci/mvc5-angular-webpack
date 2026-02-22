export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
}

export interface ProblemDetailsDto {
  title?: string;
  detail?: string;
  status?: number;
  [key: string]: unknown;
}

export interface AppApiError {
  status: number;
  title: string;
  detail: string;
  original?: unknown;
}
