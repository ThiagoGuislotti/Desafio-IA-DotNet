# Prompts Utilizados

## Prompt #1

### Contexto/Objetivo
Definir arquitetura alvo, alinhar instruções de IA e criar planejamento em fases com checklist.

### Ferramenta Utilizada
Codex (OpenAI)

### Prompt Utilizado
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

---
```

### Resultado Obtido
Criação dos documentos ARQUITETURA.md e PLANEJAMENTO.md, com definição da stack, separação de camadas e planejamento em fases, além do ajuste das instruções para uso de IA.

### Refinamentos Necessarios
Nao se aplica

### Avaliação Pessoal
- [x] Bom - fiz pequenos ajustes

---

## Prompt #2

### Contexto/Objetivo
Implementar a Fase 2 (Domain) com organizacao do dominio, value objects, eventos, excecoes e testes unitarios/integracao basicos, conforme o planejamento.

### Ferramenta Utilizada
Codex (OpenAI)

### Prompt Utilizado
```
## Implementação do Domain

## Contexto Geral

Este prompt corresponde à **Fase 2 do desafio**, conforme definido no `PLANEJAMENTO.md`.

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

---
```

### Resultado Obtido
Implementação do Domain com entidades PF/PJ, Value Objects, enums, events e exceções, criação dos projetos de testes unitários e de integração com estrutura inicial, atualização da arquitetura e do planejamento, e alinhamento das validações e testes com os padrões adotados nos projetos de referência do NetToolsKit.

### Refinamentos Necessários
1- Ajustadas as instruções globais para padronizar simplicidade de código, documentação XML e regras de validação conforme os projetos de referência.
2- Simplificado o domínio com remoção da reidratação explícita e centralização da geração de identificadores.
3- Ajustadas as validações de Email, Telefone, CPF e CNPJ seguindo os padrões já consolidados no NetToolsKit.
4- Introduzidos Value Objects imutáveis e refinada a estrutura de testes para refletir a arquitetura.
5- Aprimorada a estratégia de testes unitários com uso intensivo de TestCase para aumentar cobertura com menor duplicação de código.
6- Simplificado o modelo de exceções do domínio, consolidando validações em exceções genéricas e reduzindo especializações desnecessárias.

### Avaliação Pessoal
- [x] Bom – fiz pequenos ajustes

---