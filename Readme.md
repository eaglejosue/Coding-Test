# Documentação da API - Customer & ATM

Esta API fornece funcionalidades para gerenciamento de clientes e simulação de saques (payout combinations) em um caixa eletrônico (ATM).

## Sumário

- [Visão Geral](#visão-geral)
- [Requisitos](#requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Endpoints](#endpoints)
  - [CustomerController](#customercontroller)
    - [GET /api/customer](#get-apicustomer)
    - [POST /api/customer](#post-apicustomer)
  - [AtmController](#atmcontroller)
    - [GET /api/atm/payout/{amount}](#get-apiatmpayoutamount)
- [Notas Adicionais](#notas-adicionais)

## Visão Geral

API desenvolvida em .NET 8, utilizando Entity Framework Core com banco de dados SQLite, para as seguintes funcionalidades:

- **CustomerController**: Gerenciamento de clientes.
- **AtmController**: Simulação de combinações para saques em caixa eletrônico.

## Requisitos

- .NET 8
- SQLite
- ASP.NET Core Runtime

## Instalação e Execução

1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositório>
   ```

2. **Restaure as dependências e execute a aplicação:**
   ```bash
   cd <diretório-da-api>
   dotnet restore
   dotnet run
   ```

3. **API disponível em:** `http://localhost:5000`

## Endpoints

### CustomerController

#### GET `/api/customer`

Retorna uma lista filtrada de clientes.

**Parâmetros (opcionais):**
- `filters`: Critérios para filtrar clientes.

**Respostas:**
- `200 OK`: Clientes retornados com sucesso.
- `204 No Content`: Nenhum cliente encontrado.

#### POST `/api/customer`

Cria novos clientes.

**Corpo da Requisição:**
```json
[
  {
    "Id": 1,
    "FirstName": "Leia",
    "LastName": "Anderson",
    "Age": 25
  },
  {
    "Id": 2,
    "FirstName": "Carlos",
    "LastName": "Ray",
    "Age": 29
  }
]
```

**Respostas:**
- `200 OK`: Clientes criados com sucesso.
- `400 Bad Request`: Erro ao processar requisição.
- `207 Multi-Status`: Requisição parcialmente concluída com notificações.

### AtmController

#### GET `/api/atm/payout/{amount}`

Simula combinações de notas para saque.

**Parâmetros:**
- `amount`: Valor desejado para saque.

**Exemplo:** `/api/atm/payout/230`

**Respostas:**
- `200 OK`: Retorna combinações possíveis.
  ```json
  {
    "Message": "Combinations available:",
    "Result": [
      "23 x 10 EUR",
      "1 x 50 EUR + 18 x 10 EUR",
      "2 x 50 EUR + 13 x 10 EUR",
      "3 x 50 EUR + 8 x 10 EUR",
      "4 x 50 EUR + 3 x 10 EUR",
      "1 x 100 EUR + 13 x 10 EUR",
      "1 x 100 EUR + 1 x 50 EUR + 8 x 10 EUR",
      "1 x 100 EUR + 2 x 50 EUR + 3 x 10 EUR",
      "2 x 100 EUR + 3 x 10 EUR"
    ]
  }
  ```

- `400 Bad Request`: Valor inválido ou indisponível.
  ```json
  {"Message": "Invalid amount. Unable to dispense the requested value with available denominations."}
  ```


## Notas Adicionais
### Tecnologias e Pacotes

- .NET Core 8
- Entity Framework Core (SQLite)
- FluentValidation (11.10.0)
- FluentValidation.AspNetCore (11.3.0)
- LinqKit (1.3.8)
- Swashbuckle.AspNetCore (6.6.2) para documentação Swagger

### Testes

- **Carga:** k6
- **Unitários:** xUnit, FluentAssertions, Moq, coverlet.collector
- **Integração:** Microsoft.AspNetCore.Mvc.Testing, xUnit, FluentAssertions, coverlet.collector

### Patterns

- Validações e notificações são tratadas com Notification Pattern.
- API desenvolvida com práticas REST e tratamento consistente de erros.

### Algoritmos

- **ATM:** Algoritmo simples que percorre combinações de denominações para encontrar combinações válidas de saque.
- **Customer:** Ordenação dos clientes por sobrenome e nome utilizando algoritmo MergeSort.

---





# Testes de Carga com k6 - API Customer

Este repositório contém scripts de teste de carga para os endpoints **POST** e **GET** do seu serviço de clientes, utilizando o [k6](https://k6.io/).

## Pré-requisitos

- [Node.js](https://nodejs.org/) (para executar os scripts k6 se você optar por usá-los via npm ou diretamente via k6)
- [k6](https://k6.io/docs/getting-started/installation) instalado na sua máquina
- A API rodando localmente (ex.: `http://localhost:5000`)

## Estrutura dos Arquivos

- **config.js**  
  Contém a configuração base, parâmetros HTTP, e opções de carga (VUs e duração) para os testes.

- **payload.js**  
  Gera dinamicamente uma lista de 50 clientes. Os nomes são selecionados aleatoriamente a partir dos arrays de _firstNames_ e _lastNames_.  
  Cada cliente possui:  
  - **id**: Sequencial  
  - **firstName** e **lastName**: Nomes aleatórios  
  - **age**: Valor aleatório entre 18 e 90

- **post-customers-test.js**  
  Script para testar o endpoint **POST** (`/api/customer`). Envia o payload de clientes e valida o status da resposta (200 ou 400).

- **get-customers-test.js**  
  Script para testar o endpoint **GET** (`/api/customer`). Envia uma requisição GET com filtros como Id, Age, Name em parâmetro para simular diferentes cenários. Valida o status da resposta (200 ou 204).

## Como Rodar os Testes

### 1. Executando o Teste POST

Abra o terminal e execute:

```bash
k6 run post-customers-test.js

### 2. Executando o Teste GET

Abra o terminal e execute:

```bash
k6 run get-customers-test.js
