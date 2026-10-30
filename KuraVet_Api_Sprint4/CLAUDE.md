# CLAUDE.md — KuraVet · advanced-business-dotnet

## Contexto
API .NET do KuraVet (Challenge FIAP 2026 x Clyvo Vet). **Sprint 4 = entrega final (04/11/2026)**.
Domínio desta API: tutores, pets, **check-ins de sinais vitais** (com classificação de risco), **linha do tempo
clínica** (eventos) e **relatos de comportamento** (MongoDB). Equipe (2TDSPO): Pedro (RM563405),
Guilherme (RM562396), Henrique (RM563620).

## Estado atual (Sprint 4 — reestruturado)
- Clean Architecture com projetos na raiz (estrutura plana, sem `src/`):
  `KuraVet.Domain` ← `KuraVet.Application` ← `KuraVet.Infrastructure` ← `KuraVet.Api`
  (Api referencia Application e Infrastructure apenas para composição de DI).
- Testes: `KuraVet.Api.Tests.Unit` (Domain, Application, Infrastructure — xUnit + Moq + EF InMemory) e
  `KuraVet.Api.Tests.Integration` (WebApplicationFactory, EF InMemory, Mongo em memória, JWT de teste).
- EF Core 8 + Oracle (migrations em `KuraVet.Infrastructure/Persistence/Migrations`), MongoDB
  (`relatos_comportamento`), JWT + BCrypt (perfis Admin, Veterinario, Tutor), paginação/ordenação/filtros,
  HATEOAS, `GlobalExceptionHandler` (ProblemDetails), Swagger com Bearer, health checks, Serilog, OpenTelemetry.
- A API Key da Sprint 3 foi substituída por JWT.

## Comandos
```bash
dotnet build
dotnet test                                   # não precisa de Oracle nem Mongo
dotnet run --project KuraVet.Api --launch-profile http
dotnet ef migrations add <Nome> --project KuraVet.Infrastructure --startup-project KuraVet.Api
dotnet ef database update      --project KuraVet.Infrastructure --startup-project KuraVet.Api
dotnet ef migrations has-pending-model-changes --project KuraVet.Infrastructure --startup-project KuraVet.Api
dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings
docker compose up -d                          # MongoDB local
```

## Onde fica cada coisa
- Regra de negócio nova → entidade/serviço em `KuraVet.Domain`.
- Caso de uso, DTO, filtro, interface de repositório → `KuraVet.Application/<Recurso>/`.
- Acesso a banco (EF/Mongo), segurança (BCrypt/JWT) → `KuraVet.Infrastructure`.
- Rota HTTP → controller fino em `KuraVet.Api/Controllers` + links em `Hateoas/HateoasLinkBuilder`.
- Erros: lançar `DomainException`, `ValidacaoException`, `NotFoundException`, `ConflitoException` —
  o `GlobalExceptionHandler` converte em ProblemDetails. Não usar try/catch nos controllers.

## Rubrica da Sprint 4 (100 pts) — checklist
1. Arquitetura e código (30): Clean Architecture, SOLID, DI em extension methods, exceções globais. ✅
2. API RESTful (20): Swagger com Bearer, paginação/ordenação/filtros, HATEOAS, JWT com roles. ✅
3. Persistência (20): migrations aplicadas no Oracle (**rodar `database update`**), MongoDB, Repository. ✅ código
4. Observabilidade e testes (20): health, Serilog, OTel, testes unitários e de integração. **Medir cobertura e citar no README.**
5. Documentação (10): README, `docs/arquitetura.png`. **Exportar `docs/openapi.json`.**
Penalidades: não compilar (-20), README incompleto (-10), sem testes (-30).

## Regras invioláveis
- `dotnet build` e `dotnet test` verdes antes de cada commit.
- Não perder funcionalidade das Sprints 1–3 (rotas, health checks, logs, métricas, classificação de risco).
- **Nenhuma credencial versionada**: Oracle, Mongo, `Jwt:Key` e `Seed:Admin` via `dotnet user-secrets`
  ou variáveis de ambiente. `appsettings.json` só com placeholders.
- Schema Oracle diferente do usado pelo backend Java.
- Nunca editar migrations já aplicadas; criar uma nova.
- Commits pequenos, mensagens em pt-BR (`feat:`, `fix:`, `refactor:`, `test:`, `docs:`).
- Antes de apagar ou mover muitos arquivos, mostrar o plano e pedir confirmação.
