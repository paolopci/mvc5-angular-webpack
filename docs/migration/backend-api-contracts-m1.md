# M1.B - Specifica Contratti API e Compatibilita' Alta (WebCore9.Api)

## Scopo

Formalizzare i contratti API M1 del backend moderno (`WebCore9.Api`) con requisito di compatibilita' alta durante la migrazione:

- shape risposta di successo
- shape errori
- codici HTTP target
- coerenza controller attuali con la policy
- gap DTO/campi da monitorare
- policy di evoluzione contratti

## Ambito M1

Endpoint inclusi:

- `GET /api/health`
- `GET /api/home`
- `GET /api/home/modules`
- `GET /api/home/modules/{key}`
- `GET /api/home/loader`
- `GET /api/home/loader/chunks`
- `GET /api/home/loader/html-plugin-config`
- `GET /api/home/loader/config-diff`

Endpoint esclusi da M1 (backend-first):

- `api/heroes` reale (nel client legacy e' mock `in-memory`)

## 1) Shape di successo (policy M1)

## 1.1 Contratto canonico di successo

Per M1, **tutti gli endpoint inclusi** devono restituire successo usando il wrapper:

```json
{
  "success": true,
  "data": { }
}
```

Base type:
- `ApiResponse<T>`

Regole:
- `success` deve essere sempre `true` su `200 OK`
- `data` non deve essere `null` per gli endpoint M1 correnti (salvo futura eccezione documentata)
- non introdurre campi aggiuntivi nel wrapper in M1 senza versioning/decisione esplicita

## 1.2 Naming JSON (policy)

I DTO sono definiti in C# con proprietà `PascalCase` (`DefaultModule`, `RouteKey`, ecc.).
Con ASP.NET Core Web defaults (nessuna customizzazione rilevata), la serializzazione JSON espone proprietà `camelCase`.

Esempi attesi:
- `DefaultModule` -> `defaultModule`
- `UtcTimestamp` -> `utcTimestamp`
- `RouteKey` -> `routeKey`

Decisione M1:
- mantenere `camelCase` come output JSON canonico
- evitare override locali di naming policy sui controller

## 2) Shape errori (policy M1)

Per M1, gli errori API usano `ProblemDetails` (non wrappato in `ApiResponse<T>`).

Shape minima attesa:

```json
{
  "title": "Validation failed",
  "detail": "Module key is required.",
  "status": 400
}
```

Campi minimi obbligatori in M1:
- `title`
- `detail`
- `status`

Regole:
- `status` nel payload deve corrispondere al codice HTTP restituito
- mantenere messaggi stabili dove già coperti da test (in particolare `Module key not found`, `Reserved module key`)
- non mischiare wrapper `ApiResponse<T>` con `ProblemDetails` nello stesso branch di errore

## 3) Codici HTTP target per casi M1

## 3.1 Tabella target per endpoint

| Endpoint | 200 | 400 | 404 | 409 | 500 | Note M1 |
| --- | --- | --- | --- | --- | --- | --- |
| `GET /api/health` | Sì | No | No | No | Non previsto | Smoke/readiness |
| `GET /api/home` | Sì | No | No | No | Non previsto | Metadata overview |
| `GET /api/home/modules` | Sì | No | No | No | Non previsto | Lista moduli |
| `GET /api/home/modules/{key}` | Sì | Sì | Sì | Sì | No | Validazione + conflitto chiave riservata |
| `GET /api/home/loader` | Sì | No | No | No | Sì | `try/catch` -> internal error |
| `GET /api/home/loader/chunks` | Sì | No | No | No | Sì | `try/catch` -> internal error |
| `GET /api/home/loader/html-plugin-config` | Sì | No | No | No | Sì | `try/catch` -> internal error |
| `GET /api/home/loader/config-diff` | Sì | No | No | No | Sì | `try/catch` -> internal error |

## 3.2 Policy generale HTTP

- `200 OK`: solo con payload valido in `ApiResponse<T>`
- `400 BadRequest`: input semanticamente non valido (es. chiave con `.` o whitespace)
- `404 NotFound`: risorsa/modulo non supportato
- `409 Conflict`: input valido formalmente ma semanticamente in conflitto con chiave riservata (`loader`)
- `500 InternalServerError`: errori interni lato loader metadata, con `ProblemDetails`

## 4) Verifica coerenza controller attuali vs policy (stato attuale)

## 4.1 `HealthController`

Esito: `Coerente` con policy M1

- `GET /api/health` restituisce `ApiResponse<HealthStatusDto>` via `ApiOk(...)`
- OpenAPI metadata presente (`[ProducesApiHealthResponse]`)
- Nessuna divergenza nota su shape successo

## 4.2 `HomeController` overview/modules

Esito: `Coerente` con policy M1 (consolidare via test)

- `GET /api/home` e `GET /api/home/modules` -> `ApiOk(...)`
- `GET /api/home/modules/{key}` gestisce:
  - `400` (`ValidationFailed`)
  - `404` (`ModuleKeyNotFound`)
  - `409` (`Conflict` chiave riservata)
- OpenAPI conventions presenti per `GetModuleByKey`

## 4.3 `HomeController` loader endpoints

Esito: `Coerente ma con gap di robustezza/test`

- endpoint `loader*` wrappano il successo in `ApiResponse<T>`
- errori interni catturati come `500` con `ProblemDetails`
- criticita' M1:
  - i `catch (Exception)` sono generici (accettabile in M1, da raffinare oltre M1)
  - copertura test del ramo `500` non ancora verificata nei test attuali

## 5) Gap di naming/campi DTO (rispetto a uso reale e uso futuro client)

## 5.1 Gap rispetto al client legacy reale

Stato:
- il client legacy **non consuma** ancora questi DTO JSON per i flussi principali UI
- dipende soprattutto da route/view/script MVC5

Impatto:
- i DTO moderni sono liberi da vincoli legacy stretti lato UI corrente
- ma diventano contratti target per il client moderno (M2), quindi vanno stabilizzati ora

## 5.2 Gap/attenzioni DTO per il client moderno (M2-ready)

### `HomeInfoDto`
- `LegacyController` e' utile per diagnosi/mapping legacy ma potrebbe essere metadata transitorio
- `AvailableModules` + `DefaultModule` sono campi chiave da stabilizzare per bootstrap client moderno

Decisione M1:
- mantenere `LegacyController` (non rimuovere in M1)
- trattare `DefaultModule` e `AvailableModules` come contratti stabili

### `ModuleInfoDto`
- contiene campi fortemente legacy-coupled:
  - `LegacyView`
  - `ScriptFiles`
  - `UsesPrebuiltNg2Bundles`
- contiene anche campi utili al client moderno:
  - `RouteKey`
  - `Title`
  - `RootElementTag` (forse transitorio)
  - `LoadingText` (forse transitorio)

Decisione M1:
- mantenere tutti i campi esistenti (compatibilita'/diagnostica)
- non rinominare/rimuovere campi in M1
- pianificare in M2 una distinzione tra DTO "migration metadata" e DTO "runtime client"

### DTO `loader*`
- sono metadata di migrazione/build, non contratti business
- diversi campi riferiscono path legacy (`Views/Home`, `Client/webpack*.js`)

Decisione M1:
- stabilizzare shape attuale per supportare analisi/migrazione
- classificare questi endpoint come `migration/diagnostic contracts`

## 6) Policy di evoluzione contratti (M1)

### 6.1 Regola generale (compatibilita' alta)

Default M1: **compatibile in-place**, salvo eccezioni esplicite.

Consentito in M1:
- aggiungere nuovi endpoint
- aggiungere campi opzionali (non breaking) ai payload
- migliorare messaggi `detail` senza rompere test/consumer (da valutare caso per caso)

Non consentito in M1 (senza nuovo endpoint/versione):
- rinominare campi JSON esistenti
- rimuovere campi dai DTO M1
- cambiare wrapper di successo (`ApiResponse<T>`)
- cambiare shape errori da `ProblemDetails`
- cambiare codici HTTP dei casi gia' definiti

### 6.2 Quando introdurre nuovo endpoint (o versione)

Usare nuovo endpoint/route (o futura versione) se serve:
- cambiare semantica del payload
- ridurre campi legacy-coupled in modo breaking
- introdurre un modello specifico per client moderno incompatibile con quello M1

### 6.3 Classificazione contratti M1

- `Stable for M1`:
  - `GET /api/health`
  - `GET /api/home`
  - `GET /api/home/modules`
  - `GET /api/home/modules/{key}`
- `Migration/Diagnostic (stable within M1)`:
  - `GET /api/home/loader*`

## 7) Gap residui da chiudere nei prossimi blocchi (M1.C / M1.D)

## 7.1 M1.C (Implementazione/Allineamenti)
- Verificare se esistono endpoint M1 prioritari mancanti (dalla matrice M1.A al momento risultano coperti/parziali via metadata)
- Allineare eventuali dettagli messaggi/validazioni se emergono divergenze
- Rifinire metadata OpenAPI se servono attributi aggiuntivi su `GET /api/home` e `GET /api/home/modules` (già presenti su molti endpoint)

## 7.2 M1.D (Test)
- Aggiungere test espliciti per:
  - `400` su chiave vuota/non valorizzata (se testabile dal routing)
  - rami `500` dei `loader*` (richiede strategia test per fault injection/mocking)
  - verifica shape `ProblemDetails` coerente su tutti i casi errore M1

## 8) Esito M1.B

M1.B e' completato quando:
- questa specifica e' approvata
- `M1.C` usa la policy come riferimento implementativo
- `M1.D` usa la tabella codici HTTP/shape come criterio di test

