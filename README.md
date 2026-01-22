# CustomerPlatform

> Plataforma de cadastro de clientes com deduplicacao, busca por relevancia e consistencia eventual.

---

## Introducao

Este projeto implementa um desafio tecnico de cadastro de clientes com separacao entre write model (PostgreSQL) e read model (ElasticSearch), consistencia eventual via Outbox + RabbitMQ e deduplicacao assincrona. A arquitetura segue Clean Architecture, com camadas Domain, Application, Infrastructure, API e Worker.

---

## Features

- ✅ API REST para cadastro PF/PJ, atualizacao e busca
- ✅ Persistencia no PostgreSQL com EF Core
- ✅ Outbox + RabbitMQ para consistencia eventual
- ✅ ElasticSearch como read model com relevancia
- ✅ Deduplicacao assincrona com score e suspeitas
- ✅ Observabilidade com Serilog + OpenTelemetry + Aspire Dashboard

---

## Conteudo

- [Introducao](#introducao)
- [Features](#features)
- [Instalacao](#instalacao)
- [Quick Start](#quick-start)
- [Exemplos de Uso](#exemplos-de-uso)
  - [Exemplo 1: Cadastro PF](#exemplo-1-cadastro-pf)
  - [Exemplo 2: Busca](#exemplo-2-busca)
- [Referencia da API](#referencia-da-api)
  - [Endpoints](#endpoints)
  - [DTOs Principais](#dtos-principais)
  - [Enums](#enums)
- [Build e Testes](#build-e-testes)
- [Testes de Relevancia e Deduplicacao](#testes-de-relevancia-e-deduplicacao)
- [Dependencias](#dependencias)
- [Referencias](#referencias)
- [Licenca](#licenca)

---

## Instalacao

### Via .NET CLI
```bash
dotnet restore
```

### Via Docker Compose
```bash
docker compose -f docker/docker-compose.yaml up -d
```

---

## Quick Start

Execucao local da API:

```bash
dotnet run --project src/CustomerPlatform.Api
```

---

## Exemplos de Uso

Os exemplos abaixo usam o arquivo `src/CustomerPlatform.Api/CustomerPlatform.Api.Test.http`.

### Exemplo 1: Cadastro PF

```http
POST http://localhost:5000/customers/pf
Content-Type: application/json

{
  "fullName": "Maria da Silva",
  "cpf": "17871018434",
  "email": "maria@teste.com",
  "phone": "11999999999",
  "birthDate": "1990-01-10",
  "address": {
    "street": "Rua Central",
    "number": "100",
    "complement": "Apto 12",
    "postalCode": "12345000",
    "city": "Sao Paulo",
    "state": "SP"
  }
}
```

### Exemplo 2: Busca

```http
GET http://localhost:5000/customers/search?name=Maria&pageNumber=1&pageSize=10
```

---

## Referencia da API

### Endpoints

| Metodo | Rota | Descricao |
| --- | --- | --- |
| `POST` | `/customers/pf` | Cadastro de cliente PF |
| `POST` | `/customers/pj` | Cadastro de cliente PJ |
| `PUT` | `/customers/{id}` | Atualizacao de cliente |
| `GET` | `/customers/search` | Busca no ElasticSearch |
| `GET` | `/health` | Health checks |

### DTOs Principais

| DTO | Finalidade |
| --- | --- |
| `CreateIndividualCustomerCommand` | Cadastro PF |
| `CreateCompanyCustomerCommand` | Cadastro PJ |
| `UpdateCustomerRequest` | Atualizacao |
| `CustomerDto` | Retorno padrao |

### Enums

TipoCliente

| Valor | Descricao |
| --- | --- |
| `PF` | Pessoa fisica |
| `PJ` | Pessoa juridica |

---

## Build e Testes

```bash
dotnet build CustomerPlatform.sln
dotnet test
```

---

## Testes de Relevancia e Deduplicacao

A cobertura atual valida os principais cenarios de busca e deduplicacao para o MVP:

- Relevancia (ElasticSearch): garante indexacao e consulta basica, ordenacao com nome exato em primeiro e filtros por email/telefone.
  - Arquivo: `tests/CustomerPlatform.IntegrationTests/Tests/Infrastructure/Search/ElasticSearchIntegrationTests.cs`
- Deduplicacao: confirma suspeita com nome + telefone e nome + email, ausencia de suspeita quando apenas o nome e similar e quando compara PJ vs PF, e seed grande para validar consistencia de score.
  - Arquivo: `tests/CustomerPlatform.IntegrationTests/Tests/Infrastructure/Deduplication/DeduplicationIntegrationTests.cs`

---

## Dependencias

Runtime:
- PostgreSQL
- RabbitMQ
- ElasticSearch
- OpenTelemetry Collector
- Aspire Dashboard

Desenvolvimento:
- .NET 8 SDK
- Docker e Docker Compose

---

## Referências

### Documentação do Projeto
- `docs/README.md`
- `docs/DESAFIO.md`
- `docs/TEMPLATE_ENTREGA.md`
- `ARQUITETURA.md`
- `PLANEJAMENTO.md`
- `COMO_EXECUTAR.md`

### Instruções e Padrões Utilizados
- https://github.com/ThiagoGuislotti/copilot-instructions

---

## Licenca

Sem licenca definida para este repositorio.