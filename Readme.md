# Documenta��o da API - Customer & ATM

Esta API fornece funcionalidades para gerenciamento de clientes e simula��o de saques (payout combinations) em um caixa eletr�nico (ATM).

## Sum�rio

- [Vis�o Geral](#vis�o-geral)
- [Requisitos](#requisitos)
- [Instala��o e Execu��o](#instala��o-e-execu��o)
- [Endpoints](#endpoints)
  - [CustomerController](#customercontroller)
    - [GET /api/customer](#get-apicustomer)
    - [POST /api/customer](#post-apicustomer)
  - [AtmController](#atmcontroller)
    - [GET /api/atm/payout/{amount}](#get-apiatmpayoutamount)
- [Notas Adicionais](#notas-adicionais)

## Vis�o Geral

API desenvolvida em .NET 8, utilizando Entity Framework Core com banco de dados SQLite, para as seguintes funcionalidades:

- **CustomerController**: Gerenciamento de clientes.
- **AtmController**: Simula��o de combina��es para saques em caixa eletr�nico.

## Requisitos

- .NET 8
- SQLite
- ASP.NET Core Runtime

## Instala��o e Execu��o

1. **Clone o reposit�rio:**
   ```bash
   git clone <url-do-reposit�rio>
   ```

2. **Restaure as depend�ncias e execute a aplica��o:**
   ```bash
   cd <diret�rio-da-api>
   dotnet restore
   dotnet run
   ```

3. **API dispon�vel em:** `http://localhost:5000`

## Endpoints

### CustomerController

#### GET `/api/customer`

Retorna uma lista filtrada de clientes.

**Par�metros (opcionais):**
- `filters`: Crit�rios para filtrar clientes.

**Respostas:**
- `200 OK`: Clientes retornados com sucesso.
- `204 No Content`: Nenhum cliente encontrado.

#### POST `/api/customer`

Cria novos clientes.

**Corpo da Requisi��o:**
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
- `400 Bad Request`: Erro ao processar requisi��o.
- `207 Multi-Status`: Requisi��o parcialmente conclu�da com notifica��es.

### AtmController

#### GET `/api/atm/payout/{amount}`

Simula combina��es de notas para saque.

**Par�metros:**
- `amount`: Valor desejado para saque.

**Exemplo:** `/api/atm/payout/230`

**Respostas:**
- `200 OK`: Retorna combina��es poss�veis.
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

- `400 Bad Request`: Valor inv�lido ou indispon�vel.
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
- Swashbuckle.AspNetCore (6.6.2) para documenta��o Swagger

### Testes

- **Carga:** k6
- **Unit�rios:** xUnit, FluentAssertions, Moq, coverlet.collector
- **Integra��o:** Microsoft.AspNetCore.Mvc.Testing, xUnit, FluentAssertions, coverlet.collector

### Patterns

- Valida��es e notifica��es s�o tratadas com Notification Pattern.
- API desenvolvida com pr�ticas REST e tratamento consistente de erros.

### Algoritmos

- **ATM:** Algoritmo simples que percorre combina��es de denomina��es para encontrar combina��es v�lidas de saque.
- **Customer:** Ordena��o dos clientes por sobrenome e nome utilizando algoritmo MergeSort.

---





# Testes de Carga com k6 - API Customer

Este reposit�rio cont�m scripts de teste de carga para os endpoints **POST** e **GET** do seu servi�o de clientes, utilizando o [k6](https://k6.io/).

## Pr�-requisitos

- [Node.js](https://nodejs.org/) (para executar os scripts k6 se voc� optar por us�-los via npm ou diretamente via k6)
- [k6](https://k6.io/docs/getting-started/installation) instalado na sua m�quina
- A API rodando localmente (ex.: `http://localhost:5000`)

## Estrutura dos Arquivos

- **config.js**  
  Cont�m a configura��o base, par�metros HTTP, e op��es de carga (VUs e dura��o) para os testes.

- **payload.js**  
  Gera dinamicamente uma lista de 50 clientes. Os nomes s�o selecionados aleatoriamente a partir dos arrays de _firstNames_ e _lastNames_.  
  Cada cliente possui:  
  - **id**: Sequencial  
  - **firstName** e **lastName**: Nomes aleat�rios  
  - **age**: Valor aleat�rio entre 18 e 90

- **post-customers-test.js**  
  Script para testar o endpoint **POST** (`/api/customer`). Envia o payload de clientes e valida o status da resposta (200 ou 400).

- **get-customers-test.js**  
  Script para testar o endpoint **GET** (`/api/customer`). Envia uma requisi��o GET com filtros como Id, Age, Name em par�metro para simular diferentes cen�rios. Valida o status da resposta (200 ou 204).

## Como Rodar os Testes

### 1. Executando o Teste POST

Abra o terminal e execute:

```bash
k6 run post-customers-test.js

### 2. Executando o Teste GET

Abra o terminal e execute:

```bash
k6 run get-customers-test.js
