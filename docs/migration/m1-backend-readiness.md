# M1.E - Backend Readiness Baseline (WebCore9.Api)

## Scopo

Definire una baseline operativa ripetibile per il backend moderno (`WebCore9.Api`) al termine di `M1`:

- comandi canonici (`build`, `run`, `test`)
- prerequisiti runtime/SDK
- smoke test manuali standard
- gate CI minimi (documentati)
- criteri per dichiarare il backend "ready for client integration"

## Ambito

Questa baseline copre solo il backend moderno:

- `Server/WebCore9/WebCore9.Api`
- `Server/WebCore9/WebCore9.Api.IntegrationTests`

Non copre:
- client legacy Angular/Webpack (`Client/`)
- UI MVC5 (`Server/WebApplication`)
- pipeline CI completa (in M1 e' documentata, non ancora implementata nel repo)

## 1) Prerequisiti Runtime / SDK

## 1.1 Prerequisiti minimi

- `.NET SDK` con supporto a `net9.0` (minimo: una SDK 9.x installata)
- ambiente locale in grado di eseguire `dotnet build`, `dotnet run`, `dotnet test`

## 1.2 Toolchain rilevata (ambiente locale verificato)

SDK installate rilevate:

- `8.0.124`
- `8.0.418`
- `9.0.311`
- `10.0.103`

Decisione M1:
- supportato qualsiasi ambiente con SDK `9.x` o superiore che possa compilare/eseguire `net9.0`

Nota pratica:
- in ambiente locale puo' verificarsi lock temporaneo file durante `dotnet test` (processo `VBCSCompiler`)
- workaround non-breaking usato in validazione M1:
  - `-p:UseSharedCompilation=false`

## 2) Comandi Canonici Backend (M1)

Eseguire dal root repository (`d:\test\angular\port\mvc5-angular-webpack`).

## 2.1 Build

```bash
dotnet build Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj -nologo
```

## 2.2 Run (sviluppo locale)

```bash
dotnet run --project Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj
```

Output atteso (porte indicative, lette da `launchSettings.json`):
- `https://localhost:7248`
- `http://localhost:5021`

## 2.3 Test integrazione (canonico CI/local)

Comando standard:

```bash
dotnet test Server/WebCore9/WebCore9.Api.IntegrationTests/WebCore9.Api.IntegrationTests.csproj -nologo
```

Comando consigliato per ambienti locali con lock `VBCSCompiler`:

```bash
dotnet test Server/WebCore9/WebCore9.Api.IntegrationTests/WebCore9.Api.IntegrationTests.csproj -nologo -p:UseSharedCompilation=false
```

## 3) Smoke Test Manuale Standard (M1)

## 3.1 Avvio

1. Avviare `WebCore9.Api` (`dotnet run` o VS Code `".NET 9 Launch WebCore9.Api"`).
2. Verificare log di avvio:
   - `Application started`
   - `Hosting environment: Development`
   - `Now listening on: ...`

## 3.2 Endpoint smoke obbligatori

### `GET /api/health`

URL esempio:
- `https://localhost:7248/api/health`

Verifiche minime:
- `200 OK`
- JSON con:
  - `success = true`
  - `data.status = "Healthy"`
  - `data.service = "WebCore9.Api"`
  - `data.environment` valorizzato

### `GET /api/home`

URL esempio:
- `https://localhost:7248/api/home`

Verifiche minime:
- `200 OK`
- JSON con wrapper `ApiResponse`
- `data.defaultModule = "module1"`
- `data.availableModules` contiene `module1`, `module2`

## 3.3 Smoke test contratti errore (consigliato M1)

### `GET /api/home/modules/invalid`
- atteso `404`
- `ProblemDetails` con `status = 404`

### `GET /api/home/modules/loader`
- atteso `409`
- `ProblemDetails` con `status = 409`

## 4) Gate CI Minimi (documentati in M1)

## 4.1 Obiettivo gate M1

Bloccare regressioni evidenti del backend moderno prima dell'integrazione client.

## 4.2 Gate minimi richiesti

1. Restore + build backend moderno
2. Test integrazione `WebCore9.Api.IntegrationTests`
3. (Opzionale in M1, consigliato) smoke HTTP automatizzato su `/api/health` e `/api/home`

## 4.3 Sequenza canonica gate (script/pipeline)

```bash
dotnet build Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj -nologo
dotnet test Server/WebCore9/WebCore9.Api.IntegrationTests/WebCore9.Api.IntegrationTests.csproj -nologo -p:UseSharedCompilation=false
```

## 4.4 Esempio job CI (documentativo, non ancora versionato)

```yaml
steps:
  - checkout
  - setup-dotnet: 9.x
  - run: dotnet build Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj -nologo
  - run: dotnet test Server/WebCore9/WebCore9.Api.IntegrationTests/WebCore9.Api.IntegrationTests.csproj -nologo
```

Nota:
- se il runner CI non mostra il problema di lock, omettere `UseSharedCompilation=false`
- se il lock si presenta anche in CI, aggiungere il flag come mitigazione temporanea

## 5) Baseline M1 - "Backend Ready for Client Integration"

## 5.1 Criteri di ingresso (soddisfatti)

- Contratti M1 documentati (`docs/migration/backend-api-contracts-m1.md`)
- Matrice parita' backend disponibile (`docs/migration/backend-parity-matrix.md`)
- Endpoint M1 prioritari presenti e compatibili (`/api/health`, `/api/home*`)
- Test integrazione estesi (success + error path `400/404/409/500`)

## 5.2 Evidenze M1 (verificate)

- `dotnet build` backend moderno: OK
- `dotnet test` integrazione: OK
- suite integrazione aggiornata: `18/18` test passati
- smoke manuale `/api/health`: verificato

## 5.3 Stato baseline

Stato M1.E:
- `Backend ready for client integration` = **SÌ (baseline M1)**

Con riserve note (accettate in M1):
- CI non ancora implementata nel repository (solo gate documentati)
- `catch (Exception)` generici su endpoint `loader*` (coperti da test, raffinabili post-M1)
- assenza di `global.json` per pinning SDK (da valutare in fase Build/CI successiva)

## 6) Prossimi passi dopo M1.E

Transizione naturale verso `M2`:
- `M2.A` analisi client legacy
- `M2.B` specifica tecnica client moderno separato
- `M2.C` bootstrap SPA moderna + health page su `WebCore9.Api`

