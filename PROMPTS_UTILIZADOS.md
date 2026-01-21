# Prompts Utilizados

## Prompt #1

### 🎯 Contexto/Objetivo
Definir arquitetura alvo, alinhar instruções de IA e criar planejamento em fases com checklist.

### 🤖 Ferramenta Utilizada
Codex (OpenAI)

### 💬 Prompt Utilizado
```
## Análise e Planejamento do Desafio (Fase de Planejamento)

### Contexto Geral

Você deve analisar um projeto de DESAFIO técnico em .NET, cujo objetivo final é implementar os requisitos utilizando ferramentas de IA.

Neste prompt específico, sua função **não é implementar código**, mas **planejar, organizar e estruturar** o projeto conforme as diretrizes abaixo.
A implementação de código **será realizada em etapas posteriores**, por meio de novos prompts.

---

## Diretrizes Gerais do Planejamento

* O planejamento deve ser simples, direto e **sem exemplos de código**
* Não informar prazos
* Utilizar checklist de progresso no formato:

```
[ ] - Pendente
[x] - Feito
```

* O planejamento sempre começa pela camada **Domain** e segue para os projetos externos
  (Application → Infrastructure → API → Worker → Tests)
* O foco é arquitetura, organização e tomada de decisão técnica
* Não assumir nada fora do que foi explicitamente informado nos arquivos do desafio

---

## Estrutura de Arquitetura Obrigatória

A arquitetura do projeto deve ser ajustada exatamente para o formato abaixo:

```
├── .github/                            # Instruções para agentes de IA
├── docker/                             # Todos os arquivos de infraestrutura
├── src/
│   ├── CustomerPlatform.Api/            # API Web
│   ├── CustomerPlatform.Application/    # Camada de regras de negócio
│   ├── CustomerPlatform.Domain/         # Camada de domínio
│   ├── CustomerPlatform.Infrastructure/ # Camada de infraestrutura
│   └── CustomerPlatform.Worker/         # Worker / Processamentos assíncronos
├── tests/
│   ├── CustomerPlatform.IntegrationTests/ # Testes de integração
│   ├── CustomerPlatform.UnitTests/        # Testes unitários
│   └── CustomerPlatform.Tests/            # Projeto legado (remover)
│       └── CustomerTests.cs
├── CustomerPlatform.sln
├── docker-compose.exemplo.yml
├── nuget.config
├── .gitignore
├── AGENTS.md
├── README.md
├── DESAFIO.md
├── CRITERIOS_Avaliação.md
├── TEMPLATE_ENTREGA.md
├── DECISOES_TECNICAS.md
└── COMO_EXECUTAR.md
```

---

## Tecnologias Permitidas

### O que vamos usar

* EntityFrameworkCore
* MediatR 13.0.0
* RabbitMQ
* PostgreSQL
* Serilog
* OpenTelemetry (logs, métricas e traces)
* Aspire.Dashboard
* Ductus.FluentDocker
* Swagger
* Polly

### O que não vamos usar

* AutoMapper
* FluentAssertions
* MassTransit

---

## Referências Obrigatórias

Todas as decisões e alinhamentos devem respeitar estritamente os seguintes arquivos:

* AGENTS.md
* .github/copilot-instructions.md
* .github/instructions
* DESAFIO.md
* CRITERIOS_Avaliação.md

---

## Escopo Esperado da Análise

Considere o projeto localizado em:

```
C:\Users\tguis\Documents\Trabalho\Pessoal\Vagas\Localiza\Desafio-IA-DotNet
```

Você deve realizar exclusivamente as tarefas abaixo.

---

## 1. Ajustes de Instruções para IA

* Ajustar o conteúdo do:

  * AGENTS.md
  * .github/copilot-instructions.md
* As alterações devem refletir:

  * As regras deste prompt
  * O papel da IA nesta fase (planejamento)
* Basear-se somente nas diretivas fornecidas neste prompt

---

## 2. Arquitetura

* Criar o arquivo ARQUITETURA.md
* Descrever a arquitetura de forma simples
* Explicar:

  * Organização das camadas
  * Responsabilidade de cada projeto
  * Fluxo geral da aplicação
* Sem código
* Sem diagramas complexos

---

## 3. Planejamento

* Criar o arquivo PLANEJAMENTO.md
* Definir os passos de execução:

  * Em fases
  * Começando pelo Domain
  * Evoluindo até API, Worker e testes
  * Cada etapa com seus respectivos testes associados
* Utilizar checklist de progresso
* Não incluir prazos

---

## 4. Mapeamento do Estado Atual

* Analisar o que já existe no projeto
* Atualizar o PLANEJAMENTO.md indicando:

  * O que já está feito `[x]`
  * O que ainda está pendente `[ ]`
* Organizar o planejamento em fases claras e progressivas

---

## O que não deve ser feito

* Não escrever código neste prompt
* Não sugerir bibliotecas fora da lista
* Não inventar requisitos
* Não adicionar prazos
* Não simplificar a arquitetura além do solicitado

---

## Resultado Esperado

Ao final desta etapa de planejamento, o projeto deve conter:

* Instruções claras para agentes de IA
* Arquitetura documentada de forma objetiva
* Planejamento incremental, rastreável e auditável
* Clareza total do estado atual versus pendências
* Base sólida para execução assistida por IA em etapas posteriores

---

## Observação Final

Caso alguma informação necessária não esteja explícita nos arquivos de referência,
registre a incerteza no planejamento como pendência, sem inferir soluções.
```

### ✅ Resultado Obtido
Criação dos documentos ARQUITETURA.md e PLANEJAMENTO.md, com definição da stack, separação de camadas e planejamento em fases, além do ajuste das instruções para uso de IA.

### Refinamentos Necessarios
Criado alguns arquivos de referncias adicionais para melhor orientar a IA durante o desenvolvimento em .github.

### 📊 Avaliação Pessoal
- [ ] Excelente - usei diretamente sem modificações
- [x] Bom - fiz pequenos ajustes
- [ ] Regular - precisei modificar bastante
- [ ] Ruim - tive que refazer manualmente


---

## Prompt #2

### 🎯 Contexto/Objetivo
Implementar a Fase 2 (Domain) com organizacao do dominio, value objects, eventos, excecoes e testes unitarios/integracao basicos, conforme o planejamento.

### 🤖 Ferramenta Utilizada
Codex (OpenAI)

### 💬 Prompt Utilizado
```
## Implementação do Domain

## Contexto Geral

Este prompt corresponde à **Fase 2 do planejamento**, conforme definido no `PLANEJAMENTO.md`.

Nesta etapa, a IA **pode gerar código**, porém **exclusivamente na camada Domain e nos projetos de testes**, respeitando a arquitetura, o planejamento e as regras definidas no repositório.

---

## Objetivo da Fase

- Organizar a camada Domain, separando classes existentes em arquivos individuais.
- Implementar os componentes faltantes do domínio (ValueObjects, Enums, Events, Exceptions) se necessário.
- Criar e estruturar os projetos de testes unitários e de integração.
- Atualizar a documentação de arquitetura para refletir as decisões de testes.

---

## Regras Obrigatórias

- Gerar código **somente** em:
  - `src/CustomerPlatform.Domain`
  - `tests/CustomerPlatform.UnitTests`
  - `tests/CustomerPlatform.IntegrationTests`
- Não implementar persistência, mensageria, busca, API ou Worker.
- Domain não pode depender de EF Core, RabbitMQ, ElasticSearch ou qualquer SDK externo.
- Não antecipar responsabilidades de outras camadas.
- Seguir estritamente o que está definido em `ARQUITETURA.md` e `PLANEJAMENTO.md`.

---

## Escopo do Domain

### Refatoração de código existente

O projeto já possui entidades base de cliente no mesmo arquivo. Ajustar para:
- Um arquivo por classe:
  - `Customer.cs` (base/abstrata)
  - `ClientePessoaFisica.cs`
  - `ClientePessoaJuridica.cs`

---

### Estrutura mínima do Domain

Garantir a existência das pastas:

```
Entities/
ValueObjects/
Enums/
Events/
Exceptions/
```

---

### Implementações obrigatórias

#### Enums
- `TipoCliente`

#### ValueObjects
- `Documento` (CPF / CNPJ)
- `Email`
- `Telefone`
- `Endereco`

#### Events
- `ClienteCriado`
- `ClienteAtualizado`

#### Exceptions
- Exceções de negócio para validações inválidas

---

## Estrutura de Testes

### Testes Unitários (xUnit)

Projeto:

```
tests/CustomerPlatform.UnitTests/
  Assets/
  Tests/
    Domain/
      Entities/
      ValueObjects/
      Events/
```

Regras:
- `Assets/` deve conter helpers, mocks, builders e utilitários de teste.
- A estrutura em `Tests/Domain/` deve espelhar diretamente a organização do Domain.

Cobertura mínima:
- Validações e igualdade de ValueObjects
- Criação válida e inválida de entidades
- Consistência dos eventos de domínio

---

### Testes de Integração (NUnit)

Projeto:

```
tests/CustomerPlatform.IntegrationTests/
  Assets/
  Tests/
    Infrastructure/
```

Nesta fase, o projeto deve existir e estar configurado com NUnit, contendo ao menos um teste simples para validar o setup do runner.  
As integrações reais serão implementadas nas fases posteriores.

---

## Atualização de Documentação

- Atualizar `ARQUITETURA.md` para explicitar:
  - xUnit para testes unitários
  - NUnit para testes de integração
  - Estrutura de pastas de testes (Assets + Tests/Domain)
- Atualizar `PLANEJAMENTO.md`, marcando os itens concluídos da Fase 2.

---

## Resultado Esperado

Ao final desta fase, o projeto deve conter:

- Domain organizado e separado por arquivos
- Componentes completos do domínio (ValueObjects, Enums, Events, Exceptions)
- Projeto `CustomerPlatform.UnitTests` estruturado com xUnit
- Projeto `CustomerPlatform.IntegrationTests` estruturado com NUnit (vazio)
- Arquitetura atualizada refletindo as decisões de testes

---

## Observação Final

Caso alguma regra de negócio não esteja claramente definida nos documentos do desafio,  
registre a pendência no `PLANEJAMENTO.md` e **não implemente por suposição**.
```

### ✅ Resultado Obtido
Implementação do Domain com entidades PF/PJ, Value Objects, enums, events e exceções, criação dos projetos de testes unitários e de integração com estrutura inicial, atualização da arquitetura e do planejamento, e alinhamento das validações e testes com os padrões adotados nos projetos de referência do NetToolsKit.

### 🔄 Refinamentos Necessários
1- Ajustadas as instruções globais para padronizar simplicidade de código, documentação XML e regras de validação conforme os projetos de referência.
2- Simplificado o domínio com remoção da reidratação explícita e centralização da geração de identificadores.
3- Ajustadas as validações de Email, Telefone, CPF e CNPJ seguindo os padrões já consolidados no NetToolsKit.
4- Introduzidos Value Objects imutáveis e refinada a estrutura de testes para refletir a arquitetura.
5- Aprimorada a estratégia de testes unitários com uso intensivo de TestCase para aumentar cobertura com menor duplicação de código.
6- Simplificado o modelo de exceções do domínio, consolidando validações em exceções genéricas e reduzindo especializações desnecessárias.

### 📊 Avaliação Pessoal
- [ ] Excelente - usei diretamente sem modificações
- [ ] Bom - fiz pequenos ajustes
- [x] Regular - precisei modificar bastante
- [ ] Ruim - tive que refazer manualmente

---

## Prompt #3

### 🎯 Contexto/Objetivo
Implementar a Fase 3 (Application) aplicando CQRS e Unit of Work, com validações simples de entrada, handlers organizados por entidade e testes unitários, conforme o planejamento e a arquitetura definida.

### 🤖 Ferramenta Utilizada
Codex (OpenAI)

### 💬 Prompt Utilizado
```
# Implementacao da Camada Application (CQRS + Unit of Work + Testes)

## Contexto Geral

Este prompt corresponde a **Fase 3 do planejamento**, conforme definido no `PLANEJAMENTO.md`.

Nesta etapa, a IA pode gerar codigo, porem **exclusivamente** na camada **Application** e nos **testes unitarios** relacionados, respeitando a arquitetura, o planejamento e as instrucoes do repositorio.

---

## Referencias Obrigatorias (projetos do workspace)

Utilizar como referencia de padroes e estrutura os seguintes projetos:

- NetToolsKit\samples\src\Rent.Service.Application
- NetToolsKit\samples\tests\Rent.Service.UnitTests
- NetToolsKit\samples\tests\Rent.Service.IntegrationTests
- NetToolsKit\src\NetToolsKit.Data
- NetToolsKit\src\NetToolsKit.Data.EntityFrameworkCore

---

## Objetivo da Fase

- Criar o projeto `CustomerPlatform.Application` com estrutura de CQRS, abstracoes e DI.
- Implementar comandos, queries, handlers e validacoes.
- Implementar o padrao Unit of Work como abstracao.
- Criar testes unitarios para handlers e validacoes.
- Atualizar o planejamento e registrar este prompt.

---

## Regras Obrigatorias

- Gerar codigo somente em:
  - `src/CustomerPlatform.Application`
  - `tests/CustomerPlatform.UnitTests`
- Nao implementar infraestrutura nesta fase.
- Nao usar AutoMapper.
- Manter simplicidade e clareza.

---

## Estrutura Esperada

```
src/CustomerPlatform.Application/
  Abstractions/
  Cqrs/
  DTOs/
  Validators/
  DependencyInjections/
```

---

## Validacoes na Application (importante)

Nesta fase, os **Validators** devem validar apenas aspectos de entrada simples, como:
- campo obrigatorio (null, vazio, whitespace)
- limites minimos/maximos de tamanho
- consistencia trivial (ex.: paginacao > 0)

Nao implementar validacoes profundas de negocio (ex.: algoritmo de CPF/CNPJ, formato completo de email, normalizacao avancada de telefone).
Essas validacoes pertencem ao **Domain** (Value Objects e regras do dominio).

---

## Testes

- Framework: xUnit
- Priorizar uso de Theory/TestCase para maior cobertura com menos codigo.
- Usar **Moq** quando for necessario mockar abstracoes (repositorios, unit of work, message bus, search).
- Usar **Bogus** apenas se for util para gerar massa de dados consistente (evitar complexidade desnecessaria).

Cobertura minima:
- Validators: obrigatorio + limites simples
- Handlers: fluxo principal + falhas de validacao + falhas do dominio (quando Value Objects lancarem excecao)

---

## Documentacao

- Atualizar `PLANEJAMENTO.md`
- Registrar Prompt #3 em `PROMPTS_UTILIZADOS.md`
- Registrar decisoes tecnicas relevantes.

---

## Resultado Esperado

- Application estruturado com CQRS e UoW.
- Validacoes simples (obrigatorio/limites) e handlers testados.
- Base pronta para a Fase 4 (Infrastructure).
```

### ✅ Resultado Obtido
Criação do projeto CustomerPlatform.Application com CQRS, abstrações de leitura e escrita via Unit of Work, uso de Result para retorno de operações, handlers organizados por entidade, validações simples de entrada e testes unitários para validators e handlers, além da atualização do planejamento e registro das decisões técnicas.

### 🔄 Refinamentos Necessários
1- Ajustados contratos de Application para alinhar com o padrão do NetToolsKit (Result, Unit of Work responsável por criar repositórios e repositórios genéricos para leitura).
2- Incluída separação clara entre escrita no PostgreSQL e leitura via Elastic, com indexação assíncrona disparada por eventos.
3- Complementados comandos e handlers para suportar atualização de cliente e padronizada a organização de CQRS por entidade.
4- Ajustados testes unitários para maior cobertura com menor duplicação, utilizando mocks e dados gerados quando necessário.

### 📊 Avaliação Pessoal
- [ ] Excelente - usei diretamente sem modificações
- [ ] Bom - fiz pequenos ajustes
- [x] Regular - precisei modificar bastante
- [ ] Ruim - tive que refazer manualmente

---

## Prompt #4

### 🎯 Contexto/Objetivo
Implementar a Fase 4 (Infrastructure) com persistencia, mensageria, busca, deduplicacao, observabilidade, docker e testes de integracao.

### 🤖 Ferramenta Utilizada
Codex (OpenAI)

### 💬 Prompt Utilizado
```
# Implementação da Camada Infrastructure (PostgreSQL + RabbitMQ + Elastic + Observabilidade + Docker)

## Contexto Geral

Este prompt corresponde à **Fase 4 do planejamento**, conforme definido no `PLANEJAMENTO.md`.

Nesta etapa, a IA deve implementar a **camada Infrastructure**, incluindo:
- Persistência (PostgreSQL + EF Core)
- Mensageria (RabbitMQ)
- Read model de busca (ElasticSearch)
- Deduplicação assíncrona
- Observabilidade (Serilog + OpenTelemetry + Aspire Dashboard)
- Ajuste dos docker-compose para subir corretamente os serviços externos com **healthcheck** em todos os serviços
- Ajustar os docker-compose para utilizar versões fixas das imagens (NUNCA usar `latest`), garantindo reprodutibilidade do ambiente.
- Garantir resiliência nos comandos com uso de Polly (retry, timeout e fallback quando aplicável), validando o comportamento com testes de integração.

---

## Referências Obrigatórias (projetos do workspace)

Usar como referência de estrutura e padrões:

- `NetToolsKit\samples\src\Rent.Service.Infrastructure`
- `NetToolsKit\samples\tests\Rent.Service.IntegrationTests`

---

## Objetivo da Fase

- Implementar o `CustomerPlatform.Infrastructure` com:
  - DbContext, mappings, migrations e repositórios (PostgreSQL).
  - UnitOfWork concreto, criando repositórios internamente (padrão factory).
  - Publisher RabbitMQ para eventos de domínio.
  - Consumer/adapter para indexação no Elastic e deduplicação assíncrona.
  - Adapter de leitura para consultas via ElasticSearch (read model).
  - Configuração de Serilog e OpenTelemetry (OTLP) com visualização via Aspire Dashboard.
- Criar testes de integração (NUnit) para Postgres, RabbitMQ e Elastic, usando o setup global existente.
- Ajustar os docker-compose para provisionar todos serviços externos com healthchecks.
- Registrar este prompt como **Prompt #4** no `PROMPTS_UTILIZADOS.md`.

---

## Regras Obrigatórias

- Gerar código somente em:
  - `src/CustomerPlatform.Infrastructure`
  - `tests/CustomerPlatform.IntegrationTests`
  - `docker/**` (composes e arquivos auxiliares)
- Não criar regras de negócio nesta camada (apenas adapters e integrações).
- Implementar apenas as implementações concretas das abstrações da Application.
- Manter simplicidade e evitar complexidade desnecessária.
- Seguir `ARQUITETURA.md` e as decisões registradas em `DECISOES_TECNICAS.md`.

---

## Persistência (PostgreSQL)

- Implementar `DbContext` e mapeamentos:
  - Conversões para Value Objects.
  - Índices e unicidade para CPF/CNPJ.
- Implementar repositórios concretos para escrita/leitura conforme contratos da Application.
- Implementar `UnitOfWork` concreto:
  - Cria repositórios internamente.
  - Commit e transação quando necessário.

---

## Mensageria (RabbitMQ)

- Implementar publicação assíncrona dos eventos:
  - `ClienteCriado`
  - `ClienteAtualizado`
  - `DuplicataSuspeita` (quando aplicável)
- Garantir:
  - Publicação após commit.
  - Retry simples (Polly) e idempotência básica (eventId).

---

## ElasticSearch (Read Model)

- Implementar o compose do ElasticSearch (separado ou dentro do compose principal de infra).
- Implementar indexação assíncrona via consumo de eventos:
  - Upsert do documento no índice.
- Implementar leitura para buscas:
  - fuzzy por nome/razão social
  - filtros por email/telefone
  - paginação
  - ordenação por relevância (simplificada)

Observação: Elastic é read model. PostgreSQL continua como fonte de verdade.

---

## Deduplicação

- Implementar deduplicação assíncrona disparada por eventos (create/update).
- Buscar candidatos via Elastic e calcular score simples.
- Persistir suspeitas e publicar `DuplicataSuspeita` quando score >= threshold.
- Não realizar merge automático.

---

## Observabilidade

- Configurar Serilog (logs estruturados).
- Configurar OpenTelemetry (logs/métricas/traces).
- Exportar OTLP para Aspire Dashboard.

---

## Docker Compose (obrigatório nesta fase)

- Ajustar/organizar os arquivos de compose sob `docker/` para subir corretamente:
  - PostgreSQL
  - RabbitMQ
  - ElasticSearch (obrigatório criar o compose)
  - Aspire Dashboard (observabilidade)
- Todos os serviços em todos os composes devem ter **healthcheck**.
- Preferir composes separados por domínio quando fizer sentido (services/observability), mantendo consistência com `ARQUITETURA.md`.

---

## Testes de Integração (NUnit + Ductus.FluentDocker)

- Os testes de integração devem utilizar o arquivo global:
  - `tests/CustomerPlatform.IntegrationTests/Tests/GlobalSetup.cs`
- O `GlobalSetup.cs` deve ser usado para subir serviços externos necessários (Postgres/Rabbit/Elastic), evitando duplicação por teste.
- Criar testes mínimos cobrindo:
  - Persistência no PostgreSQL (inserir/consultar)
  - Publicação/consumo via RabbitMQ (fluxo básico)
  - Indexação e busca no Elastic (fluxo básico)

---

## Documentação Obrigatória

Ao finalizar:
- Atualizar `PLANEJAMENTO.md` (marcar itens da Fase 4).
- Registrar Prompt #4 no `PROMPTS_UTILIZADOS.md`.
- Registrar decisões relevantes no `DECISOES_TECNICAS.md`.

---

## Resultado Esperado

Ao final desta fase, o projeto deve conter:
- Infrastructure funcional (DB + Bus + Search + Dedup + Observability).
- docker-compose(s) corretos, completos e com healthchecks, incluindo compose do Elastic.
- Testes de integração executáveis usando `GlobalSetup.cs`.
- Base pronta para a Fase 5 (API e Worker).

---

## Observação Final

Caso algum detalhe técnico não esteja explicitamente definido no desafio,
registre a decisão em `DECISOES_TECNICAS.md` e não implemente por suposição.
```

### ✅ Resultado Obtido
Infraestrutura implementada com persistência (PostgreSQL + EF Core), mensageria (RabbitMQ), busca (ElasticSearch) e deduplicação, incluindo configuração de observabilidade (Serilog + OpenTelemetry + Aspire Dashboard). Docker-compose ajustados para subir corretamente os serviços externos, com imagens em versões fixas (sem `latest`), healthchecks em todos os serviços e compose específico do Elastic. Testes de integração (NUnit) estruturados para utilizar o `GlobalSetup.cs` como bootstrap dos serviços externos.

### 🔄 Refinamentos Necessários
1- Ajustado o fluxo para consistência eventual usando Outbox: registrar eventos no banco e publicar de forma assíncrona via Worker (PostgreSQL -> RabbitMQ -> Elastic).
2- Aplicada resiliência com Polly nas integrações externas (publicação RabbitMQ e chamadas ao Elastic), com validação por testes de integração.
3- Ajustados docker-compose para reprodutibilidade (versões fixas) e confiabilidade (healthcheck obrigatório em todos os serviços), incluindo compose dedicado do ElasticSearch.
4- Padronizado o setup de testes de integração usando `tests/CustomerPlatform.IntegrationTests/Tests/GlobalSetup.cs` para subir dependências externas e evitar duplicações.
5- Reorganização interna da Infrastructure para agrupar componentes de dados (Context/Mappings/Migrations, Repositories e UnitOfWork) sob `Data/` para navegação mais simples, mantendo Search/Messaging/Deduplication/Observability separados.

### 📊 Avaliação Pessoal
- [ ] Excelente - usei diretamente sem modificações
- [ ] Bom - fiz pequenos ajustes
- [x] Regular - precisei modificar bastante
- [ ] Ruim - tive que refazer manualmente

---
