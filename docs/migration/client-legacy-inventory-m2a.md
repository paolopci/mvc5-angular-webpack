# M2.A - Analisi Client Legacy (Angular 4 + Webpack 2)

## Scopo

Documentare lo stato reale del client legacy (`Client/`) per preparare:

- `M2.B` (strategia tecnica client moderno separato)
- `M2.C` (bootstrap SPA moderna)
- `M2.D` (feature pilota end-to-end)

## Sorgenti Analizzate

- `Client/package.json`
- `Client/tsconfig.json`
- `Client/webpack.config.js`
- `Client/webpack-html-plugin.config.js`
- `Client/modules/common/bootstrap.ts`
- `Client/modules/vendor.ts`
- `Client/modules/polyfill.ts`
- `Client/modules/angularModule-1/*`
- `Client/modules/angularModule-2/*`
- `Server/WebApplication/Views/Home/Module1.cshtml`
- `Server/WebApplication/Views/Home/Module2.cshtml`
- `Server/WebApplication/Views/Home/loader.cshtml`

## 1) Inventario Moduli ed Entrypoint

## 1.1 Entrypoint Webpack (legacy)

Definiti in `Client/webpack.config.js`:

- `polyfills` -> `Client/modules/polyfill.ts`
- `vendors` -> `Client/modules/vendor.ts`
- `module1` -> `Client/modules/angularModule-1/main.ts`
- `module2` -> `Client/modules/angularModule-2/main.ts`

Definiti in `Client/webpack-html-plugin.config.js` (variante html-plugin):

- `polyfills`
- `vendors`
- `module1` (solo `module1`, **non** `module2`)

## 1.2 Moduli Angular legacy

### `angularModule-1`

Caratteristiche:
- modulo Angular minimale
- nessun routing
- nessuna chiamata HTTP
- funzione principalmente dimostrativa/presentazionale

Entrypoint:
- `Client/modules/angularModule-1/main.ts`

Bootstrap target UI:
- selector root: `my-angular-app`
- view legacy: `Server/WebApplication/Views/Home/Module1.cshtml`

### `angularModule-2`

Caratteristiche:
- modulo "Tour of Heroes" con routing (`useHash: true`)
- servizi, componenti multipli, ricerca, dashboard, dettaglio
- usa `HttpClient` ma con backend mock in-memory

Entrypoint:
- `Client/modules/angularModule-2/main.ts`

Bootstrap target UI:
- selector root: `tour-of-heroes`
- view legacy: `Server/WebApplication/Views/Home/Module2.cshtml`

## 1.3 Bootstrap condiviso

File: `Client/modules/common/bootstrap.ts`

Osservazioni:
- funzione `boostrap(...)` (typo nel nome, legacy)
- usa `DOMContentLoaded` e `platformBrowserDynamic().bootstrapModule(...)`
- chiama `enableProdMode()` sempre (anche in locale)

Impatto migrazione:
- il bootstrap del client moderno non deve riutilizzare questo pattern direttamente
- da sostituire con bootstrap standard del framework target (Angular moderno)

## 2) Dipendenze npm e Librerie Bloccanti (Angular 4 / Webpack 2)

## 2.1 Versioni principali rilevate

- `@angular/*`: `4.0.3`
- `typescript`: `^2.1.5`
- `rxjs`: `^5.0.1`
- `webpack`: `2.7.0`
- `webpack-dev-server`: `^1.16.2`
- `html-webpack-plugin`: `^2.15.0`
- `awesome-typescript-loader`: `^3.2.1`
- `angular2-template-loader`: `^0.4.0`
- `tslint`: `^5.0.0`

## 2.2 Dipendenze/asset legacy-coupled o obsolete (bloccanti per upgrade diretto)

Bloccanti tecnici principali:

- Angular 4 + RxJS 5 (API/deprecazioni molto distanti dal target Angular 21)
- Webpack 2 + plugin legacy (`CommonsChunkPlugin`, `uglifyjs-webpack-plugin` old generation)
- `awesome-typescript-loader` (legacy)
- `angular2-template-loader` (legacy)
- `TSLint` + `codelyzer` (ecosistema deprecato)
- `core-js` import legacy (`es6`, `es7/reflect`)
- `@angular/http` ancora incluso in `vendor.ts` (deprecato, anche se `module2` usa `HttpClient`)

## 2.3 Compatibilita' toolchain (rischio M2)

Rischio noto:
- stack `Angular 4` + `Webpack 2` puo' essere incompatibile con Node moderni (es. Node 20)

Impatto:
- necessario pin/validare una versione Node legacy per mantenere build legacy durante la transizione

## 3) Mappa Output Build Legacy verso MVC5

## 3.1 Output JS build standard

Da `Client/webpack.config.js`:

- output path: `../Server/WebApplication/Scripts/ng2`
- pattern filename: `[name].[hash].js`
- source map: `[name].[hash].js.map`

Contratto di integrazione legacy:
- le view MVC5 consumano script in `~/Scripts/ng2/`

## 3.2 Generazione view tramite HtmlWebpackPlugin

### `webpack.config.js`

Genera:
- `Server/WebApplication/Views/Home/Index.cshtml`
usando template:
- `Server/WebApplication/Views/Home/loader.cshtml`

### `webpack-html-plugin.config.js`

Genera:
- `Server/WebApplication/Views/Home/Module1.cshtml`
usando template:
- `Server/WebApplication/Views/Home/loader.cshtml`

Osservazione:
- la variante `build:html` e' focalizzata solo su `module1`
- `module2` resta fuori dal path html-plugin specifico

## 3.3 Accoppiamenti forti con MVC5 (da rimuovere nel client moderno)

- output JS scritto direttamente dentro `Server/WebApplication/Scripts/ng2`
- generazione `.cshtml` dentro `Server/WebApplication/Views/Home`
- script references attese da Razor (`polyfills`, `vendors`, `module1`, `module2`)

Decisione per M2:
- il nuovo client moderno deve essere **completamente disaccoppiato** da questi path

## 4) Chiamate HTTP reali usate dal client (incluse mock/in-memory)

## 4.1 `angularModule-1`

- Nessuna chiamata HTTP rilevata
- modulo puramente presentazionale

## 4.2 `angularModule-2`

Servizio: `Client/modules/angularModule-2/hero.service.ts`

Endpoint usati dal servizio:
- `GET api/heroes`
- `GET api/heroes/?id={id}`
- `GET api/heroes/{id}`
- `GET api/heroes/?name={term}`
- `POST api/heroes`
- `DELETE api/heroes/{id}`
- `PUT api/heroes`

### Stato reale del backend per queste chiamate

Nel modulo e' attivo:
- `HttpClientInMemoryWebApiModule.forRoot(InMemoryDataService, ...)`

Quindi:
- tutte le chiamate `api/heroes` sono intercettate **client-side**
- non usano un backend server reale (`MVC5` o `WebCore9.Api`)

## 4.3 Contratti dati mock attuali (`heroes`)

Shape record mock:

```json
{
  "id": 11,
  "name": "Mr. Nice"
}
```

Impatto M2:
- `api/heroes` e' un buon candidato per feature pilota se si vuole introdurre un endpoint reale in `WebCore9.Api`
- ma non e' richiesto per chiudere M1 backend-first

## 5) Routing e Feature Inventory (modulo 2)

Route rilevate in `angularModule-2/app-routing.module.ts`:

- `""` -> redirect `/dashboard`
- `dashboard`
- `detail/:id`
- `heroes`

Feature UI principali:
- dashboard heroes
- lista heroes
- dettaglio hero
- ricerca hero
- messaggi applicativi

Impatto M2/M3:
- `angularModule-2` e' il modulo con maggiore valore per una migrazione feature-driven
- richiede backend reale se si rimuove l'in-memory API

## 6) Valutazione "Vertical Slice" Pilota (M2.D)

## 6.1 Candidati considerati

### Candidato A - `Health / Status` page (nuovo client -> `/api/health`)

Pro:
- backend gia' pronto (`/api/health`)
- contratto stabile e testato
- basso rischio
- ottimo per validare bootstrap SPA, environment, HTTP client, error handling

Contro:
- basso valore funzionale lato business
- non esercita routing/CRUD complessi

### Candidato B - `Home modules catalog` (nuovo client -> `/api/home`, `/api/home/modules`)

Pro:
- backend gia' pronto e testato
- esercita DTO reali del backend moderno (`ApiResponse<T>`)
- utile per creare una landing/page informativa del nuovo client
- rischio medio-basso

Contro:
- ancora metadata-driven, non una feature "business" ricca

### Candidato C - `Heroes` (sostituzione in-memory con backend reale)

Pro:
- alto valore dimostrativo end-to-end (CRUD/search/list/detail)
- esercita routing, form, ricerca, error handling

Contro:
- richiede nuovi endpoint reali backend + persistenza/mock server-side
- scope ampio per primo slice
- rischio piu' alto per M2 iniziale

## 6.2 Raccomandazione M2.D (feature pilota)

Raccomandazione:
- **Pilota primario M2.D = Candidato B (`Home modules catalog`)**
- **Pilota tecnico iniziale M2.C = Candidato A (`Health / Status`)**

Motivazione:
- `Health` e' ideale per validare bootstrap SPA + integrazione API
- `Home modules catalog` e' il primo slice funzionale a basso rischio che usa i contratti gia' stabilizzati in M1
- `Heroes` va pianificato come slice successivo (`M3.A` o M2 avanzato) dopo consolidamento client moderno

## 7) Blocchi/Gap da Portare in M2.B

- Decisione struttura cartella/workspace del nuovo client moderno (separato da `Client/`)
- Strategia adapter per consumare `ApiResponse<T>` e `ProblemDetails`
- Policy routing moderna (hash vs path-based)
- CORS/config backend per SPA separata
- Strategy per coesistenza legacy/moderno durante transizione
- Pinning Node legacy per mantenere build `Client/` operativa

## 8) Esito M2.A

M2.A e' completato quando:
- inventory client legacy e accoppiamenti sono documentati
- dipendenze bloccanti sono esplicitate
- output build verso MVC5 e' mappato
- le chiamate HTTP reali/mock sono classificate
- la feature pilota raccomandata per `M2.D` e' definita

