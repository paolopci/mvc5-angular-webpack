import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AppApiError, ProblemDetailsDto } from '../api/api-contracts';

export const problemDetailsInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: unknown) => {
      const normalized = normalizeApiError(error);
      console.error('[API ERROR]', req.method, req.url, normalized);
      return throwError(() => normalized);
    })
  );
};

// Provide a separate exported function to keep error normalization testable and framework-agnostic.
export function normalizeApiError(error: unknown): AppApiError {
  if (error instanceof HttpErrorResponse) {
    const problem = isProblemDetails(error.error) ? error.error : undefined;
    return {
      status: error.status || 0,
      title: problem?.title ?? (error.status ? `HTTP ${error.status}` : 'Network error'),
      detail:
        problem?.detail ??
        error.message ??
        'An unexpected error occurred while calling the API.',
      original: error
    };
  }

  return {
    status: 0,
    title: 'Unexpected error',
    detail: 'An unexpected client error occurred.',
    original: error
  };
}

function isProblemDetails(value: unknown): value is ProblemDetailsDto {
  return typeof value === 'object' && value !== null;
}
