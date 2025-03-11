# API Documentation - Customer & ATM

This API provides functionalities for customer management and ATM payout simulations.

## Table of Contents

- [Overview](#overview)
- [Requirements](#requirements)
- [Installation and Execution](#installation-and-execution)
- [Endpoints](#endpoints)
  - [CustomerController](#customercontroller)
    - [GET /api/customer](#get-apicustomer)
    - [POST /api/customer](#post-apicustomer)
  - [AtmController](#atmcontroller)
    - [GET /api/atm/payout/{amount}](#get-apiatmpayoutamount)
- [Additional Notes](#additional-notes)

## Overview

API developed using .NET 8, Entity Framework Core with SQLite database, providing:

- **CustomerController**: Customer management.
- **AtmController**: ATM payout combination simulations.

## Requirements

- .NET 8
- SQLite
- ASP.NET Core Runtime

## Installation and Execution

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   ```

2. **Restore dependencies and run the application:**
   ```bash
   cd <api-directory>
   dotnet restore
   dotnet run
   ```

3. **API available at:** `http://localhost:5000`

## Endpoints

### CustomerController

#### GET `/api/customer`

Returns a filtered list of customers.

**Optional Parameters:**
- `filters`: Criteria to filter customers.

**Responses:**
- `200 OK`: Customers successfully returned.
- `204 No Content`: No customers found.

#### POST `/api/customer`

Creates new customers.

**Request Body:**
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

**Responses:**
- `200 OK`: Customers successfully created.
- `400 Bad Request`: Error processing request.
- `207 Multi-Status`: Request partially completed with notifications.

### AtmController

#### GET `/api/atm/payout/{amount}`

Simulates combinations of notes for withdrawal.

**Parameters:**
- `amount`: Desired withdrawal amount.

**Example:** `/api/atm/payout/230`

**Responses:**
- `200 OK`: Returns possible combinations.
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

- `400 Bad Request`: Invalid or unavailable amount.
  ```json
  {"Message": "Invalid amount. Unable to dispense the requested value with available denominations."}
  ```

## Additional Notes
### Technologies and Packages

- .NET Core 8
- Entity Framework Core (SQLite)
- FluentValidation (11.10.0)
- FluentValidation.AspNetCore (11.3.0)
- LinqKit (1.3.8)
- Swashbuckle.AspNetCore (6.6.2) for Swagger documentation

### Testing

- **Load:** k6
- **Unit:** xUnit, FluentAssertions, Moq, coverlet.collector
- **Integration:** Microsoft.AspNetCore.Mvc.Testing, xUnit, FluentAssertions, coverlet.collector

### Patterns

- Validations and notifications handled via Notification Pattern.
- API built following REST best practices with consistent error handling.

### Algorithms

- **ATM:** Simple algorithm iterating through denomination combinations to find valid payout combinations.
- **Customer:** Customers sorted by last name and first name using MergeSort algorithm.

---

# Load Testing with k6 - Customer API

This repository contains load-testing scripts for the **POST** and **GET** endpoints of your customer service, using [k6](https://k6.io/).

## Prerequisites

- [Node.js](https://nodejs.org/) (optional for running k6 scripts via npm)
- [k6](https://k6.io/docs/getting-started/installation) installed locally
- API running locally (e.g., `http://localhost:5000`)

## File Structure

- **config.js**  
  Contains base configuration, HTTP parameters, and load options (VUs and duration).

- **payload.js**  
  Dynamically generates a list of 50 customers. Names are randomly selected from arrays of _firstNames_ and _lastNames_.  
  Each customer has:  
  - **id**: Sequential  
  - **firstName** and **lastName**: Random names  
  - **age**: Random age between 18 and 90

- **post-customers-test.js**  
  Script for testing the **POST** endpoint (`/api/customer`). Sends customer payload and validates response status (200 or 400).

- **get-customers-test.js**  
  Script for testing the **GET** endpoint (`/api/customer`). Sends GET request with filters like Id, Age, Name to simulate scenarios. Validates response status (200 or 204).

## How to Run Tests

### 1. Executing the POST Test

Open terminal and run:

```bash
k6 run post-customers-test.js
```

### 2. Executing the GET Test

Open terminal and run:

```bash
k6 run get-customers-test.js
```

