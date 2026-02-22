# mvc5-angular-webpack

Integrate Angular and Webpack into ASP.NET MVC application for both development and production deployment.

## Current Status (Hybrid Repository)

This repository is currently in a **hybrid migration state**:

- Legacy application: `ASP.NET MVC 5` on `.NET Framework 4.8` in `Server/WebApplication`
- Modern backend (in progress): `ASP.NET Core` on `.NET 9` in `Server/WebCore9`
- Legacy frontend: `Angular 4` + custom `Webpack 2` build in `Client/`

The full port is **not complete yet**. Legacy and modern stacks currently coexist.

## Legacy vs Modern (Porting Status)

| Area | Legacy (current) | Modern (current) | Porting status | Notes |
| --- | --- | --- | --- | --- |
| Backend runtime | `ASP.NET MVC 5` / `.NET Framework 4.8` (`Server/WebApplication`) | `ASP.NET Core` / `.NET 9` (`Server/WebCore9`) | In progress | Both stacks coexist |
| Backend endpoints | MVC controller actions + views | REST API controllers (`/api/home`, `/api/health`) | Partial | Modern API is available and debuggable |
| Frontend | `Angular 4` + custom `Webpack 2` (`Client/`) | Not migrated to Angular 21 yet | Not started / pending | Legacy client still drives UI scenarios |
| UI hosting | Razor views in MVC5 (`Views/Home/*.cshtml`) | No ASP.NET Core UI hosting replacement yet | Pending | `WebCore9.Api` currently serves API only |
| Debug workflow | Visual Studio (`Server/Web-Core.sln`) | VS Code / `dotnet` (`.vscode` + `WebCore9.Api`) | Partial | `.NET 9 Launch WebCore9.Api` available |
| Automated tests | Limited manual smoke tests for legacy UI | Integration tests in `WebCore9.Api.IntegrationTests` | Partial | Add more contract/regression coverage over time |
| Smoke test | Manual UI run in MVC5 | `GET /api/health` | Available | Example endpoint verified in Development |

## Migration Backlog (Operational View)

| Area | Current status | Priority | Next step (Milestone) | Risk | Owner |
| --- | --- | --- | --- | --- | --- |
| Backend API (`WebCore9.Api`) | Running, debuggable, health endpoint verified | High | `M1`: map feature parity vs legacy `HomeController`; `M2`: implement missing endpoints/contracts | Medium | `Backend` (TBD) |
| Backend contracts | Partial DTO/API coverage in modern stack | High | `M1`: baseline response schema list; `M2`: add contract checks for payload/error shape | Medium | `Backend` (TBD) |
| Frontend migration | Legacy `Angular 4` + `Webpack 2` only | High | `M1`: inventory modules/dependencies; `M2`: define upgrade path/intermediate versions; `M3`: start first module migration | High | `Frontend` (TBD) |
| UI hosting replacement | Legacy MVC5 Razor views still host UI | High | `M1`: choose hosting strategy on ASP.NET Core; `M2`: prototype integration path | High | `Backend/Frontend` (TBD) |
| Build/toolchain | Legacy client may be incompatible with modern Node versions | Medium | `M1`: pin/test compatible Node version for legacy client; `M2`: document local setup and constraints | Medium | `DevEx/Build` (TBD) |
| Test coverage | Integration tests available only for modern backend | Medium | `M1`: expand `/api/home*` integration tests; `M2`: add smoke regression checklist for hybrid flows | Medium | `QA/Backend` (TBD) |
| CI/CD | No unified migration pipeline documented | Medium | `M1`: split legacy/modern jobs; `M2`: define minimum CI gates (`build`, `test`, smoke) | Medium | `DevEx/CI` (TBD) |
| Rollout/transition | Hybrid repo without formal rollout plan | Medium | `M1`: define phased rollout; `M2`: define rollback criteria and feature validation gates | High | `Tech Lead` (TBD) |

Related migration docs (M1 backend):
- `docs/migration/backend-parity-matrix.md`
- `docs/migration/backend-api-contracts-m1.md`
- `docs/migration/m1-backend-readiness.md`

Related migration docs (M2 client):
- `docs/migration/client-legacy-inventory-m2a.md`
- `docs/migration/client-modern-strategy-m2b.md`
- `docs/migration/m2d-home-modules-pilot.md`
- `docs/migration/m2e-toolchain-coexistence.md`

Related migration docs (M3 slices):
- `docs/migration/m3a-feature-slice-priority-and-mapping.md`
- `docs/migration/m3b-ci-cd-transition-plan.md`

Recommend to use Visual Studio Code for Angular development and Visual Studio 2017 to run the MVC application.

The repository came up after I answered a question on [Stackoverflow](https://stackoverflow.com/a/47918737/3375906)

## How to run?

Node.js and npm are essential to Angular development.

[Get them now](https://docs.npmjs.com/getting-started/installing-node) if they're not already installed on your machine.

Verify that you are running node **v6.x.x** or higher and npm **3.x.x** or higher by running the commands **node -v** and **npm -v** in a terminal/console window. Older versions produce errors.

## Run the Modern Backend (.NET 9 / WebCore9.Api)

From repository root:

```bash
dotnet build Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj
dotnet run --project Server/WebCore9/WebCore9.Api/WebCore9.Api.csproj
```

Useful smoke test endpoint after startup:

- `https://localhost:7248/api/health` (example HTTPS URL, actual port may vary)
- `http://localhost:5021/api/health` (example HTTP URL, actual port may vary)

Run backend integration tests:

```bash
dotnet test Server/WebCore9/WebCore9.Api.IntegrationTests/WebCore9.Api.IntegrationTests.csproj
```

If you use VS Code, a `.vscode` debug configuration is available for:

- `.NET 9 Launch WebCore9.Api`

It builds `WebCore9.Api`, starts the API, and opens the browser automatically.

## Run the Modern Client (ClientModern / Angular 21)

From `ClientModern/`:

```bash
npm install
npm start
```

Useful routes after startup:

- `http://localhost:4200/health` (M2.C health/status pilot)
- `http://localhost:4200/modules` (M2.D home modules catalog pilot)
- `http://localhost:4200/heroes` (M3.A heroes read/search/detail slice)

Build the modern client:

```bash
npm run build
```

Run modern client quality gates (local CI subset):

```bash
npm run lint
npm run test -- --watch=false
```

## Run the Legacy MVC5 + Angular/Webpack Application

- Clone or download the repository. It will contain **Server** and **Client** folders.
- Make sure you have node installed as above requirement.
- Open Client folder, run build.bat.
- Open Web-Core.sln in Visual Studio, press `F5` or `Ctrl + F5` to run the web application.

Note: the legacy client (`Angular 4` + `Webpack 2`) may require an older Node.js version than modern Node releases (for example Node 20 can cause compatibility issues depending on packages/tooling).

Current local validation (this repo state):
- `Node 20.19.6` + `npm 10.8.2` successfully executed `Client/npm install` and `Client/npm run build:prod`
- `Client/npm run lint` currently fails due to legacy lint issues in the source (not necessarily Node incompatibility)

If everything works fine, you will see the screenshot below.

![module 1](./screen-1.png)
![module 2](./screen-2.png)

For step by step tutorial, please visit https://trungk18.com/experience/asp.net-mvc-5-angular-webpack/

## Reference

This module 2 originally based on the tutorial [Tour of Heroes@Angular team](https://angular.io/tutorial)
