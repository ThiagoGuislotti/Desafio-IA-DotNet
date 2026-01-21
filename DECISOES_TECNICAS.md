## Fase 1 – Planejamento

- Definição da arquitetura alvo e separação clara entre Domain, Application, Infrastructure, API e Worker.
- Planejamento incremental do desenvolvimento, priorizando Domain antes das camadas externas.
- Definição das diretrizes para uso de IA, organização do código e documentação.

## Fase 2 – Domain

- Implementação de um domínio rico com entidades, Value Objects imutáveis, eventos e exceções.
- Centralização das regras de negócio e validações no Domain.
- Garantia de consistência e invariantes de negócio desde a origem dos dados.
- Criação de testes unitários do domínio e setup inicial de testes de integração.

## Fase 3 – Application

- Implementação da camada Application aplicando CQRS com MediatR.
- Uso de Unit of Work e repositórios como abstrações, mantendo desacoplamento da infraestrutura.
- Validações simples de entrada na Application, delegando regras profundas ao Domain.
- Handlers organizados por entidade, uso de Result para retorno padronizado.
- Criação de testes unitários para handlers e validações.

## Fase 4 – Infrastructure (inclui Worker)

- Implementação da persistência com PostgreSQL e Entity Framework Core.
- Adoção do padrão Outbox para garantir consistência eventual entre banco e mensageria.
- Implementação de RabbitMQ para eventos assíncronos.
- Implementação do ElasticSearch como read model para buscas probabilísticas.
- Execução assíncrona de deduplicação via Worker.
- Configuração de observabilidade com Serilog e OpenTelemetry (Aspire Dashboard).
- Ajuste de docker-compose com versões fixas e healthchecks obrigatórios.
- Criação de testes de integração usando NUnit e Ductus.FluentDocker.

## Fase 5 – API

- Implementação da API REST para cadastro, atualização e busca de clientes.
- Integração da API com a camada Application.
- Padronização de erros, health checks e observabilidade.