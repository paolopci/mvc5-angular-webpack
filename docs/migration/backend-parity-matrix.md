# M1.A - Backend Parity Matrix (Legacy MVC5 -> WebCore9 API)

## Scopo

Documentare la parita' tra le azioni legacy (`ASP.NET MVC 5`) e gli endpoint moderni (`ASP.NET Core .NET 9`) per guidare `M1` (backend first, compatibilita' alta).

## Sorgenti Analizzate

- `Server/WebApplication/Controllers/HomeController.cs`
- `Server/WebApplication/App_Start/RouteConfig.cs`
- `Server/WebApplication/Views/Home/Module1.cshtml`
- `Server/WebApplication/Views/Home/Module2.cshtml`
- `Server/WebApplication/Views/Home/loader.cshtml`
- `Server/WebCore9/WebCore9.Api/Controllers/HomeController.cs`
- `Server/WebCore9/WebCore9.Api/Controllers/HealthController.cs`
- `Server/WebCore9/WebCore9.Api/Controllers/ApiControllerBase.cs`
- `Server/WebCore9/WebCore9.Api/Common/ApiProblemDetailsFactory.cs`
- `Server/WebCore9/WebCore9.Core/Models/ApiResponse.cs`
- `Client/modules/angularModule-2/app.module.ts`
- `Client/modules/angularModule-2/hero.service.ts`
- `Client/modules/angularModule-2/in-memory-data.service.ts`
- `Client/webpack.config.js`

## 1) Inventario Azioni Legacy (MVC5)

### `HomeController` (legacy)

| Azione | Route effettiva (default MVC) | Comportamento | Output |
| --- | --- | --- | --- |
| `Index()` | `GET /Home/Index` (e `/`) | Redirect a `Module1` | Redirect HTTP verso azione MVC |
| `Module1()` | `GET /Home/Module1` | Render view | `Views/Home/Module1.cshtml` |
| `Module2()` | `GET /Home/Module2` | Render view | `Views/Home/Module2.cshtml` |

### Note legacy rilevanti

- `RouteConfig` usa route MVC standard `{controller}/{action}/{id}` con default `Home/Index`.
- Le view `Module1/Module2` montano Angular tramite script statici in `~/Scripts/ng2/*.js`.
- `loader.cshtml` e' un template Webpack HTML plugin usato per generare una view/index con script chunk dinamici.

## 2) Inventario Endpoint Moderni (`WebCore9.Api`)

### `HomeController` (moderno)

Base route: `GET /api/home*`

| Endpoint | Metodo | Scopo | Response successo | Errori espliciti |
| --- | --- | --- | --- | --- |
| `/api/home` | `GET` | Overview home/default module | `ApiResponse<HomeInfoDto>` | n/d |
| `/api/home/modules` | `GET` | Lista moduli | `ApiResponse<IReadOnlyList<ModuleInfoDto>>` | n/d |
| `/api/home/modules/{key}` | `GET` | Dettaglio modulo per chiave | `ApiResponse<ModuleInfoDto>` | `400`, `404`, `409` |
| `/api/home/loader` | `GET` | Metadata loader | `ApiResponse<LoaderInfoDto>` | `500` (catch generico) |
| `/api/home/loader/chunks` | `GET` | Manifest chunk loader | `ApiResponse<LoaderChunkManifestDto>` | `500` |
| `/api/home/loader/html-plugin-config` | `GET` | Metadata html-plugin config | `ApiResponse<LoaderHtmlPluginConfigDto>` | `500` |
| `/api/home/loader/config-diff` | `GET` | Diff config webpack/html-plugin | `ApiResponse<LoaderWebpackConfigDiffDto>` | `500` |

### `HealthController` (moderno)

Base route: `GET /api/health`

| Endpoint | Metodo | Scopo | Response successo | Errori espliciti |
| --- | --- | --- | --- | --- |
| `/api/health` | `GET` | Stato servizio/API | `ApiResponse<HealthStatusDto>` | n/d |

## 3) Matrice di Parita' `legacy -> modern`

Legenda stato:
- `Coperto`: equivalente moderno diretto disponibile
- `Parziale`: metadata/semantica coperta, ma non stessa route/hosting/output
- `Mancante`: nessun equivalente moderno
- `Non necessario (M1)`: non richiesto per M1 backend-first

| Legacy (MVC5) | Ruolo nel sistema legacy | Equivalente moderno | Stato | Note |
| --- | --- | --- | --- | --- |
| `GET /` -> `Home/Index` | Entry route applicazione (redirect) | `GET /api/home` (overview) + client SPA futura | Parziale | Manca route UI/redirect equivalente lato hosting ASP.NET Core |
| `GET /Home/Index` | Redirect a `Module1` | `GET /api/home` (`DefaultModule = module1`) | Parziale | Semantica "default module" presente, non il comportamento di redirect MVC |
| `GET /Home/Module1` | Hosting UI module1 via Razor + script bundle | `GET /api/home/modules/module1` + `/api/home/loader*` (metadata) | Parziale | Moderno espone metadata, non serve ancora UI |
| `GET /Home/Module2` | Hosting UI module2 via Razor + script bundle | `GET /api/home/modules/module2` | Parziale | Manca hosting UI; metadata modulo presenti |
| `Views/Home/loader.cshtml` (template) | Generazione script chunk dinamici | `/api/home/loader`, `/api/home/loader/chunks`, `/api/home/loader/html-plugin-config`, `/api/home/loader/config-diff` | Coperto (metadata) | Copertura API diagnostica/metadata, non rendering Razor |
| Route MVC default `{controller}/{action}` | Routing UI/server-rendered | API routing `/api/*` | Non necessario (M1) | Target finale e' SPA separata + API |

## 4) Contratti Payload/Errori usati dal client legacy (stato reale)

## 4.1 Contratti effettivamente usati dal client legacy oggi

### A. Contratti "UI hosting" (server-side MVC) - usati davvero

Il client legacy dipende principalmente da:

- Route MVC:
  - `/` -> `Home/Index`
  - `/Home/Module1`
  - `/Home/Module2`
- Script bundle statici in Razor:
  - `~/Scripts/ng2/polyfills.js`
  - `~/Scripts/ng2/vendors.js`
  - `~/Scripts/ng2/module1.js`
  - `~/Scripts/ng2/module2.js`
- Template `loader.cshtml` per generazione view con chunk Webpack

Questi sono contratti di integrazione *hosting/build*, non contratti REST JSON.

### B. Contratti HTTP nel client Angular legacy (`angularModule-2`)

`angularModule-2` usa `HeroService` con URL `api/heroes`, ma nel modulo e' attivo:

- `HttpClientInMemoryWebApiModule.forRoot(InMemoryDataService, ...)`

Quindi:
- le chiamate `api/heroes` sono **mock in-memory client-side**
- non dipendono da un endpoint backend reale (`MVC5` o `WebCore9.Api`)

Shape dati usata dal modulo (mock):

```json
{ "id": 11, "name": "Mr. Nice" }
```

### C. Contratti errori usati dal client legacy

- Nessun contratto errori server-side formale rilevato nel client legacy per `api/heroes` (mock in-memory).
- Gestione errori nel `HeroService` e' generica (`catchError`) e non dipende da `ProblemDetails`.

## 4.2 Contratti moderni gia' definiti (per compatibilita' M1)

### Successo

- Wrapper `ApiResponse<T>`:
  - `success: true`
  - `data: <payload>`

### Errori

- `ProblemDetails` con campi principali:
  - `title`
  - `detail`
  - `status`

Codici HTTP gia' emessi esplicitamente dal backend moderno (`HomeController`):
- `400` validation failed
- `404` module key not found
- `409` reserved module key (`loader`)
- `500` internal error sui metadati loader

## 5) Prioritizzazione Endpoint da completare / consolidare in M1 (`WebCore9.Api`)

Nota: molti endpoint M1 sono gia' presenti. La priorita' M1 e' soprattutto **consolidamento contratti + test**, non solo implementazione ex novo.

### Priorita' alta (M1 obbligatoria)

1. `GET /api/home`
   - Motivo: sostituisce semantica `Home/Index` (default module) a livello metadata/API
   - Azione M1: consolidare contratto e test

2. `GET /api/home/modules`
   - Motivo: inventory moduli per client moderno
   - Azione M1: consolidare contratto e test

3. `GET /api/home/modules/{key}`
   - Motivo: lookup modulo + validazioni + codici errore (core compat policy)
   - Azione M1: ampliare test su `400/404/409`

4. `GET /api/health`
   - Motivo: smoke test e readiness baseline backend
   - Azione M1: mantenere contratto stabile e testare in CI

### Priorita' media (M1 consigliata)

5. `GET /api/home/loader`
6. `GET /api/home/loader/chunks`
7. `GET /api/home/loader/html-plugin-config`
8. `GET /api/home/loader/config-diff`

Motivo:
- utili per migrazione/diagnostica dell'accoppiamento legacy Webpack/MVC
- aiutano il mapping tra build legacy e target moderno

Azione M1:
- verificare shape payload
- rafforzare test integrazione e error path (`500`)

### Non in scope M1 (backend-first)

- Endpoint reale `api/heroes` per `angularModule-2`
  - oggi e' mock in-memory client-side
  - potra' diventare feature pilota in `M2/M3` se scelto

## 6) Gap M1 Residui (da chiudere nei blocchi successivi M1.B/M1.C/M1.D)

- Formalizzare policy contratti (tabella endpoint -> success/error shape target)
- Verificare naming/shape DTO rispetto a consumo futuro client moderno
- Estendere test integrazione su `modules/{key}` e `loader*`
- Decidere se mantenere sempre `ApiResponse<T>` o ammettere endpoint health "raw" (default M1: mantenere wrapper)

## 7) Esito M1.A

M1.A considerato completabile quando:
- la presente matrice e' approvata
- `M1` scope backend e priorita' endpoint sono accettati
- il lavoro passa a `M1.B` (specifica contratti API e compatibilita' alta)

