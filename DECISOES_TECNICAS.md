## Fase 1 – Planejamento

- Definição da arquitetura alvo e separação clara entre Domain, Application, Infrastructure, API e Worker.
- Planejamento incremental do desenvolvimento, priorizando Domain antes das camadas externas.
- Definição das diretrizes para uso de IA, organização do código e documentação.

- **Trade-off:** optar por planejamento incremental reduziu risco de retrabalho, mas exigiu revisões frequentes dos prompts.
- **Justificativa:** priorizar clareza arquitetural e governança de IA foi mais importante do que avançar rapidamente no código.

## Fase 2 – Domain

- Implementação de um domínio rico com entidades, Value Objects imutáveis, eventos e exceções.
- Centralização das regras de negócio e validações no Domain.
- Garantia de consistência e invariantes de negócio desde a origem dos dados.
- Criação de testes unitários do domínio e setup inicial de testes de integração.

- **Trade-off:** adoção de domínio rico aumentou a quantidade de classes e validações.
- **Justificativa:** garantiu consistência e invariantes desde a origem, simplificando Application e Infrastructure posteriormente.

## Fase 3 – Application

- Implementação da camada Application aplicando CQRS com MediatR.
- Uso de Unit of Work e repositórios como abstrações, mantendo desacoplamento da infraestrutura.
- Validações simples de entrada na Application, delegando regras profundas ao Domain.
- Handlers organizados por entidade, uso de Result para retorno padronizado.
- Criação de testes unitários para handlers e validações.

- **Trade-off:** CQRS com abstrações (UoW/Repositórios) adicionou camadas intermediárias.
- **Justificativa:** desacoplamento da infraestrutura e melhor testabilidade compensaram a complexidade adicional.

## Fase 4 – Infrastructure (inclui Worker)

- Implementação da persistência com PostgreSQL e Entity Framework Core.
- Adoção do padrão Outbox para garantir consistência eventual entre banco e mensageria.
- Implementação de RabbitMQ para eventos assíncronos.
- Implementação do ElasticSearch como read model para buscas probabilísticas.
- Execução assíncrona de deduplicação via Worker.
- Configuração de observabilidade com Serilog e OpenTelemetry (Aspire Dashboard).
- Ajuste de docker-compose com versões fixas e healthchecks obrigatórios.
- Criação de testes de integração usando NUnit e Ductus.FluentDocker.

- **Trade-off:** adoção de consistência eventual (Outbox + Worker) e uso do ElasticSearch como read model adicionaram processamento assíncrono e latência ao fluxo.
- **Justificativa:** a abordagem aumentou significativamente a robustez e resiliência do sistema, além de permitir buscas probabilísticas e ordenação por relevância com melhor performance e escalabilidade.


## Fase 5 – API

- Implementação da API REST para cadastro, atualização e busca de clientes.
- Integração da API com a camada Application.
- Padronização de erros, health checks e observabilidade.
- Uso de middlewares para correlationId, logs e ProblemDetails.
- Atualização unificada via `PUT /customers/{id}` com escolha de PF/PJ pelo `CustomerType`.
- Testes de integração da API (cadastro/atualização/health) usando WebApplicationFactory.

- **Trade-off:** centralizar exceções e logs via middleware reduz controle pontual nos controllers.
- **Justificativa:** padronização de erros, observabilidade e menor duplicação de código.

## Fase 6 – Finalizacao

- README na raiz criado seguindo template e referencias a `docs/`.
- Exemplos de Swagger adicionados via SchemaFilter e OperationFilter (sem bibliotecas extras).
- Prometheus mantido como opcional; observabilidade principal via OTLP + Aspire Dashboard.
- Testes de integracao com fluxo completo do Worker e cenarios de deduplicacao com seed controlado.
- Threshold de deduplicacao nos testes ajustado para 0.6 para cobrir cenarios de nome + email/telefone.
- Deduplicacao consulta candidatos pelo nome (fuzzy) e aplica score para email/telefone, evitando filtros restritivos no Elastic.

- **Trade-off:** ampliar testes de deduplicação aumentou o tempo de execução dos testes.
- **Justificativa:** maior confiança no cálculo de score e no fluxo ponta a ponta (Postgres → Outbox → Rabbit → Elastic → API).