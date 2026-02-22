# M2.E - Toolchain e Compatibilita' Client Legacy / ClientModern

## Scopo

Definire una baseline operativa duale per:

- `Client/` (legacy Angular 4 + Webpack 2)
- `ClientModern/` (Angular moderno, SPA separata)

Obiettivi M2.E:
- testare compatibilita' toolchain reale del client legacy
- documentare setup consigliato legacy
- separare comandi legacy vs moderno
- fissare baseline di build client moderno
- formalizzare regole di coesistenza

## 1) Toolchain Verificata (ambiente locale)

Versioni rilevate:
- `node`: `v20.19.6`
- `npm`: `10.8.2`
- `Angular CLI` (globale): `21.1.0`

## 2) Compatibilita' Client Legacy (`Client/`) - Evidenza Reale

## 2.1 Comandi eseguiti

Eseguiti in `Client/`:

- `npm install` ✅
- `npm run build:prod` ✅
- `npm run lint` ❌

## 2.2 Esito compatibilita' con Node 20

### Compatibilita' operativa minima (M2.E)

Con `Node 20.19.6`:
- install dipendenze: funzionante
- build produzione legacy (`webpack 2`): funzionante

Quindi, per questo repository/stato attuale:
- `Node 20` e' **utilizzabile** per mantenere il client legacy durante la transizione (almeno per `install` + `build:prod`)

### Limiti osservati

- `npm run lint` fallisce, ma per **problemi legacy preesistenti** (regole TSLint su selector `sg-*`, whitespace interpolation), non per incompatibilita' Node
- presenti numerosi warning/deprecations/vulnerabilita' npm (attesi su stack legacy)

## 2.3 Posizionamento della compatibilita'

Decisione M2.E:
- classificare `Node 20` come **compatibile pragmaticamente** per la coesistenza M2 (build legacy + sviluppo client moderno)
- mantenere comunque la nota di rischio nel README, perche' la compatibilita' puo' variare su macchine/registry/OS diversi

## 3) Setup Consigliato (Legacy)

## 3.1 Requisiti minimi legacy (pragmatici)

- `Node 20.x` (verificato localmente in questo repo) **oppure**
- una versione Node legacy se emergono problemi specifici su altre macchine

`npm`:
- `10.x` testato con successo per `install`/`build:prod`

## 3.2 Comandi legacy consigliati

Eseguire da `Client/`:

```bash
npm install
npm run build:prod
```

Comandi utili:

```bash
npm run dev
npm run build
npm run build:html
```

Nota lint:
- `npm run lint` al momento non e' un gate affidabile del porting perche' fallisce su issue legacy preesistenti

## 4) Baseline ClientModern (M2.E)

## 4.1 Comandi canonici `ClientModern/`

```bash
npm install
npm start
npm run build
```

## 4.2 Baseline verificata in M2.C/M2.D

- `ClientModern` bootstrap completato ✅
- pagina `/health` collegata a `WebCore9.Api` ✅
- feature pilota `/modules` collegata a `GET /api/home` + `GET /api/home/modules` ✅
- `npm run build` (`ClientModern`) ✅

## 5) Separazione Comandi Legacy vs Modern (regola operativa)

## 5.1 Legacy UI (MVC5 + Angular/Webpack)

Quando usare:
- confronto regressione con UI esistente
- validazione build legacy
- mantenimento flusso attuale non ancora migrato

Comandi:
- `Client/` -> `npm install`, `npm run build:prod` (o `build.bat`)
- `Server/Web-Core.sln` in Visual Studio per avvio UI MVC5

## 5.2 Modern path (SPA + API)

Quando usare:
- sviluppo nuove feature migrate
- test contratti API M1/M2
- lavoro incrementale su client moderno

Comandi:
- backend: `dotnet run --project Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj`
- client: `cd ClientModern && npm start`

## 5.3 Regola anti-confusione

Non mischiare nello stesso task:
- build output legacy (`Client/` -> `Server/WebApplication`)
- build output moderno (`ClientModern/dist`)

## 6) Regole di Coesistenza (M2.E)

1. `Client/` resta sorgente legacy e non va rifattorizzato per ospitare feature nuove.
2. Le nuove feature migrate vanno in `ClientModern/`.
3. `WebCore9.Api` e' backend target per il client moderno.
4. `Server/WebApplication` resta host della UI legacy finche' non si raggiunge parita' sufficiente.
5. Prima di testare feature client moderne:
   - verificare smoke backend (`/api/health`)
   - garantire suite integrazione backend verde (M1 gate)
6. Le regressioni del client legacy si validano separatamente, non bloccano automaticamente il lavoro del client moderno salvo impatti condivisi.

## 7) Gap Residui / Prossimi Passi

- Valutare `global.json` per pinning SDK .NET in Build/CI successivo
- Valutare `.nvmrc` / `.node-version` se il team necessita pinning Node condiviso
- Aggiornare eventuali pipeline CI con job separati:
  - legacy build
  - backend modern tests
  - client modern build

## 8) Esito M2.E

M2.E e' completato quando:
- la compatibilita' del client legacy e' testata con evidenza reale
- setup legacy e moderno sono documentati
- comandi sono separati chiaramente
- la baseline `ClientModern` e' confermata
- le regole di coesistenza sono esplicite

