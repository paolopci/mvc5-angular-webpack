# M3.C - Rollout Graduale e Dismissione Legacy (Piano Operativo)

## Scopo

Definire un piano eseguibile per:

- criteri di **feature parity sufficiente**
- rollout graduale del client moderno (`ClientModern`) + backend moderno (`WebCore9.Api`)
- rollback per rilascio/slice
- inventario dipendenze legacy residue
- decommissioning controllato di `Client/` e hosting UI MVC5
- documentazione operativa minima (runbook + architettura target + supporto)

## 1) Criteri di “Feature Parity Sufficiente”

La parita' non richiede una copia pixel-perfect iniziale della UI legacy, ma la copertura dei flussi target con comportamento accettabile.

### 1.1 Criteri generali (gate di parity)

Una feature puo' essere considerata **parzialmente pronta al rollout** quando:

1. Esiste una route/page nel `ClientModern` per il flusso target.
2. Il backend `WebCore9.Api` espone endpoint equivalenti con contratti stabili (wrapper `ApiResponse`, errori coerenti).
3. Sono presenti test automatici backend (integrazione/contratto) per gli endpoint coinvolti.
4. Sono definiti smoke test manuali minimi lato UI.
5. Esiste fallback/rollback documentato verso il flusso legacy.

Una feature puo' essere considerata **parity sufficiente per switch primario** quando, oltre ai punti sopra:

1. I principali scenari happy-path sono coperti e validati.
2. Gli error path critici sono gestiti (es. 400/404/500 con feedback UI).
3. Il team accetta eventuali differenze UX residue come non bloccanti.
4. Le regressioni note sono documentate e tracciate con ownership.

### 1.2 Stato parity per feature note (snapshot corrente)

| Feature | Stato | Parity sufficiente? | Note |
| --- | --- | --- | --- |
| `Health/Status` (`/health`) | Migrata | `Si` (tecnica) | Feature diagnostica, non business |
| `Home Modules Catalog` (`/modules`) | Migrata | `Si` (informativa) | Slice metadata, basso rischio |
| `Heroes` read/search/detail/CRUD (`/heroes`) | Migrata parziale avanzata | `Parziale` | CRUD e feedback base presenti; mancano persistence reale e parity UX completa |
| `Module1` UI finale | Non migrata | `No` | Richiede slice funzionale dedicata |
| `Module2` full parity | In corso | `No` | `/heroes` copre solo una parte del modulo |

## 2) Piano di Rollout Graduale (feature / ambiente / utenza)

Target finale: `ClientModern` (SPA separata) + `WebCore9.Api`, con dismissione progressiva del percorso MVC5 per i flussi migrati.

## 2.1 Fasi di rollout consigliate

### Fase R0 - Internal Dev / Smoke (attuale)

- `WebCore9.Api` avviato localmente (`dotnet run` / VS Code debug)
- `ClientModern` avviato localmente (`npm start`)
- Validazioni:
  - `/health`
  - `/modules`
  - `/heroes`

Obiettivo:
- stabilizzare contratti e UX delle slice migrate

### Fase R1 - Preview interna (team tecnico)

- Rendere il client moderno accessibile in ambiente preview/staging (URL separato)
- Mantenere `WebApplication` MVC5 come percorso principale
- Eseguire smoke test comparativi legacy vs modern per feature migrate

Gate uscita:
- CI modern/legacy verdi
- assenza errori bloccanti su flussi migrati
- rollback testato (UI-only)

### Fase R2 - Rollout per feature (switch selettivo)

- Esporre selettivamente feature migrate nel client moderno (o link dedicato)
- Mantenere fallback esplicito verso flusso legacy per feature non migrate
- Monitorare errori e feedback d’uso

Gate uscita:
- feature migrate con parity sufficiente
- nessuna regressione severa lato API contratti
- metriche/errori minimi osservabili (anche solo log + counters) per le slice attive

### Fase R3 - Client moderno primario (transizione avanzata)

- `ClientModern` diventa percorso raccomandato per flussi target
- MVC5 UI legacy resta disponibile solo per feature residue
- Pianificare freeze su nuove feature nel client legacy

Gate uscita:
- majority feature target migrate
- runbook operativo aggiornato
- piano decommissioning approvato

### Fase R4 - Decommissioning legacy UI

- Rimozione progressiva dipendenze a `Client/` e `Server/WebApplication` per il frontend
- eventuale mantenimento di backend legacy per compatibilita' residuale (se necessario)

Gate uscita:
- zero dipendenze critiche ai bundle `Scripts/ng2`
- smoke e rollback finali verificati

## 2.2 Strategia di exposure (consigliata)

Per ridurre rischio:

- **Routing separato** durante la transizione (URL distinti legacy/modern)
- **Feature-by-feature enablement** invece di switch totale
- **Rollback UI-first** (ritiro della route/page moderna) come default

## 3) Piano di Rollback per Rilascio / Slice

## 3.1 Principi

- Preferire rollback **non distruttivo**.
- Se possibile, mantenere deploy backend moderno anche se si ritira la UI moderna.
- Rollback rapido = disabilitare entrypoint/route moderna della feature.

## 3.2 Playbook rollback per slice (template)

Per ogni slice migrata (es. `/heroes`):

1. Identificare il trigger rollback:
   - errori runtime frequenti
   - regressioni contrattuali API
   - UX/performance non accettabili
2. Applicare rollback UI:
   - rimuovere link/entrypoint alla route moderna
   - indirizzare gli utenti al flusso legacy equivalente
3. Valutare rollback backend:
   - solo se gli endpoint moderni introdotti causano regressioni collaterali
4. Verificare smoke:
   - legacy flow equivalente
   - `/api/health`
5. Registrare incidente/decisione e criteri per retry

## 3.3 Rollback plan per stato corrente (slice note)

| Slice moderna | Fallback legacy | Rollback consigliato |
| --- | --- | --- |
| `/health` | n/a (diagnostica tecnica) | Nascondere route se inutile; backend health resta attivo |
| `/modules` | `Home`/view MVC5 metadata implicita | UI-only rollback, nessun rollback backend necessario |
| `/heroes` | `Module2` legacy (mock in-memory) | UI-only rollback; backend `HeroesController` e metriche possono restare |

## 4) Dipendenze Legacy Residue (Inventario Operativo)

Inventario basato su codice/config reali del repo.

## 4.1 Coupling build client legacy -> MVC5 hosting

### `Client/webpack.config.js` e `Client/webpack-html-plugin.config.js`

Scrivono output direttamente in:

- `../Server/WebApplication/Scripts/ng2`
- `../Server/WebApplication/Views/Home`

Impatto:
- il build client legacy modifica asset e view del progetto MVC5
- complica CI/CD e rollback se si mescolano build legacy e modern nello stesso rilascio

## 4.2 Dipendenze MVC5 a bundle Angular legacy

### `Server/WebApplication/App_Start/BundleConfig.cs`

Bundle MVC puntano a:

- `~/Scripts/ng2/polyfills.js`
- `~/Scripts/ng2/vendors.js`
- `~/Scripts/ng2/module1.js`
- `~/Scripts/ng2/module2.js`

### View MVC5

Dipendenze dirette ai bundle `Scripts/ng2` in:

- `Server/WebApplication/Views/Home/Module1.cshtml`
- `Server/WebApplication/Views/Home/Module2.cshtml`
- `Server/WebApplication/Views/Home/Index.cshtml` (output generato / hashed assets)

## 4.3 Hook di build legacy

- `Client/build.bat`
  - `npm install --production`
  - `npm shrinkwrap`
  - `npm run build:prod`

Impatto:
- puo' aggiornare `Client/npm-shrinkwrap.json`
- puo' rigenerare `Server/WebApplication/Views/Home/Index.cshtml`

## 4.4 Dipendenze funzionali legacy residue (M3.C snapshot)

- `Module1` UI finale: ancora su MVC5 + Angular 4
- `Module2` full feature parity: ancora su MVC5 + Angular 4 (la slice `/heroes` moderna copre read/search/detail/CRUD ma non tutta la UX)

## 5) Piano di Decommissioning `Client/` legacy e UI MVC5 (progressivo)

## 5.1 Preconditions (prima di iniziare decommissioning reale)

Tutte vere:

1. Feature target migrate con parity sufficiente.
2. `ClientModern` usato come percorso primario in ambiente target.
3. Nessuna dipendenza attiva a `Scripts/ng2` per flussi supportati.
4. Rollback documentato e testato per ultime feature migrate.
5. CI modern stabile per almeno una finestra di rilascio concordata.

## 5.2 Sequenza di decommissioning consigliata

### Step D1 - Freeze legacy client

- Bloccare nuove feature in `Client/`
- Consentire solo fix critici
- Aggiornare README/runbook con stato “legacy maintenance only”

### Step D2 - Rimozione references UI per feature migrate

- Rimuovere link/route legacy per feature gia' migrate
- Tenere disponibili solo le view residue necessarie

### Step D3 - Scollegare build pipeline legacy dal percorso principale

- Rendere `ci-legacy.yml` opzionale/manuale o solo su branch manutenzione
- Conservare artefatti/documentazione per rollback storico

### Step D4 - Rimozione accoppiamento `Scripts/ng2`

- Eliminare riferimenti in `BundleConfig.cs` e `Views/Home/*.cshtml` quando non piu' necessari
- Rimuovere output path legacy dai webpack config (se il client legacy resta solo archivio)

### Step D5 - Archivio / rimozione `Client/` legacy

Opzioni:
- archiviare in branch/tag di maintenance
- rimuovere dal trunk principale dopo approvazione

### Step D6 - Dismissione hosting UI MVC5

- `Server/WebApplication` non piu' usato per servire UI target
- mantenimento eventuale solo per compatibilita' temporanea o storica

## 6) Documentazione Operativa Finale (Runbook / Architettura / Supporto)

Questa sezione funge da baseline minima da evolvere in runbook separati se il rollout entra in staging/production.

## 6.1 Runbook Operativo (ibrido, stato corrente)

### Avvio locale - moderno

- Backend: `dotnet run --project Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj`
- Client moderno: `cd ClientModern && npm start`

Smoke minimi:

- `GET /api/health`
- `GET /api/home`
- `http://localhost:4200/health`
- `http://localhost:4200/modules`
- `http://localhost:4200/heroes`
- CRUD smoke `heroes`: create -> update -> delete + fallback legacy `Module2` se necessario

### Avvio locale - legacy (quando necessario)

- `cd Client && npm run build:prod` (o `build.bat`)
- Avvio MVC5 via Visual Studio (`Server/Web-Core.sln`)

### Incident triage rapido

1. Verificare `WebCore9.Api` (`/api/health`)
2. Verificare console/network `ClientModern`
3. Verificare log backend `HeroesController` (warning/error) e contatori metrici se esposti dal runtime
4. Verificare fallback su flow legacy equivalente
5. Se il problema e' solo UI moderna, applicare rollback UI-only

## 6.2 Architettura target (transizione)

### Stato target intermedio (raccomandato)

- `ClientModern` (SPA separata)
- `WebCore9.Api` (backend target)
- MVC5 UI legacy solo per feature residue durante transizione

### Stato target finale

- SPA moderna + backend `WebCore9.Api`
- nessuna dipendenza runtime dai bundle `Scripts/ng2`
- MVC5 UI dismessa (o mantenuta solo come archivio/compatibilita' temporanea)

## 6.3 Supporto Operativo (ownership minima)

Ruoli consigliati:

- `Backend owner`: contratti API, integration test, logging/metriche, rollback backend
- `Frontend owner`: `ClientModern`, UX parity, rollback UI
- `DevEx/CI owner`: workflow CI legacy/modern, prerequisiti toolchain
- `Tech lead`: gate parity, decisioni rollout/decommissioning

Checklist supporto per rilascio slice:

- CI `modern` verde
- CI `legacy` verde (o advisory failure noto/accettato)
- smoke manuale feature migrata
- fallback legacy verificato
- note di rilascio / differenze note documentate

## 7) Prossimi Passi dopo M3.C

1. Completare telemetry export (OpenTelemetry / sink) per usare i contatori `Heroes` in ambienti non-locali
2. Eseguire `M3.A` su slice successive oppure completare parity UX `Module2`
3. Estrarre `runbook` e `architettura` in documenti dedicati quando si entra in staging/production
