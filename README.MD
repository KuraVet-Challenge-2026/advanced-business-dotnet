# KuraVet API - Infraestrutura de Saúde Veterinária Digital 🐾
O KuraVet é uma solução de backend desenvolvida em .NET 8 para o Challenge 2026, em parceria entre a FIAP e a Clyvo.
O projeto foca na continuidade do cuidado e no engajamento da jornada de saúde do pet, transformando a relação reativa (ir ao veterinário apenas na doença) em uma jornada proativa e contínua.
Esta API gerencia desde o cadastro de tutores e pets até o monitoramento preventivo via check-ins de sinais vitais e histórico clínico (linha do tempo).

**Novidades da Sprint 3 (Advanced Business Development with .NET):**
- 🩺 **Health Checks** (`/health`, `/health/live`, `/health/ready`) para monitorar a saúde da API, a conectividade com o banco Oracle e o uso de recursos.
- 📝 **Logging estruturado** com Serilog (console + arquivo), com correlação de requisições entre as camadas.
- 📊 **Tracing distribuído e métricas de desempenho** com OpenTelemetry (tempo de resposta, taxa de erros).
- 🔑 **Autenticação por API Key** em todos os endpoints de negócio.
- 🧪 **Testes automatizados** com xUnit (unitários com Moq + integração com WebApplicationFactory).

Veja a seção [🩺 Sprint 3 — Monitoramento, Observabilidade e Testes Automatizados](#-sprint-3--monitoramento-observabilidade-e-testes-automatizados) para todos os detalhes.

---

# 👥 Equipe (Turma 2TDSPO)
- Pedro Henrique Luiz Alves Duarte - RM563405
- Guilherme Macedo Martins - RM562396
- Henrique Martins Oliveira - RM563620

---

# 📖 Descrição do Projeto
A API KuraVet serve como o núcleo de dados para um ecossistema que conecta clínicas veterinárias e tutores.

## Objetivo
Garantir que a saúde do animal seja monitorada constantemente, permitindo intervenções precoces baseadas em dados.

## Diferencial
Possui uma lógica de Simulação de IA no registro de Check-ins, que analisa parâmetros como o Tempo de Preenchimento Capilar (TPC) para classificar o nível de risco do pet automaticamente.

## Persistência
Utiliza banco de dados Oracle com Entity Framework Core, garantindo integridade e escalabilidade para grandes volumes de históricos clínicos.

---

# 🛣️ Documentação das Rotas
Abaixo estão os endpoints principais.
Para detalhes técnicos de cada modelo (JSON), acesse a interface do Swagger ou Scalar.

---

# 👤 Gestão de Tutores (`/api/Tutor`)
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Tutor` | Cadastra um novo tutor |
| GET | `/api/Tutor` | Lista todos os tutores cadastrados |
| GET | `/api/Tutor/{id}` | Busca detalhes de um tutor pelo ID |
| PUT | `/api/Tutor/{id}` | Atualiza dados de um tutor existente |
| DELETE | `/api/Tutor/{id}` | Remove um tutor pelo ID |

---

# 🐾 Gestão de Pets (`/api/Pet`)
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Pet` | Cadastra um pet vinculado a um Tutor |
| GET | `/api/Pet` | Lista todos os pets cadastrados |
| GET | `/api/Pet/{id}` | Retorna o perfil completo do Pet |
| GET | `/api/Pet/tutor/{tutorId}` | Lista todos os pets de um tutor específico |
| PUT | `/api/Pet/{id}` | Atualiza dados de um pet existente |
| DELETE | `/api/Pet/{id}` | Remove um pet pelo ID |

---

# 🩺 Check-ins de Sinais Vitais (`/api/Checkin`)
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Checkin` | Registra sinais vitais (Respiração, TPC, Hidratação) |
| GET | `/api/Checkin` | Lista todos os check-ins |
| GET | `/api/Checkin/{id}` | Busca um check-in pelo ID |
| GET | `/api/Checkin/pet/{petId}` | Histórico cronológico de saúde de um pet |
| GET | `/api/Checkin/risco/{nivel}` | Filtra alertas por nível (Ex: "Moderado", "Baixo") |
| PUT | `/api/Checkin/{id}` | Atualiza dados de um check-in |
| DELETE | `/api/Checkin/{id}` | Remove um registro de histórico |

---

# 🏥 Linha do Tempo Clínica (`/api/EventoConsulta`)
| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/EventoConsulta` | Registra Vacinas, Exames ou Consultas |
| GET | `/api/EventoConsulta` | Lista todos os eventos clínicos |
| GET | `/api/EventoConsulta/{id}` | Busca um evento pelo ID |
| GET | `/api/EventoConsulta/pet/{petId}` | Retorna a linha do tempo completa do pet |
| PUT | `/api/EventoConsulta/{id}` | Atualiza um evento clínico existente |
| DELETE | `/api/EventoConsulta/{id}` | Remove um evento clínico |

---

# ⚙️ Instalação e Execução

## Pré-requisitos
- SDK .NET 8.0
- Visual Studio 2022 (ou VS Code com C# Dev Kit)
- Acesso a um banco de dados Oracle (Cloud ou Local)

---

# 🚀 Passo a Passo

## 1. Clonar o Repositório
```bash
git clone https://github.com/KuraVet-Challenge-2026/advanced-business-dotnet.git
cd KuraVet.Api
```

## 2. Configuração da String de Conexão
No arquivo `appsettings.json`, ajuste a chave `OracleConnection` com suas credenciais:
```json
"ConnectionStrings": {
  "OracleConnection": "Data Source=localhost:1521/XEPDB1;User Id=SEU_USER;Password=SUA_SENHA;"
}
```

## 3. Migração do Banco de Dados
Execute as migrações para criar as tabelas automaticamente via Package Manager Console:
```powershell
Update-Database
```
Ou via CLI:
```bash
dotnet ef database update
```

## 4. Execução
Inicie o projeto através do Visual Studio (F5) ou via terminal:
```bash
dotnet run
```

---

# 📄 Documentação com Open API

A API KuraVet possui documentação interativa completa configurada com **Swagger UI** e **Scalar**, ambas disponíveis assim que o projeto é iniciado.

## 🔗 Interfaces Disponíveis

| Interface | URL | Descrição |
|---|---|---|
| **Swagger UI** | `https://localhost:{porta}/swagger` | Interface clássica com formulários de teste integrados |
| **Scalar** | `https://localhost:{porta}/scalar/v1` | Interface moderna com client HTTP integrado |
| **OpenAPI JSON** | `https://localhost:{porta}/openapi/v1.json` | Especificação bruta em formato JSON |

---

## ✅ Swagger UI Configurado

O Swagger está configurado com as seguintes funcionalidades:

- `EnableAnnotations()` — habilita as anotações `[SwaggerOperation]` nos controllers
- `IncludeXmlComments()` — inclui os comentários XML gerados automaticamente pelo build
- `DisplayRequestDuration()` — exibe o tempo de resposta de cada requisição testada
- `EnableFilter()` — campo de busca para encontrar endpoints rapidamente
- `EnableTryItOutByDefault()` — formulário de teste já aberto por padrão, sem precisar clicar
- `DefaultModelsExpandDepth(2)` — schemas dos models expandidos automaticamente

O `OpenApiInfo` está configurado com título, versão, descrição, contato da equipe e licença MIT.

---

## ✅ Scalar Configurado

O Scalar oferece uma interface moderna com:

- Tema **Purple**
- Client padrão **C# HttpClient** (gera snippet de código automaticamente)
- Integrado ao mesmo `openapi/v1.json` gerado pelo Swagger

---

## 📋 Endpoints Documentados

Todos os **20 endpoints** da API estão completamente documentados com:

**Nos Controllers** — `[ProducesResponseType]`, `[SwaggerOperation]` e comentários XML:
```csharp
/// <summary>Registra um novo check-in de saúde.</summary>
/// <param name="model">Sinais vitais do pet a serem registrados.</param>
/// <returns>Check-in criado com o nível de risco calculado pela IA.</returns>
[HttpPost]
[SwaggerOperation(
    Summary = "Registrar novo check-in",
    Description = "Cadastra os sinais vitais e gera automaticamente o nível de risco via IA.",
    OperationId = "CreateCheckin")]
[ProducesResponseType(typeof(CheckinHistorico), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Post([FromBody] CheckinHistorico model) { ... }
```

**Nos Models** — `/// <summary>` e `/// <example>` em todos os campos:
```csharp
/// <summary>Tempo de preenchimento capilar em segundos. Valores acima de 2 indicam risco moderado.</summary>
/// <example>1</example>
[Required]
public int TempoPreenchimentoCapilar { get; set; }
```

**Códigos de resposta documentados por endpoint:**

| Controller | GET all | GET by id | GET filtro | POST | PUT | DELETE |
|---|---|---|---|---|---|---|
| Tutor | 200, 204, 400 | 200, 404, 400 | — | 201, 400 | 200, 404, 400 | 200, 404, 400 |
| Pet | 200, 204, 400 | 200, 404, 400 | 200, 204, 400 | 201, 400 | 200, 404, 400 | 200, 404, 400 |
| Checkin | 200, 204, 400 | 200, 404, 400 | 200, 204, 400 | 201, 400 | 200, 404, 400 | 200, 404, 400 |
| EventoConsulta | 200, 204, 400 | 200, 404, 400 | 200, 204, 400 | 201, 400 | 200, 404, 400 | 200, 404, 400 |

---

## 🧪 Testando pela Interface

**Via Swagger UI (`/swagger`):**
1. Acesse `https://localhost:{porta}/swagger` no navegador
2. Os endpoints estão organizados por grupo: **Tutores**, **Pets**, **Check-ins de Saúde**, **Eventos Clínicos**
3. Clique em qualquer endpoint — o formulário "Try it out" já estará aberto
4. Preencha os campos (os schemas com exemplos aparecem ao lado)
5. Clique em **Execute** e veja a resposta real da API

**Via Scalar (`/scalar/v1`):**
1. Acesse `https://localhost:{porta}/scalar/v1`
2. Selecione o endpoint desejado no menu lateral
3. Preencha os parâmetros e clique em **Send**
4. O Scalar gera automaticamente o snippet de código C# equivalente

**Exemplo de body para testar `POST /api/Checkin`:**
```json
{
  "dataCheckin": "2026-05-21T10:30:00",
  "frequenciaRespiratoria": 22,
  "tempoPreenchimentoCapilar": 1,
  "corMucosa": "Rosada",
  "nivelHidratacao": "Normal",
  "petId": 1
}
```
> 💡 O campo `nivelRiscoIA` não precisa ser enviado — é calculado automaticamente pela API.

---

---

# 🩺 Sprint 3 — Monitoramento, Observabilidade e Testes Automatizados

Esta seção documenta o que foi adicionado na Sprint 3 (Advanced Business Development with .NET), em cima da API já existente. A entrega é **puramente local**: sem deploy na Azure e sem integração com serviços/interfaces externas.

## 🩺 Health Checks

A API expõe três endpoints de Health Check (`Microsoft.Extensions.Diagnostics.HealthChecks`), todos retornando um JSON detalhado com o status geral, a duração de cada checagem e dados adicionais:

| Endpoint | Finalidade | Checagens incluídas |
|---|---|---|
| `GET /health` | Visão geral completa | Banco Oracle + API + Memória |
| `GET /health/live` | Liveness (a API está de pé?) | Apenas a checagem `api-self` |
| `GET /health/ready` | Readiness (pronta para atender?) | Banco Oracle (`oracle-database`) + Memória (`system-resources`) |

Esses três endpoints **não exigem autenticação** (API Key), para permitir que ferramentas de monitoramento/orquestração os consultem livremente.

Exemplo de resposta de `GET /health`:
```json
{
  "status": "Healthy",
  "totalDurationMs": 12.4,
  "checks": [
    {
      "name": "oracle-database",
      "status": "Healthy",
      "description": null,
      "durationMs": 8.1,
      "tags": [ "ready", "db" ],
      "data": {}
    },
    {
      "name": "api-self",
      "status": "Healthy",
      "description": "A API KuraVet está em execução e respondendo normalmente.",
      "durationMs": 0.01,
      "tags": [ "live" ],
      "data": {}
    },
    {
      "name": "system-resources",
      "status": "Healthy",
      "description": "Uso de memória dentro do esperado.",
      "durationMs": 0.3,
      "tags": [ "ready" ],
      "data": { "memoriaAlocadaMB": 42.7 }
    }
  ]
}
```

> ℹ️ A checagem `system-resources` (uso de memória do processo) substitui, nesta entrega 100% local, uma verificação de "disponibilidade de serviços externos" — já que o projeto não integra com nenhum serviço de terceiros.

## 📝 Logging Estruturado (Serilog)

- Logs em `Information`, `Warning` (respostas de erro/"não encontrado") e `Error` (exceções), em todos os Controllers.
- Saída simultânea em **Console** e em **arquivo** (`logs/kuravet-api-{data}.log`, rotação diária, mantendo os últimos 7 arquivos).
- Cada requisição tem um `CorrelationId` (baseado no `TraceIdentifier` do ASP.NET Core) propagado nos logs e devolvido no header de resposta `X-Correlation-Id`, permitindo correlacionar uma mesma requisição entre as diferentes camadas da aplicação.
- Configuração em `appsettings.json`, seção `"Serilog"`.

## 📊 Tracing e Métricas (OpenTelemetry)

- Tracing distribuído com `ActivitySource` próprio (`KuraVet.Api`) + instrumentação automática de ASP.NET Core e `HttpClient`.
- Métricas customizadas expostas via `Meter` (`KuraVet.Api.Metrics`):
  - `kuravet_api_requests_total` — total de requisições.
  - `kuravet_api_requests_errors_total` — total de requisições com status ≥ 400 (taxa de erros).
  - `kuravet_api_request_duration_ms` — tempo de resposta (ms).
- Exportador **Console** (traces e métricas aparecem no terminal onde a API está rodando) — mantém a entrega 100% local, sem Azure Application Insights nem coletores externos.

## 🔑 Autenticação (API Key)

O projeto da Sprint 2 não possuía nenhum mecanismo de autenticação. Como a rubrica da Sprint 3 exige que os testes de integração cubram o fluxo de autenticação, foi adicionado um esquema simples e **100% local** (sem integração com nenhum provedor de identidade externo): todos os endpoints de `Tutor`, `Pet`, `Checkin` e `EventoConsulta` agora exigem o header `X-Api-Key`.

- Chave padrão (ambiente de desenvolvimento): `kuravet-dev-key-2026` (configurável em `appsettings.json`, chave `Authentication:ApiKey`).
- Sem o header ou com uma chave incorreta → `401 Unauthorized`.
- Os endpoints de Health Check (`/health*`) **não** exigem essa chave.
- No Swagger UI, clique em **Authorize** e informe a chave para testar os endpoints protegidos.

## 🧪 Testes Automatizados

A solução agora tem **três projetos**:

```
KuraVet.Api.sln
├── KuraVet.Api                     (API)
├── KuraVet.Api.Tests.Unit          (xUnit + Moq — Domínio e Aplicação)
└── KuraVet.Api.Tests.Integration   (xUnit + WebApplicationFactory)
```

### Como rodar os testes

```bash
# Todos os testes (os dois projetos)
dotnet test

# Apenas os testes unitários
dotnet test KuraVet.Api.Tests.Unit

# Apenas os testes de integração
dotnet test KuraVet.Api.Tests.Integration
```

Os testes de integração **não dependem do Oracle real**: o `CustomWebApplicationFactory` substitui o `KuraVetDbContext` pelo provider **InMemory** do EF Core, então `dotnet test` roda offline, sem nenhuma credencial de banco.

### `KuraVet.Api.Tests.Unit`

- **Domínio**: validação das Data Annotations de `Tutor`, `Pet`, `CheckinHistorico`, `EventoConsulta`, e testes de `CheckinRiscoService` (regra de classificação de risco, extraída do `CheckinController`).
- **Aplicação** (Controllers): `TutorControllerTests`, `PetControllerTests`, `CheckinControllerTests`, `EventoConsultaControllerTests` — dependências mockadas com **Moq** (`ILogger`, e `ICheckinRiscoService` no caso do `CheckinController`), persistência com EF Core InMemory.
- `PetControllerTests` usa `IClassFixture<SeedDataFixture>` para compartilhar uma base semeada entre os testes da classe.

### `KuraVet.Api.Tests.Integration`

- `CustomWebApplicationFactory : WebApplicationFactory<Program>` — troca o Oracle pelo InMemory e injeta uma API Key de teste.
- `KuraVetApiCollection` (`ICollectionFixture`) compartilha essa mesma factory entre as classes de teste de endpoints, evitando subir a aplicação uma vez por classe.
- Cobrem o fluxo HTTP completo: `AuthenticationTests` (401 sem chave / com chave errada / 200-204 com chave válida), `HealthCheckEndpointTests`, `TutorEndpointsTests`, `PetEndpointsTests`, `CheckinEndpointsTests` (inclui a classificação de risco de ponta a ponta) e `EventoConsultaEndpointsTests` — sempre cobrindo sucesso, 404 e 400 (validação).

### Convenção de nomenclatura

Todos os testes seguem `MetodoTestado_Cenario_ResultadoEsperado`, por exemplo:
`Post_CheckinComTpcAcimaDeDois_DeveRetornarRiscoModerado`.

---

# 🛠️ Tecnologias
- ASP.NET Core 8.0
- Entity Framework Core 8 (Oracle / InMemory nos testes)
- Swagger UI / OpenAPI (`Swashbuckle.AspNetCore` 6.6.2)
- Scalar (`Scalar.AspNetCore` 2.5.6)
- Auto-análise de Risco (Simulação de IA) — `KuraVet.Api.Domain.CheckinRiscoService`
- **Sprint 3:**
  - `Microsoft.Extensions.Diagnostics.HealthChecks` + `...HealthChecks.EntityFrameworkCore` (Health Checks)
  - `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.File` (logging estruturado)
  - `OpenTelemetry`, `OpenTelemetry.Instrumentation.AspNetCore/Http`, `OpenTelemetry.Exporter.Console` (tracing e métricas)
  - Autenticação por API Key (esquema local, sem integração externa)
  - `xUnit`, `Moq`, `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory` (testes automatizados)
