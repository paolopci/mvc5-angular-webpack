# M3.A - Feature Slice Priority, Mapping e Rollback (Stato Aggiornato)

## Scopo

Documentare l'avanzamento della migrazione per slice dopo `M2`, includendo:

- priorita' feature (business/tecnica)
- mapping `legacy -> migrato`
- fallback/rollback per la slice implementata
- gap backend residui emersi

## Stato di partenza (prima di M3.A)

Gia' migrato:
- `M2.C`: Health/Status page (`ClientModern` -> `/api/health`)
- `M2.D`: Home Modules Catalog (`ClientModern` -> `/api/home`, `/api/home/modules`)

Obiettivo `M3.A` in questo ciclo:
- implementare la **prima slice successiva** dopo `M2`:
  - `Heroes` (read/search/detail) con backend reale `WebCore9.Api`

## 1) Ordinamento Feature per Priorita' (business/tecnica)

Legenda:
- `P1` alta priorita'
- `P2` media priorita'
- `P3` bassa priorita'/posticipabile

| Ordine | Feature legacy | Priorita' | Valore | Complessita' | Stato |
| --- | --- | --- | --- | --- | --- |
| 1 | Health/Status (nuova capability di verifica) | `P1` | Alta per bootstrap/diagnostica | Bassa | `Migrata (M2.C)` |
| 2 | Home Modules Catalog (metadata moduli) | `P1` | Alta per discovery e integrazione API | Bassa-Media | `Migrata (M2.D)` |
| 3 | Heroes - read/search/detail | `P1` | Alta (feature UI concreta) | Media | `Migrata (M3.A - questo step)` |
| 4 | Heroes - mutation (add/update/delete) | `P2` | Media | Media-Alta | `Da pianificare` |
| 5 | Hero search UX avanzata (debounce/live search) | `P2` | Media | Media | `Da pianificare` |
| 6 | UI parity completa Module1/Module2 | `P2` | Alta | Alta | `Parziale / in corso` |
| 7 | Sostituzione hosting MVC5 per flussi migrati | `P1` | Alta | Alta | `Da pianificare in rollout` |

## 2) Mapping Aggiornato “legacy / migrato”

| Area/feature | Legacy path | Modern path | Stato | Note |
| --- | --- | --- | --- | --- |
| Health status | n/a (non feature UI legacy equivalente) | `ClientModern /health` + `GET /api/health` | Migrato | Slice tecnica bootstrap/API |
| Home modules catalog | Metadata impliciti in MVC5 + `Home` | `ClientModern /modules` + `/api/home*` | Migrato | UI moderna informativa |
| Heroes list/detail/search | `angularModule-2` + mock `in-memory` (`api/heroes`) | `ClientModern /heroes` + backend reale `/api/heroes` | Migrato (read-only slice) | Backend reale introdotto in `WebCore9.Api` |
| Heroes add/update/delete | `angularModule-2` mock in-memory | Non ancora migrato | Da fare | CRUD server-side non ancora implementato |
| Module1 UI finale | MVC5 view + script bundle | Non ancora migrata come UI feature completa | Da fare | Esiste solo metadata/catalog |
| Module2 UI finale (full parity) | MVC5 + Angular4 Tour of Heroes | Parziale (`/heroes` slice moderna) | In corso | Mancano mutation e parity UI completa |

## 3) Slice Implementata in M3.A: Heroes (read/search/detail)

## 3.1 Scope implementato

### Backend (`WebCore9.Api`)
- `GET /api/heroes`
- `GET /api/heroes?id={id}`
- `GET /api/heroes?name={term}`
- `GET /api/heroes/{id}`

### ClientModern
- route `/heroes`
- lista heroes
- ricerca per nome (submit esplicito)
- dettaglio hero selezionato
- error/loading state

## 3.2 Scope non incluso (esplicitamente)

- `POST /api/heroes`
- `PUT /api/heroes`
- `DELETE /api/heroes/{id}`
- parity UX completa del modulo legacy (messages/search debounce/router detail dedicated)

## 4) Fallback Temporaneo e Rollback (per slice Heroes)

## 4.1 Fallback operativo durante transizione

In caso di problemi sulla slice `ClientModern /heroes`:

- continuare a usare il flusso legacy `Module2` in MVC5 (`Server/WebApplication/Views/Home/Module2.cshtml`)
- il modulo legacy usa `HttpClientInMemoryWebApiModule`, quindi non dipende dagli endpoint reali `WebCore9.Api /api/heroes`

Vantaggio:
- il fallback legacy e' indipendente dal nuovo backend `HeroesController`

## 4.2 Rollback tecnico (slice moderna)

Rollback minimo (non distruttivo):
1. Disabilitare/ignorare route `ClientModern /heroes` (o non usarla)
2. Lasciare `ClientModern /health` e `/modules` operative
3. Mantenere `HeroesController` backend deployato se non crea regressioni (read-only e isolato)

Rollback piu' stretto (se necessario):
1. Rimuovere route `/heroes` da `ClientModern`
2. Rimuovere feature files `ClientModern/src/app/features/heroes/*`
3. (Opzionale) rimuovere `HeroesController` e servizi backend correlati

Decisione consigliata:
- preferire rollback UI-only nella maggior parte dei casi, perche' il backend `Heroes` read-only e' isolato e a basso rischio

## 4.3 Criteri di rollback della slice

Eseguire rollback della slice `/heroes` se:
- errori runtime frequenti lato client su `/heroes`
- regressioni contrattuali backend `api/heroes` che impattano altre feature
- incompatibilita' performance/usabilita' non accettabili nel pilot

## 5) Gap Contrattuali Residui Lato Backend (emersi dopo M3.A)

Chiusi in questo step:
- assenza endpoint reali `api/heroes` per avviare una migrazione oltre il mock client-side

Residui:
- CRUD `heroes` non implementato (`POST/PUT/DELETE`)
- persistence reale assente (attualmente dataset in-memory server-side)
- policy contratti `heroes` da formalizzare come M1-style spec se la feature diventa core
- test error-path `400` per `GET /api/heroes/{id}` (id <= 0) non prioritario ma aggiungibile

## 6) Evidenze di Validazione (M3.A)

Backend:
- `dotnet build` ✅
- `dotnet test` (`WebCore9.Api.IntegrationTests`) ✅
- suite aggiornata: `23/23` test passati

ClientModern:
- `npm run build` ✅
- verifica runtime `/heroes` ✅
- verifica search `tor` -> `Tornado` ✅
- verifica detail `#20 Tornado` ✅

## 7) Prossimo step suggerito dopo M3.A

Per proseguire su `M3.A`/`M3.B`:

1. `Heroes` mutation slice (`POST/PUT/DELETE`) con test integrazione
2. CI duale (legacy + backend modern + client modern build)
3. Criteri rollout/rollback formalizzati per feature migrate

