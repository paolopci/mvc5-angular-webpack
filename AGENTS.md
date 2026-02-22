# Repository Guidelines

## Project Structure & Module Organization
This repository is split into `Client/` (Angular + Webpack) and `Server/` (ASP.NET MVC 5).

- `Client/modules/`: Angular source code, shared bootstrap, polyfills, and feature modules (`angularModule-1`, `angularModule-2`).
- `Client/webpack*.js`: Webpack build configs for normal and HTML-plugin builds.
- `Server/WebApplication/`: MVC app (`Controllers/`, `Views/`, `Content/`, `Scripts/`, `App_Start/`).
- `Server/Web-Core.sln`: Visual Studio solution for the server application.

Keep client and server changes scoped to their folders unless the integration contract changes.

## Build, Test, and Development Commands
Run client commands from `Client/`.

- `npm install`: install dependencies.
- `npm run dev`: start `webpack-dev-server` (HTTPS, opens browser) for Angular-only development.
- `npm run build`: build client bundles with Webpack.
- `npm run build:prod`: production client build (`--env.MODE=prod`).
- `npm run lint`: run TSLint on `modules/**/*.ts`.
- `build.bat`: legacy scripted flow (`npm install --production`, shrinkwrap, prod build).

Run the MVC app by opening `Server/Web-Core.sln` in Visual Studio and pressing `F5` / `Ctrl+F5`.

## Coding Style & Naming Conventions
Use existing project conventions:

- TypeScript/Angular uses 2-space indentation.
- Angular components follow kebab-case file names (for example, `heroes.component.ts`).
- Component selectors use the `sg-` prefix and kebab-case; directives use `sg` + camelCase (enforced in `Client/tslint.json`).
- Keep classes/types in PascalCase and methods/properties in camelCase.

Run `npm run lint` before opening a PR.

## Testing Guidelines
There is no automated test suite in this repository yet (no `*.spec.ts` or server test project checked in).

Minimum validation for contributions:
- `npm run lint`
- `npm run build:prod`
- Smoke test the MVC app via Visual Studio (`Server/Web-Core.sln`)

If adding tests, place Angular specs next to source files as `*.spec.ts` and document how to run them.

## Commit & Pull Request Guidelines
Recent history uses short messages like `Update README.md` and `Update URL`. For new work, prefer clearer imperative messages with scope, for example:

- `client: fix webpack production config`
- `server: update HomeController route`

PRs should include a concise summary, affected area (`Client`/`Server`), manual test steps, and screenshots for UI changes.
