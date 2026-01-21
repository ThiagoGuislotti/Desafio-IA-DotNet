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
- [ ] Criar `CustomerPlatform.Application` com Cqrs/, Abstractions/ e DependencyInjections/.
- [ ] Implementar comandos, queries, handlers e validacoes.
- [ ] Criar testes unitarios para handlers e validacoes.
- [ ] Registrar Prompt #3 em `PROMPTS_UTILIZADOS.md`.

Fase 4 - Infrastructure
- [ ] Implementar DbContext, migrations e repositorios (PostgreSQL).
- [ ] Implementar RabbitMQ (publicacao/consumo) com resiliencia.
- [ ] Implementar Search (ElasticSearch) e indexacao.
- [ ] Implementar Deduplicacao com score e suspeitas.
- [ ] Configurar Serilog e OpenTelemetry.
- [ ] Criar testes de integracao para DB, RabbitMQ e Search.
- [ ] Registrar Prompt #4 em `PROMPTS_UTILIZADOS.md`.

Fase 5 - API e Worker
- [ ] Implementar controllers REST (CRUD + busca probabilistica).
- [ ] Implementar ProblemDetails, correlationId e health checks.
- [ ] Criar Worker e consumidores de eventos para deduplicacao.
- [ ] Criar testes de integracao de API e fluxo de eventos do Worker.
- [ ] Registrar Prompt #5 em `PROMPTS_UTILIZADOS.md`.

Fase 6 - Testes, Docker e Docs
- [ ] Criar UnitTests e IntegrationTests com Ductus.FluentDocker.
- [ ] Ajustar docker compose para PostgreSQL, RabbitMQ, Search e observabilidade.
- [ ] Atualizar README/COMO_EXECUTAR e preencher DECISOES_TECNICAS e PROMPTS_UTILIZADOS.
- [ ] Registrar Prompt #6 em `PROMPTS_UTILIZADOS.md`.

## Estado atual (mapeado)
Dominio
- [x] Entidades base de cliente existem em `src/CustomerPlatform.Domain/Entities/Customer.cs`.
- [x] Entidades PF/PJ separadas por arquivo em `src/CustomerPlatform.Domain/Entities`.
- [x] Pastas Enums, ValueObjects, Events e Exceptions existem.

Aplicacao
- [ ] `src/CustomerPlatform.Application` nao existe.

Infraestrutura
- [x] Projeto `src/CustomerPlatform.Infrastructure` existe (vazio).
- [ ] Persistencia, message bus, deduplicacao, search e DI estao faltando.

API
- [x] `src/CustomerPlatform.Api` existe com HealthController e Swagger.
- [x] Decisao de interface: REST.
- [ ] Endpoints de clientes, validacoes e observabilidade estao faltando.

Worker
- [ ] `src/CustomerPlatform.Worker` nao existe.

Testes
- [x] `tests/CustomerPlatform.Tests` existe (legado).
- [x] `tests/CustomerPlatform.UnitTests` existe.
- [x] `tests/CustomerPlatform.IntegrationTests` existe (setup inicial).

Docker
- [x] Pasta `docker/` com compose files existe.
- [x] `docker-compose.exemplo.yml` existe.

Docs
- [x] README.md, DESAFIO.md, TEMPLATE_ENTREGA.md, ESTRUTURA_PROJETO.md existem.
- [ ] CRITERIOS_AVALIACAO.md, DECISOES_TECNICAS.md, COMO_EXECUTAR.md faltam.
- [x] PROMPTS_UTILIZADOS.md existe.
- [x] ARQUITETURA.md existe.
- [x] PLANEJAMENTO.md existe.