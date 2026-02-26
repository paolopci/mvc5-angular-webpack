# M2.D - Feature Pilota End-to-End: Home Modules Catalog (ClientModern + WebCore9.Api)

## Scopo

Documentare l'implementazione della feature pilota `Home Modules Catalog` nel client moderno, usando i contratti backend:

- `GET /api/home`
- `GET /api/home/modules`

## Scope Implementato (M2.D)

### ClientModern

- route `"/modules"` attiva
- chiamate reali a `WebCore9.Api` (non mock)
- adapter/service con unwrap `ApiResponse<T>`
- mapping DTO backend -> view model UI
- UI minima con:
  - loading state
  - error state (`ProblemDetails` via interceptor)
  - riepilogo overview (`defaultModule`, `availableModules`, `legacyController`)
  - cards dei moduli (`module1`, `module2`)

### Backend

- nessuna nuova route necessaria
- contratti M1 riusati senza cambi breaking

## File Principali (ClientModern)

- `ClientModern/src/app/features/home-modules/home-modules-page.component.ts`
- `ClientModern/src/app/features/home-modules/home-modules-api.service.ts`
- `ClientModern/src/app/features/home-modules/home-modules.models.ts`

Dipendenze di supporto:
- `ClientModern/src/app/core/api/api-contracts.ts`
- `ClientModern/src/app/core/interceptors/problem-details.interceptor.ts`

## Differenze Rispetto al Legacy

## 1) Origine dati

Legacy:
- la UI principale (`Module1`/`Module2`) e' montata via MVC5 Razor
- metadata/loader legati a view/scripts server-side

Pilota M2.D:
- UI SPA separata (`ClientModern`)
- dati letti da API REST `WebCore9.Api`

## 2) Contratti di integrazione

Legacy:
- forte dipendenza da:
  - `Views/Home/*.cshtml`
  - `Scripts/ng2/*.js`
  - output Webpack dentro `Server/WebApplication`

Pilota M2.D:
- dipendenza solo da endpoint HTTP:
  - `/api/home`
  - `/api/home/modules`

## 3) Gestione errori

Legacy:
- non c'e' una gestione centralizzata `ProblemDetails` per il modulo pilota

Pilota M2.D:
- `HttpInterceptor` centralizzato per normalizzare errori API
- UI con error state e retry

## 4) Modello dati UI

Legacy:
- il concetto di "modulo" e' implicito nelle view e negli script bundle

Pilota M2.D:
- i moduli sono rappresentati come view model espliciti (`HomeModuleCard`)
- la UI nasconde la complessita' del wrapper `ApiResponse<T>`

## Allineamenti Backend Residui (emersi: nessun blocco)

- Nessun gap contrattuale bloccante emerso per `/api/home` e `/api/home/modules`
- I contratti M1 sono risultati sufficienti per il pilota

## Validazione M2.D (attesa/eseguita)

Validazioni richieste:
- build backend `WebCore9.Api`
- build `ClientModern`
- avvio in parallelo backend + SPA
- verifica UI route `/modules`
- verifica rendering cards moduli (`module1`, `module2`)
- verifica fallback errore (simulabile fermando backend)

## Prossimi passi dopo M2.D

- M2.E: toolchain duale (legacy/client moderno) e regole di coesistenza
- M3.A: migrazione feature successive (es. `Heroes`) con backend reale

