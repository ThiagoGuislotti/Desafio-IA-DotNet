# Arquitetura

Tecnologias e ferramentas (o que vamos usar)
- .NET 8
- Entity Framework Core + PostgreSQL
- MediatR 13.0.0 (CQRS)
- RabbitMQ (mensageria)
- ElasticSearch (busca probabilistica)
- Serilog (logs estruturados)
- OpenTelemetry (logs, metricas, traces)
- Aspire Dashboard (observabilidade)
- Polly (resiliencia)
- Swagger (documentacao da API)
- Ductus.FluentDocker (infra para testes de integracao)
- xUnit (testes unitarios)
- NUnit (testes de integracao)
- Nao usar: AutoMapper, FluentAssertions, MassTransit

Decisao de interface
- API REST

Estrutura alvo (com separacao por pastas)
Desafio-IA-DotNet/
├── docker/                                  # Infra local, observabilidade e servicos de apoio
│   ├── env/                                 # Variaveis de ambiente e exemplos
│   ├── observability/                       # Stack de logs/metricas/traces
│   ├── services/                            # Banco, mensageria, busca
│   ├── docker-compose.yaml                  # Orquestracao principal
│   ├── docker-compose-api.yaml              # API isolada
│   └── docker-compose-worker.yaml           # Worker isolado
├── src/
│   ├── CustomerPlatform.Domain/             # Regras de negocio puras
│   │   ├── Entities/                         # Entidades do dominio (PF/PJ)
│   │   ├── Enums/                            # Enumeracoes do dominio
│   │   ├── ValueObjects/                     # CPF/CNPJ, Email, Telefone, Endereco
│   │   ├── Events/                           # ClienteCriado, ClienteAtualizado, DuplicataSuspeita
│   │   └── Exceptions/                       # Regras e validacoes de negocio
│   ├── CustomerPlatform.Application/        # Casos de uso e orquestracao
│   │   ├── Abstractions/                     # Contratos de portas
│   │   │   ├── Repositories/                 # Interfaces de repositorios
│   │   │   ├── MessageBus/                   # Interfaces de publicacao/consumo
│   │   │   ├── Search/                       # Interface do motor de busca
│   │   │   └── UnitOfWork/                   # Contratos transacionais
│   │   ├── Cqrs/
│   │   │   ├── Commands/                     # Escrita
│   │   │   ├── Queries/                      # Leitura
│   │   │   └── Notifications/                # Eventos de aplicacao
│   │   ├── DTOs/                             # Modelos de entrada/saida
│   │   ├── Validators/                       # Validacoes de entrada
│   │   └── DependencyInjections/             # Registro de dependencias da camada
│   ├── CustomerPlatform.Infrastructure/      # Adaptadores externos
│   │   ├── Context/                           # DbContext, mapeamentos e migrations
│   │   ├── Repositories/                      # Implementacoes EF Core
│   │   ├── MessageBus/                        # RabbitMQ publisher/consumer
│   │   ├── Search/                            # ElasticSearch indexacao/consulta
│   │   ├── Deduplicacao/                      # Similaridade, score e suspeitas
│   │   ├── Observability/                     # Serilog/OpenTelemetry config
│   │   └── DependencyInjections/              # Registro de dependencias da infraestrutura
│   ├── CustomerPlatform.Api/                 # Camada de apresentacao REST
│   │   ├── Controllers/                       # Endpoints REST
│   │   ├── Middleware/                        # ProblemDetails, correlationId
│   │   ├── DependencyInjections/              # Registro de dependencias da API
│   │   ├── Program.cs                         # Bootstrap
│   │   └── appsettings.json                   # Configuracoes
│   └── CustomerPlatform.Worker/              # Processamento assincrono
│       ├── Consumers/                         # Consumo de eventos
│       ├── HostedServices/                    # BackgroundService
│       ├── DependencyInjections/              # Registro de dependencias do worker
│       ├── Observability/                     # Tracing/logs
│       └── Program.cs                         # Bootstrap
├── tests/
│   ├── CustomerPlatform.UnitTests/           # Testes unitarios (Domain/Application)
│   │   ├── Assets/                            # Helpers, mocks, builders e utilitarios
│   │   └── Tests/
│   │       └── Domain/
│   │           ├── Entities/
│   │           ├── ValueObjects/
│   │           └── Events/
│   └── CustomerPlatform.IntegrationTests/    # DB, RabbitMQ, Search
│       ├── Assets/                            # Helpers e utilitarios
│       └── Tests/
│           └── Infrastructure/
├── CustomerPlatform.sln
├── docker-compose.exemplo.yml
├── nuget.config
├── .gitignore
├── AGENTS.md
├── README.md
├── DESAFIO.md
├── CRITERIOS_AVALIACAO.md
├── TEMPLATE_ENTREGA.md
├── DECISOES_TECNICAS.md
├── COMO_EXECUTAR.md
├── ARQUITETURA.md
└── PLANEJAMENTO.md

Dependencias entre camadas:
- Domain: sem dependencias externas.
- Application: depende de Domain e define portas (abstracoes).
- Infrastructure: implementa portas definidas em Application/Domain.
- API/Worker: dependem de Application e Infrastructure para orquestrar casos de uso.

Fluxos principais:
- Cadastro: API REST -> Command Handler -> Domain -> Repository/DbContext -> Event -> MessageBus.
- Busca: API REST -> Query Handler -> Search/DB -> retorno paginado e ordenado por relevancia.
- Deduplicacao: Event Consumer (Worker) -> Deduplicacao + Search -> grava suspeitas -> publica DuplicataSuspeita.

MER:

Entidades (PostgreSQL):
- Customers
  - PK: customerId
  - Colunas principais: customerType, document, fullName/corporateName/tradeName, email, phone, birthDate, address*, createdAt, updatedAt
- OutboxEvents
  - PK: outboxEventId
  - Unico: eventId
  - Colunas principais: eventType, payload (jsonb), occurredAt, createdAt, processedAt, retryCount, lastError
- DuplicateSuspicions
  - PK: duplicateSuspicionId
  - Colunas principais: customerId, candidateCustomerId, score, reason, createdAt

Relacionamentos logicos:
- DuplicateSuspicions.customerId -> Customers.customerId
- DuplicateSuspicions.candidateCustomerId -> Customers.customerId
- OutboxEvents referencia o dominio via payload (sem FK).

Read model (ElasticSearch):
- Indice `customers`: id, customerType, document, name, tradeName, email, phone, address, createdAt, updatedAt.

Mensageria e eventos:
- Eventos: ClienteCriado, ClienteAtualizado, DuplicataSuspeita.
- RabbitMQ para publicacao/consumo com retries e idempotencia.

Observabilidade:
- Serilog com logs estruturados e correlationId.
- OpenTelemetry para logs, metricas e traces.
- Aspire Dashboard para visualizar traces e metricas.

Resiliencia:
- Polly com retry, timeout e circuit breaker para chamadas externas (busca/mensageria).
- Consumers idempotentes e tratamento de falhas.

Testes:
> Os testes devem refletir a organizacao dos projetos, mantendo um espelhamento direto entre pastas e camadas.

Estrutura comum:
- Assets/   # Helpers, mocks, builders e utilitarios de teste
- Tests/    # Testes organizados por camadas e funcionalidades
- Testes Unitarios
    - Projeto: `tests/CustomerPlatform.UnitTests`
    - Framework: xUnit
    - Objetivo: validar regras de negocio, entidades, value objects e eventos do dominio.
    - A estrutura em `Tests/` deve espelhar diretamente a organizacao do Domain e da Application.
- Testes de Integracao
    - Projeto: `tests/CustomerPlatform.IntegrationTests`
    - Framework: NUnit
    - Infraestrutura: PostgreSQL, RabbitMQ e motor de busca via Ductus.FluentDocker
    - Objetivo: validar integracoes entre camadas e dependencias externas (DB, mensageria e busca).