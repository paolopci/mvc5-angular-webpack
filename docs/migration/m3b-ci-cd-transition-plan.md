# M3.B - CI/CD Unificata per Transizione (Legacy + Modern)

## Obiettivo

Introdurre una pipeline transitoria che separi chiaramente i controlli `legacy` e `modern`, mantenendo i gate necessari fino al decommissioning progressivo dello stack MVC5 + Angular 4.

## Stato Implementato (Repo)

Workflow aggiunti:

- `.github/workflows/ci-modern.yml`
- `.github/workflows/ci-legacy.yml`

### CI Modern (`ci-modern.yml`)

Job principali:

- `backend-webcore9`
  - `dotnet restore`
  - `dotnet build` (`WebCore9.Api`)
  - `dotnet test` (`WebCore9.Api.IntegrationTests`)
- `client-modern`
  - `npm ci` in `ClientModern/`
  - `npm run lint` (gate transitorio = `typecheck`)
  - `npm run build`
  - `npm run test -- --watch=false`
  - smoke dist: verifica `dist/ClientModern/browser/index.html`
  - copre anche la regressione base della shell aggiornata (`/heroes` CRUD UI)
- `smoke-api`
  - avvia `WebCore9.Api` su HTTP locale (`127.0.0.1:5021`)
  - smoke `GET /api/health`
  - smoke `GET /api/home`

### CI Legacy (`ci-legacy.yml`)

Job principali:

- `client-legacy-build`
  - `npm install` in `Client/`
  - `npm run build:prod`
  - smoke file generato `Server/WebApplication/Views/Home/Index.cshtml`
- `client-legacy-lint-advisory` (`continue-on-error: true`)
  - `npm run lint`
  - mantenuto come indicatore diagnostico durante la transizione (sono noti errori legacy)

## Gate Minimi per Area (M3.B)

### Modern

- Back-end moderno: `build + integration tests` (bloccanti)
- Client moderno:
  - `lint` (transitorio: `typecheck`) ✅ bloccante
  - `build` ✅ bloccante
  - `test` ✅ bloccante
- Smoke API: `/api/health`, `/api/home` ✅ bloccante
- API `Heroes` CRUD coperta dai test integrazione backend ✅ bloccante (tramite `WebCore9.Api.IntegrationTests`)
- Smoke client: dist output `index.html` ✅ bloccante

### Legacy (transizione)

- Build client legacy `build:prod` ✅ bloccante (finché UI legacy è supportata)
- Lint client legacy ⚠️ advisory (`continue-on-error`) fino a bonifica sorgenti
- Nessun job automatico per avvio MVC5 completo (richiede stack Visual Studio/hosting più pesante): mantenuto smoke manuale/documentato

## Branch Scope

I workflow sono configurati su:

- `push` e `pull_request` verso `main`
- `push` e `pull_request` verso `master`
- `workflow_dispatch` (esecuzione manuale)

## Criteri per Rimozione Graduale Pipeline Legacy

La pipeline `ci-legacy.yml` può essere ridotta/rimossa progressivamente quando TUTTI i criteri seguenti sono soddisfatti:

1. Le feature UI prioritarie sono migrate in `ClientModern` e validate (`M3.A`).
2. Il client legacy non è più richiesto per i flussi target in ambienti attivi.
3. Gli endpoint backend usati dal client legacy hanno equivalenti moderni con test di contratto/regressione.
4. Esiste un piano di rollback che non dipende dalla ricostruzione on-demand del bundle legacy.
5. La documentazione operativa (`README`/runbook) punta al client moderno come percorso primario.

## Riduzione Rischio / Note Operative

- `ClientModern` non aveva un gate lint nativo; in questa fase è stato introdotto un gate statico transitorio (`typecheck`) esposto come `npm run lint`.
- L'estensione `Heroes` CRUD non richiede cambi CI aggiuntivi: rientra gia' nei gate `backend-webcore9` (integration tests) e `client-modern` (build/test).
- Il job legacy usa `windows-latest` per allinearsi meglio alla validazione locale già eseguita (Node 20 + Webpack 2).
- Il test backend locale può fallire se `WebCore9.Api` è in esecuzione in debug (file lock); in CI il job parte da ambiente pulito.
