# KuraVet API - Infraestrutura de Saúde Veterinária Digital 🐾

O KuraVet é uma solução de backend desenvolvida em .NET 8 para o Challenge 2026, em parceria entre a FIAP e a Clyvo.

O projeto foca na continuidade do cuidado e no engajamento da jornada de saúde do pet, transformando a relação reativa (ir ao veterinário apenas na doença) em uma jornada proativa e contínua.

Esta API gerencia desde o cadastro de tutores e pets até o monitoramento preventivo via check-ins de sinais vitais e histórico clínico (linha do tempo).

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

Para detalhes técnicos de cada modelo (JSON), acesse a interface do Swagger.

---

# 👤 Gestão de Tutores (`/api/Tutor`)

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Tutor` | Cadastra um novo tutor |
| GET | `/api/Tutor` | Lista todos os tutores cadastrados |
| GET | `/api/Tutor/{id}` | Busca detalhes de um tutor pelo ID |

---

# 🐾 Gestão de Pets (`/api/Pet`)

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Pet` | Cadastra um pet vinculado a um Tutor |
| GET | `/api/Pet/{id}` | Retorna o perfil completo do Pet |
| GET | `/api/Pet/tutor/{tutorId}` | Lista todos os pets de um tutor específico |

---

# 🩺 Check-ins de Sinais Vitais (`/api/Checkin`)

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Checkin` | Registra sinais vitais (Respiração, TPC, Hidratação) |
| GET | `/api/Checkin/pet/{petId}` | Histórico cronológico de saúde de um pet |
| GET | `/api/Checkin/risco/{nivel}` | Filtra alertas por nível (Ex: "Moderado", "Baixo") |
| PUT | `/api/Checkin/{id}` | Atualiza dados de um check-in |
| DELETE | `/api/Checkin/{id}` | Remove um registro de histórico |

---

# 🏥 Linha do Tempo Clínica (`/api/EventoConsulta`)

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/EventoConsulta` | Registra Vacinas, Exames ou Consultas |
| GET | `/api/EventoConsulta/pet/{petId}` | Retorna a linha do tempo completa do pet |

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
git clone https://github.com/SEU_USUARIO/KuraVet.Api.git
cd KuraVet.Api

2. Configuração da String de Conexão
No arquivo appsettings.json, ajuste a chave OracleConnection com suas credenciais:
"ConnectionStrings": {
  "OracleConnection": "Data Source=localhost:1521/XEPDB1;User Id=SEU_USER;Password=SUA_SENHA;"}

3. Migração do Banco de Dados
Execute as migrações para criar as tabelas automaticamente via Package Manager Console:
Update-Database

Ou via CLI:
dotnet ef database update

4. Execução

Inicie o projeto através do Visual Studio (F5) ou via terminal:
dotnet run

Após iniciar, a documentação Swagger estará disponível em:
https://localhost:PORTA/swagger


🛠️ Tecnologias
ASP.NET Core 8.0
Entity Framework Core 8 (Oracle)
Swagger/OpenAPI
Auto-análise de Risco (Simulação de IA)
