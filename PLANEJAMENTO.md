# Planejamento

Principios:
- Comecar pelo Dominio e seguir para fora.
- Manter passos simples e checklist do que foi feito.

## Fases (checklist)

Fase 1 - Planejamento e alinhamento
- [x] Consolidar arquitetura e stack em `ARQUITETURA.md`.
- [x] Definir interface REST.
- [x] Estruturar planejamento em fases.
- [x] Registrar Prompt #1 em `PROMPTS_UTILIZADOS.md`.

Fase 2 - Dominio
- [x] Adicionar Enums, ValueObjects e Events.
- [x] Definir regras de validacao e excecoes de negocio.
- [x] Criar testes unitarios do dominio (entidades, value objects, regras).
- [x] Criar estrutura inicial de testes de integracao (setup do runner).
- [x] Registrar Prompt #2 em `PROMPTS_UTILIZADOS.md`.

Fase 3 - Application
- [x] Criar `CustomerPlatform.Application` com Cqrs/, Abstractions/ e DependencyInjections/.
- [x] Implementar comandos, queries, handlers e validacoes.
- [x] Criar testes unitarios para handlers e validacoes.
- [x] Registrar Prompt #3 em `PROMPTS_UTILIZADOS.md`.

Fase 4 - Infrastructure
- [x] Implementar DbContext, migrations e repositorios (PostgreSQL).
- [x] Implementar Outbox (tabela, escrita e migrations).
- [x] Implementar RabbitMQ (publicacao/consumo) com resiliencia.
- [x] Implementar Search (ElasticSearch) e indexacao.
- [x] Implementar Deduplicacao com score e suspeitas.
- [x] Configurar Serilog e OpenTelemetry.
- [x] Ajustar docker compose (servicos, healthcheck, imagens fixas).
- [x] Criar testes de integracao para DB, RabbitMQ e Search.
- [x] Registrar Prompt #4 em `PROMPTS_UTILIZADOS.md`.

Fase 5 - API e Worker
- [ ] Implementar controllers REST (CRUD + busca probabilistica).
- [ ] Implementar ProblemDetails, correlationId e health checks.
- [x] Substituir publicacao direta por Outbox (Application).
- [x] Criar Worker e consumidores de eventos para deduplicacao.
- [x] Aplicar resiliencia no Worker (RabbitMQ/Elastic).
- [ ] Criar testes de integracao de API e fluxo de eventos do Worker.
- [x] Registrar Prompt #5 em `PROMPTS_UTILIZADOS.md`.

Fase 6 - Testes, Docker e Docs
- [x] Criar UnitTests e IntegrationTests com Ductus.FluentDocker.
- [x] Ajustar docker compose para PostgreSQL, RabbitMQ, Search e observabilidade.
- [ ] Atualizar README/COMO_EXECUTAR e preencher DECISOES_TECNICAS e PROMPTS_UTILIZADOS.
- [ ] Registrar Prompt #6 em `PROMPTS_UTILIZADOS.md`.

## Estado atual (mapeado)
Dominio
- [x] Entidades base de cliente existem em `src/CustomerPlatform.Domain/Entities/Customer.cs`.
- [x] Entidades PF/PJ separadas por arquivo em `src/CustomerPlatform.Domain/Entities`.
- [x] Pastas Enums, ValueObjects, Events e Exceptions existem.

Aplicacao
- [x] `src/CustomerPlatform.Application` existe com CQRS, DTOs, validadores e DI.
- [x] Testes unitarios da Application em `tests/CustomerPlatform.UnitTests/Tests/Application`.

Infraestrutura
- [x] Projeto `src/CustomerPlatform.Infrastructure` existe com persistencia (EF Core).
- [x] Mensageria RabbitMQ (publicacao/consumo) e outbox configurados.
- [x] ElasticSearch para leitura e indexacao assincrona.
- [x] Deduplicacao com score e registro de suspeitas.
- [x] Observabilidade (Serilog + OpenTelemetry) configurada.

API
- [x] `src/CustomerPlatform.Api` existe com HealthController e Swagger.
- [x] Decisao de interface: REST.
- [ ] Endpoints de clientes, validacoes e observabilidade estao faltando.

Worker
- [x] `src/CustomerPlatform.Worker` existe com HostedServices para outbox e consumo.

Testes
- [x] `tests/CustomerPlatform.Tests` existe (legado).
- [x] `tests/CustomerPlatform.UnitTests` existe.
- [x] `tests/CustomerPlatform.IntegrationTests` existe (setup inicial).

Docker
- [x] Pasta `docker/` com compose files existe.
- [x] Composes de Postgres, RabbitMQ, Elastic e observabilidade atualizados.
- [x] `docker-compose.exemplo.yml` existe.

Docs
- [x] README.md, DESAFIO.md, TEMPLATE_ENTREGA.md, ESTRUTURA_PROJETO.md existem.
- [x] DECISOES_TECNICAS.md existe.
- [ ] CRITERIOS_AVALIACAO.md e COMO_EXECUTAR.md faltam.
- [x] PROMPTS_UTILIZADOS.md existe.
- [x] ARQUITETURA.md existe.
- [x] PLANEJAMENTO.md existe.
