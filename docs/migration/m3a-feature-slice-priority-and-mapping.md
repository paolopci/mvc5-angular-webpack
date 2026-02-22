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
- implementare e poi estendere la slice `Heroes`:
  - `read/search/detail` (prima fase)
  - `CRUD + parity UX base + telemetry minima` (estensione successiva)

## 1) Ordinamento Feature per Priorita' (business/tecnica)

Legenda:
- `P1` alta priorita'
- `P2` media priorita'
- `P3` bassa priorita'/posticipabile

| Ordine | Feature legacy | Priorita' | Valore | Complessita' | Stato |
| --- | --- | --- | --- | --- | --- |
| 1 | Health/Status (nuova capability di verifica) | `P1` | Alta per bootstrap/diagnostica | Bassa | `Migrata (M2.C)` |
| 2 | Home Modules Catalog (metadata moduli) | `P1` | Alta per discovery e integrazione API | Bassa-Media | `Migrata (M2.D)` |
| 3 | Heroes - read/search/detail | `P1` | Alta (feature UI concreta) | Media | `Migrata (M3.A)` |
| 4 | Heroes - mutation (add/update/delete) | `P2` | Media | Media-Alta | `Migrata (M3.A estensione CRUD)` |
| 5 | Hero search UX avanzata (debounce/live search) | `P2` | Media | Media | `Parziale avanzata (debounce + empty/filter UX)` |
| 6 | UI parity completa Module1/Module2 | `P2` | Alta | Alta | `Parziale avanzata / in corso` |
| 7 | Sostituzione hosting MVC5 per flussi migrati | `P1` | Alta | Alta | `Da pianificare in rollout` |

## 2) Mapping Aggiornato “legacy / migrato”

| Area/feature | Legacy path | Modern path | Stato | Note |
| --- | --- | --- | --- | --- |
| Health status | n/a (non feature UI legacy equivalente) | `ClientModern /health` + `GET /api/health` | Migrato | Slice tecnica bootstrap/API |
| Home modules catalog | Metadata impliciti in MVC5 + `Home` | `ClientModern /modules` + `/api/home*` | Migrato | UI moderna informativa |
| Heroes list/detail/search | `angularModule-2` + mock `in-memory` (`api/heroes`) | `ClientModern /heroes` + backend reale `/api/heroes` | Migrato | Include search debounce base e detail |
| Heroes add/update/delete | `angularModule-2` mock in-memory | `ClientModern /heroes` + `POST/PUT/DELETE /api/heroes*` | Migrato | CRUD reale introdotto in `WebCore9.Api` con storage persistente JSON |
| Module1 UI finale | MVC5 view + script bundle | Non ancora migrata come UI feature completa | Da fare | Esiste solo metadata/catalog |
| Module2 UI finale (full parity) | MVC5 + Angular4 Tour of Heroes | Parziale avanzata (`/heroes*` routes moderne) | In corso | CRUD + dashboard/search/detail route presenti; mancano persistence reale e alcune refinement UX |

## 3) Slice Implementata in M3.A: Heroes (read/search/detail -> CRUD)

## 3.1 Scope implementato

### Backend (`WebCore9.Api`)
- `GET /api/heroes`
- `GET /api/heroes?id={id}`
- `GET /api/heroes?name={term}`
- `GET /api/heroes/{id}`
- `POST /api/heroes`
- `PUT /api/heroes/{id}`
- `DELETE /api/heroes/{id}`
- logging strutturato + metriche minime (`System.Diagnostics.Metrics`) per requests/mutations/errors

### ClientModern
- route `/heroes`
- route `/heroes/dashboard`
- route `/heroes/search`
- route `/heroes/:id` (detail route dedicata)
- lista heroes
- ricerca per nome (submit + debounce 350ms + empty/filter state)
- dettaglio hero selezionato
- mutation flow (`add`, `edit`, `delete`)
- feedback success/error + loading state granulari
- pannello `Messages` (activity log) in stile parity funzionale del modulo legacy
- delete a doppia conferma (UI-only) e focus/keyboard polish (`Esc`, refocus input)
- dashboard `Top Heroes` con quick navigation
- hero search dedicata (live debounce + result links verso detail route)
- flow `save/back` sulla detail route dedicata

## 3.2 Scope non incluso (esplicitamente)

- persistence reale (database) - attuale storage in-memory server-side
- parity UX completa del modulo legacy (router detail separato, dashboard/search route parity, eventuali flow aggiuntivi)
- persistence DB (attuale storage reale = file JSON) e parity completa del comportamento storico di messaggistica/flow multi-view
- export/visualizzazione metriche (strumentazione pronta, exporter/dashboard non configurati)

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
- preferire rollback UI-only nella maggior parte dei casi, perche' il backend `Heroes` resta isolato e a basso rischio (anche con CRUD in-memory)

## 4.3 Criteri di rollback della slice

Eseguire rollback della slice `/heroes` se:
- errori runtime frequenti lato client su `/heroes`
- regressioni contrattuali backend `api/heroes` che impattano altre feature
- incompatibilita' performance/usabilita' non accettabili nel pilot

## 5) Gap Contrattuali Residui Lato Backend (emersi dopo M3.A)

Chiusi in questo step:
- assenza endpoint reali `api/heroes` per avviare una migrazione oltre il mock client-side
- gap CRUD (`POST/PUT/DELETE`) per parity funzionale base `Module2`
- gap test integrazione su error path `400/404/409` per mutation flow

Residui:
- persistence DB assente (attuale storage reale file JSON `App_Data/heroes-store.json`)
- policy contratti `heroes` da formalizzare come M1-style spec se la feature diventa core
- test error-path `400` per `GET /api/heroes/{id}` (id <= 0) non prioritario ma aggiungibile
- no exporter/collector per metriche (solo instrumentation locale nel backend)
- repository/storage `Heroes` non ancora formalizzato con EF/SQLite (attuale file JSON)

## 6) Evidenze di Validazione (M3.A)

Backend:
- `dotnet test` (`WebCore9.Api.IntegrationTests`, `Release`) ✅
- suite aggiornata: `32/32` test passati
- `Heroes` storage reale creato e seedato in `Server/WebCore9/WebCore9.Api/App_Data/heroes-store.json` ✅

ClientModern:
- `npm run build` ✅
- `npm run test -- --watch=false` ✅
- `npm run lint` (typecheck gate) ✅
- verifica runtime `/heroes` ✅
- verifica search/detail (`tor` -> `Tornado`) ✅
- verifica mutation flow (`add/edit/delete`) ✅
- verifica parity UX base (`Messages`, filtro/empty state, delete con doppia conferma, clear messages) ✅
- verifica parity routing `Module2` (`/heroes/dashboard`, `/heroes/search`, `/heroes/:id`) ✅
- verifica flow `detail route` `save/back` ✅

## 7) Prossimo step suggerito dopo M3.A

Per proseguire su `M3.A`/`M3.B`:

1. Evolvere `Heroes` storage da file JSON a DB/repository (es. SQLite/EF Core) se serve scenario multi-instance
2. Telemetry/exporter (OpenTelemetry/metrics sink) per usare davvero i contatori in rollout
3. Rifinire parity `Module2` residuale (dashboard/search UX edge cases) oppure aprire nuova slice `Module1`
