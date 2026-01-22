# Como Executar

## Pre-requisitos

- .NET 8 SDK
- Docker e Docker Compose

## Subir infraestrutura (PostgreSQL, RabbitMQ, Elastic, Observabilidade)

```bash
docker compose -f docker/docker-compose.yaml up -d
```

## Rodar API

```bash
dotnet run --project src/CustomerPlatform.Api
```

## Rodar Worker

```bash
dotnet run --project src/CustomerPlatform.Worker
```

## Rodar testes

```bash
dotnet test
```

## Endpoints uteis

- Swagger: `http://localhost:5000/swagger`
- Health: `http://localhost:5000/health`
- Aspire Dashboard: `http://localhost:18888` (sem autenticacao)

- RabbitMQ UI: `http://localhost:15672` (usuario: admin, senha: NetToolsKit.Pass!)
- PostgreSQL: `Host=localhost;Port=5432;Database=CustomerPlatformDbDevelopment;Username=postgres;Password=NetToolsKit.Pass!`
- Kibana: `http://localhost:5601` (sem autenticacao)
- ElasticSearch: `http://localhost:9200`

## Documentacao do desafio

Os arquivos originais estao em `docs/`.