# Criterios de Avaliacao

## Itens avaliaveis

- Arquitetura seguindo Domain -> Application -> Infrastructure -> API -> Worker
- Persistencia no PostgreSQL com outbox e consistencia eventual
- Indexacao e busca via ElasticSearch (read model)
- Deduplicacao assincrona com registro de suspeitas
- Observabilidade (Serilog + OpenTelemetry + Aspire Dashboard)
- Docker compose com healthchecks e imagens fixas
- Testes unitarios e de integracao executando
- Documentacao clara de execucao e uso

## Como validar

1) Subir infraestrutura:
```bash
docker compose -f docker/docker-compose.yaml up -d
```

2) Rodar API e Worker:
```bash
dotnet run --project src/CustomerPlatform.Api
dotnet run --project src/CustomerPlatform.Worker
```

3) Executar testes:
```bash
dotnet test
```

4) Validar endpoints:
- `GET /health`
- `POST /customers/pf`
- `POST /customers/pj`
- `PUT /customers/{id}`
- `GET /customers/search`

5) Validar observabilidade:
- Aspire Dashboard: `http://localhost:18888`