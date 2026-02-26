# M2.B - Strategia Tecnica Client Moderno (SPA separata + WebCore9.Api)

## Scopo

Definire una specifica tecnica `decision-complete` per introdurre un client moderno separato che consumi `WebCore9.Api`, senza sostituire immediatamente il client legacy.

Questa strategia guida:
- `M2.C` (bootstrap SPA moderna)
- `M2.D` (feature pilota end-to-end)
- coesistenza legacy/moderno durante la transizione

## Vincoli e Decisioni Gia' Confermate

- Strategia migrazione: incrementale
- `M1`: backend-first (gia' completato)
- Target finale UI hosting: **SPA separata + API**
- Compatibilita' API: **alta**
- Client legacy (`Client/`) deve restare operativo durante M2

## Stato Attuale Rilevato (Backend e Client)

### Backend (`WebCore9.Api`)
- API `.NET 9` attiva con endpoint `api/health`, `api/home*`
- Nessuna configurazione CORS presente in `Program.cs`
- `launchSettings.json` espone:
  - `https://localhost:7248`
  - `http://localhost:5021`

### Client legacy (`Client/`)
- Angular `4.0.3` + Webpack `2.7.0`
- output e HTML generation accoppiati a `Server/WebApplication`
- `angularModule-2` usa `HttpClient`, ma con `in-memory` backend client-side

## 1) Struttura del Nuovo Client Separato (workspace/cartella target)

## 1.1 Decisione (M2)

Nuovo client moderno in cartella separata:

- `ClientModern/`

Motivazione:
- separazione netta dal client legacy (`Client/`)
- nessun rischio di rompere la build legacy durante bootstrap iniziale
- naming semplice e chiaro nel repo ibrido

## 1.2 Struttura target minima (M2.C/M2.D)

```text
ClientModern/
  package.json
  angular.json               (o config equivalente del tool scelto)
  tsconfig*.json
  src/
    main.ts
    index.html
    app/
      core/
        api/
        config/
        interceptors/
      shared/
      features/
        health/
        home-modules/
      app.routes.ts
      app.component.ts
```

Regole:
- niente riferimenti a `Server/WebApplication/*`
- niente output in cartelle server-side
- build output locale del client moderno resta in `ClientModern/dist` (o equivalente)

## 1.3 Scelta tooling (default M2)

Default:
- Angular moderno con toolchain ufficiale (`Angular CLI`) per bootstrap e build

Nota:
- la creazione effettiva del progetto avverra' in `M2.C`
- in `M2.B` blocchiamo la strategia, non il comando di bootstrap

## 2) Policy Environment (`baseUrl` API, dev/prod)

## 2.1 Contratto configurazione client moderno

Il client moderno deve consumare `WebCore9.Api` tramite configurazione di ambiente, mai con URL hardcoded nei servizi.

Configurazione minima richiesta:
- `apiBaseUrl`
- `environmentName` (opzionale ma utile per debug/log UI)

## 2.2 Valori target (M2)

### Development (default locale)
- `apiBaseUrl = https://localhost:7248`

Fallback compatibile:
- `http://localhost:5021` se HTTPS non disponibile

### Production/Test (placeholder M2)
- `apiBaseUrl` definito per environment di deploy/pipeline

Regola:
- il client non deve assumere porte fisse fuori da `Development`

## 2.3 Uso pratico della config

I servizi/adapter devono usare una singola fonte di verita' (es. `ApiConfigService` o provider config centralizzato) invece di concatenare stringhe sparse.

## 3) Routing, Error Handling HTTP, Interceptor, Auth Placeholder

## 3.1 Routing (decisione M2)

Decisione:
- routing **path-based** (no `hash`) per il client moderno

Motivazione:
- target SPA moderna separata
- allineamento a pratiche moderne
- `useHash: true` resta solo nel client legacy

Nota rollout:
- se l'hosting finale/temporaneo non supporta SPA fallback, si puo' usare una fallback strategy lato host/reverse proxy (non hash routing)

## 3.2 Error handling HTTP (baseline M2)

Pattern richiesto:
- `HttpInterceptor` globale per:
  - gestione errori rete / 5xx
  - parsing `ProblemDetails`
  - logging client-side minimo
- UI feature-level per:
  - stati `loading`
  - messaggi errore leggibili
  - retry manuale dove sensato

Regole:
- non interpretare direttamente errori raw nei componenti
- centralizzare la normalizzazione errori in `core/api` o interceptor

## 3.3 Interceptor (set minimo)

M2 baseline:
1. `ApiBaseUrl` non via interceptor (preferire service/config)
2. `ProblemDetails/Error interceptor` (obbligatorio)
3. `Correlation/headers interceptor` (placeholder, opzionale M2)

## 3.4 Auth placeholder (decisione M2)

Stato:
- nessun requisito auth/autz implementato nel backend moderno attuale

Decisione:
- predisporre solo uno `auth placeholder` nel client (`core/auth/` o equivalente)
- non introdurre auth reale in M2 salvo richiesta esplicita

## 4) Strategia Adapter/Service per Compatibilita' Payload Backend

## 4.1 Obiettivo

Isolare il client moderno dai dettagli del backend M1:
- wrapper `ApiResponse<T>`
- errori `ProblemDetails`
- DTO con campi legacy-coupled/transitori

## 4.2 Pattern deciso (M2)

Strati:
1. `Api client layer` (HTTP raw)
2. `Adapter layer` (unwrap `ApiResponse<T>`, mapping errori)
3. `Feature service` (consumo feature-driven)
4. `UI component` (solo view-state e interazioni)

## 4.3 Regole adapter (vincolanti)

- `ApiResponse<T>` viene unwrapped **nel layer adapter**, non nei componenti
- `ProblemDetails` viene convertito in un modello errore client coerente (es. `AppApiError`)
- I DTO backend M1 non devono essere usati direttamente in tutta la UI senza un livello di mapping nei punti di confine (soprattutto per `ModuleInfoDto`)

## 4.4 Strategia per endpoint M1

### `/api/health`
- adapter semplice: `ApiResponse<HealthStatusDto>` -> `HealthStatusViewModel`

### `/api/home` e `/api/home/modules`
- adapter dedicato:
  - mappa `HomeInfoDto` e `ModuleInfoDto` in modelli UI
  - nasconde campi legacy-coupled non necessari alla UI moderna

Decisione M2:
- creare modelli UI espliciti (`HomeModuleCard`, `HomeOverviewState`, ecc.) anziche' usare direttamente i DTO backend

## 5) Strategia di Coesistenza con Client Legacy (nessuna sostituzione immediata)

## 5.1 Principio

Durante M2:
- `Client/` (legacy) resta il percorso UI esistente
- `ClientModern/` e' percorso sperimentale/parallelizzato
- `WebCore9.Api` e' backend target del client moderno

## 5.2 Regole di coesistenza

- non modificare output path/build del client legacy in M2.B/M2.C
- non rimuovere view MVC5 (`Module1`, `Module2`, `loader`) durante M2
- documentare chiaramente i comandi di avvio separati (legacy vs moderno)
- mantenere smoke backend M1 come gate minimo prima di testare il client moderno

## 5.3 Sequenza operativa raccomandata (M2)

1. Avvia `WebCore9.Api`
2. Avvia `ClientModern/` in dev mode
3. Testa `Health page`
4. Testa `Home modules catalog`
5. Mantieni il client legacy disponibile per confronto/regressione manuale

## 6) Requisiti Backend di Supporto al Client Moderno (da implementare in M2.C/M2.D)

## 6.1 CORS (necessario)

Stato attuale:
- nessuna policy CORS configurata nel backend

Decisione M2:
- aggiungere policy CORS `Development` per consentire il dev server del client moderno

Policy minima (M2.C):
- `AllowOrigins`: origin del dev server SPA (es. `http://localhost:4200` o porta effettiva)
- `AllowAnyHeader`
- `AllowAnyMethod`
- nessun wildcard indiscriminato in produzione

## 6.2 Nessuna dipendenza da MVC5

Il backend moderno deve rimanere indipendente dal client legacy:
- no rendering Razor per la SPA moderna
- no output static files del nuovo client dentro `Server/WebApplication`

## 7) Feature Pilota M2.D (decisione operativa)

Decisione basata su M2.A:
- `M2.C` (pilota tecnico): `Health / Status` page (`/api/health`)
- `M2.D` (pilota funzionale): `Home modules catalog` (`/api/home`, `/api/home/modules`)

Scope esplicito `M2.D`:
- lista moduli (`module1`, `module2`)
- metadata base (`title`, `routeKey`, stato disponibile)
- UI informativa / navigazione base, non ancora migrazione UI completa di `Module1/Module2`

## 8) Deliverable Attesi per M2.C/M2.D (derivati da questa strategia)

## 8.1 M2.C
- bootstrap `ClientModern/`
- environment config con `apiBaseUrl`
- shell + routing base
- pagina `Health / Status`
- interceptor errori base
- avvio locale in parallelo con `WebCore9.Api`

## 8.2 M2.D
- feature `Home modules catalog`
- adapter/services per `/api/home*`
- UI minima con gestione errori/loading
- documentazione differenze rispetto a legacy

## 9) Rischi M2 e Mitigazioni

### Rischio: CORS dimenticato / blocking in dev
- Mitigazione: M2.C deve includere esplicitamente la policy CORS development nel backend

### Rischio: uso diretto dei DTO backend nella UI moderna
- Mitigazione: imporre adapter + view models nei feature services

### Rischio: confusione tra comandi legacy e moderni
- Mitigazione: documentazione separata e naming esplicito (`Client/` vs `ClientModern/`)

### Rischio: drift contratti backend durante M2
- Mitigazione: mantenere test integrazione M1 come gate prima del lavoro client

## 10) Esito M2.B

M2.B e' completato quando:
- la struttura target del client moderno e' decisa
- le policy environment/routing/error handling sono definite
- la strategia adapter e' bloccata
- la coesistenza legacy/moderno e' formalizzata
- il lavoro puo' passare a `M2.C` senza decisioni architetturali aperte

