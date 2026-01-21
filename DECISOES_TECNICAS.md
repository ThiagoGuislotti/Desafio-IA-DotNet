# Decisoes Tecnicas

## Fase 3 - Application
- CQRS com MediatR 13 para comandos e consultas.
- Validacoes simples na Application; regras profundas permanecem no Domain.
- Abstracoes genericas IRepository/IQueryRepository e IUnitOfWork para isolar persistencia e transacoes.
- Repository criado via UnitOfWork (padrao factory) para reduzir acoplamento.
- Result/ValidationResult para padronizar retornos e erros dos handlers.
- Abstracoes para Elastic (busca), RabbitMQ (publicacao) e deduplicacao.
- Handlers consolidados por entidade (CustomerCommandHandler e CustomerQueryHandler).
- Mapeamento manual de entidades para DTOs (sem AutoMapper).
- Testes unitarios com xUnit e Moq para validators e handlers.
