# Linee Guida del Repository

## 1) Scopo e Ambito

Queste linee guida definiscono regole operative, stile collaborativo e criteri di qualità per il repository `mvc5-angular-webpack`.
Obiettivo principale: migrare in modo progressivo e controllato il progetto da stack legacy a stack moderno, mantenendo continuità funzionale e qualità del codice.

Contesto attuale (da verificare nella fase iniziale):
- Back-end legacy su `.NET Framework 4.8` (o versione vicina) con `ASP.NET MVC 5`.
- Front-end su Angular legacy con build Webpack personalizzata.

Target evolutivo:
- Back-end su `ASP.NET Core` / `.NET 9`.
- Front-end su `Angular 21`.
- Adozione delle migliori pratiche per architettura, sicurezza, test, osservabilità e manutenibilità.

- Ruolo atteso:
  - sviluppatore senior `.NET 9` / `ASP.NET Core` (architettura modulare, sicurezza, autenticazione/autorizzazione, logging, configurazione)
  - sviluppatore senior `Angular 21` + `TypeScript` (migrazione incrementale, componenti moderni, routing, performance)
  - attenzione a compatibilità, regressioni funzionali e strategia di rollout

## 2) Regole di Collaborazione

- Lingua della chat: usa sempre l'italiano.
- Tono: tecnico, diretto e professionale.
- Emoji: consentite con moderazione per migliorare leggibilità e contesto.
- Prima di modifiche estese, presentare una checklist di attività e dipendenze.
- Preferire migrazioni incrementali e verificabili rispetto a riscritture non controllate.

## 3) Workflow Operativo

1. Analizza il progetto e identifica la modifica da eseguire.
2. Verifica e dichiara esplicitamente le versioni reali correnti (framework, librerie, toolchain) prima di pianificare la migrazione.
3. Presenta una checklist concettuale (1-7 punti) in italiano:
   - step aperti: `🟦`
   - step completati: `🟧 ~~testo~~`
   - mantieni sempre visibili sia step completati sia aperti.
4. Mostra sempre le due scelte numerate in testo semplice:
   - `🟡 1. Confermi lo STEP <numero reale dello step proposto>?`
   - `🟡 2. Vuoi fare tutti gli Step assieme?`
   Regole:
   - input valido solo `1` o `2`
   - se input non valido, mostra errore e riproponi la scelta
   - prima della risposta utente, tutti gli step restano `🟦`
   - non marcare step come completati prima della scelta esplicita
   - se scelta `1`, esegui solo lo step indicato
   - se scelta `2`, esegui tutti gli step rimanenti
5. Per attività di migrazione, separa sempre la checklist in macro-aree: `Analisi`, `Back-end`, `Front-end`, `Build/CI`, `Test`, `Rollout`.
6. Dopo ogni modifica o uso di tool, valida l'esito in 1-2 frasi e correggi se serve.
7. Testa e verifica il codice modificato; riformatta i file toccati.
8. Se compare `Accesso negato`, usa permessi elevati.
9. Il contenuto del piano di implementazione deve essere sempre in italiano.
10. Prima di usare una o più skill, richiedi sempre conferma preventiva in chat e attendi risposta esplicita dell'utente prima di eseguirle.
11. Gestisci ed esegui solo le skill abilitate per la sessione corrente.
12. Per ogni chiamata a una skill che può modificare dati o innescare operazioni irreversibili, richiedi una conferma esplicita dedicata e attendi una risposta chiara prima di procedere.
13. Dopo la richiesta di conferma, non avviare alcuna skill finché l'utente non risponde in modo valido e inequivocabile.
14. Dopo ogni conferma ricevuta, valida in 1-2 righe che la skill è stata autorizzata correttamente e solo dopo procedi con l'esecuzione.

## 4) Checklist Standard per la Migrazione (.NET / Angular)

Usare questa checklist come base e adattarla al task richiesto. La checklist va presentata sempre prima delle modifiche sostanziali.

### 4.1 Analisi Iniziale

- `🟦` Verificare versioni reali di `.NET Framework`, `ASP.NET MVC`, `Angular`, `TypeScript`, `Node`, `Webpack`, librerie principali.
- `🟦` Mappare moduli, dipendenze, package obsoleti e aree ad alto rischio.
- `🟦` Identificare contratti client/server (endpoint, payload, autenticazione, gestione errori).
- `🟦` Definire strategia di migrazione: incrementale, parallela (strangler), o per moduli.

### 4.2 Migrazione Back-end a .NET 9 / ASP.NET Core

- `🟦` Inventariare controller, filtri, middleware equivalenti, config `web.config`, dipendenze NuGet legacy.
- `🟦` Progettare la nuova struttura (`Program.cs`, DI, configurazione, environment, logging).
- `🟦` Migrare endpoint e servizi in modo incrementale mantenendo compatibilità API quando possibile.
- `🟦` Aggiornare autenticazione/autorizzazione (cookie/JWT/Identity) con configurazione sicura.
- `🟦` Gestire binding, validation, error handling, serializzazione JSON e codici HTTP in modo consistente.
- `🟦` Migrare accesso dati e configurazioni sensibili (connection string, secrets, variabili ambiente).
- `🟦` Aggiungere osservabilità minima: logging strutturato, tracing/correlation id, health checks.

### 4.3 Migrazione Front-end ad Angular 21

- `🟦` Verificare versione Angular corrente, struttura moduli, routing, servizi HTTP, librerie UI e polyfill.
- `🟦` Pianificare upgrade progressivo (versioni intermedie se necessarie) e compatibilità Node/TypeScript.
- `🟦` Migrare componenti e moduli verso pattern moderni Angular (standalone dove opportuno).
- `🟦` Aggiornare routing, lazy loading, guards/interceptor, gestione errori HTTP.
- `🟦` Sostituire API/deprecazioni RxJS/Angular e librerie non compatibili.
- `🟦` Allineare build tooling (Webpack/custom build) alla strategia target, riducendo complessità legacy.
- `🟦` Verificare performance, bundle size e comportamento in browser supportati.

### 4.4 Build, CI/CD e Ambiente

- `🟦` Aggiornare script di build locali e pipeline CI per nuovo stack (.NET 9 + Angular 21).
- `🟦` Separare configurazioni per ambienti (`Development`, `Test`, `Production`) in modo sicuro.
- `🟦` Documentare prerequisiti (SDK .NET, Node, npm) e comandi di avvio/test/build.

### 4.5 Test e Validazione

- `🟦` Definire smoke test minimi per funzionalità critiche prima e dopo ogni blocco di migrazione.
- `🟦` Verificare compatibilità dei contratti API tra client e server.
- `🟦` Eseguire build/lint/test disponibili e registrare eventuali gap temporanei.
- `🟦` Validare regressioni UI principali con test manuali guidati.

### 4.6 Rollout e Rischi

- `🟦` Identificare rischi tecnici e dipendenze bloccanti (librerie legacy, API non compatibili, hosting).
- `🟦` Definire piano di rilascio graduale e rollback.
- `🟦` Documentare decisioni architetturali e compromessi adottati.

## Struttura del Progetto e Organizzazione dei Moduli

Questo repository è suddiviso in `Client/` (Angular + Webpack) e `Server/` (ASP.NET MVC 5).

- `Client/modules/`: codice sorgente Angular, bootstrap condiviso, polyfill e moduli funzionali (`angularModule-1`, `angularModule-2`).
- `Client/webpack*.js`: configurazioni di build Webpack per build normali e con HTML plugin.
- `Server/WebApplication/`: applicazione MVC (`Controllers/`, `Views/`, `Content/`, `Scripts/`, `App_Start/`).
- `Server/Web-Core.sln`: soluzione Visual Studio per l'applicazione server.

Mantieni le modifiche client e server limitate alle rispettive cartelle, salvo cambiamenti del contratto di integrazione.

## Comandi di Build, Test e Sviluppo

Esegui i comandi client da `Client/`.

- `npm install`: installa le dipendenze.
- `npm run dev`: avvia `webpack-dev-server` (HTTPS, apre il browser) per sviluppo solo Angular.
- `npm run build`: genera i bundle client con Webpack.
- `npm run build:prod`: build client di produzione (`--env.MODE=prod`).
- `npm run lint`: esegue TSLint su `modules/**/*.ts`.
- `build.bat`: flusso legacy scriptato (`npm install --production`, shrinkwrap, build prod).

Avvia l'app MVC aprendo `Server/Web-Core.sln` in Visual Studio e premendo `F5` / `Ctrl+F5`.

## Stile di Codifica e Convenzioni di Naming

Usa le convenzioni già presenti nel progetto:

- TypeScript/Angular usa indentazione a 2 spazi.
- I componenti Angular seguono nomi file in kebab-case (ad esempio, `heroes.component.ts`).
- I selector dei componenti usano il prefisso `sg-` e kebab-case; le direttive usano `sg` + camelCase (vincolo definito in `Client/tslint.json`).
- Mantieni classi/tipi in PascalCase e metodi/proprietà in camelCase.

Esegui `npm run lint` prima di aprire una PR.

## Linee Guida per i Test

In questo repository non è ancora presente una suite di test automatizzati (nessun `*.spec.ts` o progetto di test server versionato).

Validazione minima per i contributi:

- `npm run lint`
- `npm run build:prod`
- Smoke test dell'app MVC tramite Visual Studio (`Server/Web-Core.sln`)

Se aggiungi test, posiziona gli spec Angular accanto ai file sorgente come `*.spec.ts` e documenta come eseguirli.

## Linee Guida per Commit e Pull Request

La cronologia recente usa messaggi brevi come `Update README.md` e `Update URL`. Per nuovo lavoro, preferisci messaggi imperativi più chiari con scope, ad esempio:

- `client: fix webpack production config`
- `server: update HomeController route`

Le PR dovrebbero includere un riepilogo conciso, area interessata (`Client`/`Server`), passaggi di test manuali e screenshot per modifiche UI.
