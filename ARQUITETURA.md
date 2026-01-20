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
- xUnit (testes)
- Nao usar: AutoMapper, FluentAssertions, MassTransit

Decisao de interface
- API REST

Estrutura alvo (com separacao por pastas)
Desafio-IA-DotNet/
│   ├── docker/                                  # Infra local, observabilidade e servicos de apoio
│   │   ├── env/                                 # Variaveis de ambiente e exemplos
│   │   ├── observability/                       # Stack de logs/metricas/traces
│   │   ├── services/                            # Banco, mensageria, busca
│   │   ├── docker-compose.yaml                  # Orquestracao principal
│   │   ├── docker-compose-api.yaml              # API isolada
│   │   └── docker-compose-worker.yaml           # Worker isolado
│   ├── src/
│   │   ├── CustomerPlatform.Domain/             # Regras de negocio puras
│   │   │   ├── Entities/                         # Entidades do dominio (PF/PJ)
│   │   │   ├── Enums/                            # Enumeracoes do dominio
│   │   │   ├── ValueObjects/                     # CPF/CNPJ, Email, Telefone, Endereco
│   │   │   ├── Events/                           # ClienteCriado, ClienteAtualizado, DuplicataSuspeita
│   │   │   └── Exceptions/                       # Regras e validacoes de negocio
│   │   ├── CustomerPlatform.Application/        # Casos de uso e orquestracao
│   │   │   ├── Abstractions/                     # Contratos de portas
│   │   │   │   ├── Repositories/                 # Interfaces de repositorios
│   │   │   │   ├── MessageBus/                   # Interfaces de publicacao/consumo
│   │   │   │   ├── Search/                       # Interface do motor de busca
│   │   │   │   └── UnitOfWork/                   # Contratos transacionais
│   │   │   ├── Cqrs/
│   │   │   │   ├── Commands/                     # Escrita
│   │   │   │   ├── Queries/                      # Leitura
│   │   │   │   └── Notifications/                # Eventos de aplicacao
│   │   │   ├── DTOs/                             # Modelos de entrada/saida
│   │   │   ├── Validators/                       # Validacoes de entrada
│   │   │   └── DependencyInjections/             # Registro de dependencias da camada
│   │   ├── CustomerPlatform.Infrastructure/      # Adaptadores externos
│   │   │   ├── Context/                           # DbContext, mapeamentos e migrations
│   │   │   ├── Repositories/                      # Implementacoes EF Core
│   │   │   ├── MessageBus/                        # RabbitMQ publisher/consumer
│   │   │   ├── Search/                            # ElasticSearch indexacao/consulta
│   │   │   ├── Deduplicacao/                      # Similaridade, score e suspeitas
│   │   │   ├── Observability/                     # Serilog/OpenTelemetry config
│   │   │   └── DependencyInjections/              # Registro de dependencias da infraestrutura
│   │   ├── CustomerPlatform.Api/                 # Camada de apresentacao REST
│   │   │   ├── Controllers/                       # Endpoints REST
│   │   │   ├── Middleware/                        # ProblemDetails, correlationId
│   │   │   ├── DependencyInjections/              # Registro de dependencias da API
│   │   │   ├── Program.cs                         # Bootstrap
│   │   │   └── appsettings.json                   # Configuracoes
│   │   └── CustomerPlatform.Worker/              # Processamento assincrono
│   │       ├── Consumers/                         # Consumo de eventos
│   │       ├── HostedServices/                    # BackgroundService
│   │       ├── DependencyInjections/              # Registro de dependencias do worker
│   │       ├── Observability/                     # Tracing/logs
│   │       └── Program.cs                         # Bootstrap
│   ├── tests/
│   │   ├── CustomerPlatform.UnitTests/           # Testes unitarios (Domain/Application)
│   │   └── CustomerPlatform.IntegrationTests/    # DB, RabbitMQ, Search
│   ├── CustomerPlatform.sln
│   ├── docker-compose.exemplo.yml
│   ├── nuget.config
│   ├── .gitignore
│   ├── AGENTS.md
│   ├── README.md
│   ├── DESAFIO.md
│   ├── CRITERIOS_AVALIACAO.md
│   ├── TEMPLATE_ENTREGA.md
│   ├── DECISOES_TECNICAS.md
│   ├── COMO_EXECUTAR.md
│   ├── ARQUITETURA.md
│   └── PLANEJAMENTO.md

Dependencias entre camadas (direcao)
- Domain: sem dependencias externas.
- Application: depende de Domain e define portas (abstracoes).
- Infrastructure: implementa portas definidas em Application/Domain.
- API/Worker: dependem de Application e Infrastructure para orquestrar casos de uso.

Fluxos principais
- Cadastro: API REST -> Command Handler -> Domain -> Repository/DbContext -> Event -> MessageBus.
- Busca: API REST -> Query Handler -> Search/DB -> retorno paginado e ordenado por relevancia.
- Deduplicacao: Event Consumer (Worker) -> Deduplicacao + Search -> grava suspeitas -> publica DuplicataSuspeita.

Mensageria e eventos
- Eventos: ClienteCriado, ClienteAtualizado, DuplicataSuspeita.
- RabbitMQ para publicacao/consumo com retries e idempotencia.

Observabilidade
- Serilog com logs estruturados e correlationId.
- OpenTelemetry para logs, metricas e traces.
- Aspire Dashboard para visualizar traces e metricas.

Resiliencia
- Polly com retry, timeout e circuit breaker para chamadas externas (busca/mensageria).
- Consumers idempotentes e tratamento de falhas.

Testes
- Unitarios: regras de dominio e handlers da aplicacao.
- Integracao: PostgreSQL, RabbitMQ e motor de busca via Ductus.FluentDocker.
