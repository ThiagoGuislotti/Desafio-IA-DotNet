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
├── CRITERIOS_AVALIACAO.md
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
* CRITERIOS_AVALIACAO.md

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

### Avaliacao Pessoal
- [x] Bom - fiz pequenos ajustes

---