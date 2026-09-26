\# CommerceOS



CommerceOS is a modular e-commerce backend API built with \*\*ASP.NET Core and .NET 10\*\*.



The project demonstrates a production-style backend architecture with authentication, authorization, product management, shopping carts, inventory management, orders, payments, caching, background processing, logging, and global exception handling.



\## Features



\- JWT Authentication

\- Role-Based Authorization

\- User Management

\- Customer Management

\- Product Management

\- Product Variant Management

\- Shopping Cart

\- Inventory Management

\- Inventory Reservations

\- Order Management

\- Payment Management

\- Redis Caching

\- Background Workers

\- Global Exception Handling

\- Serilog Logging

\- Entity Framework Core

\- MySQL

\- Swagger / OpenAPI



\## Architecture



CommerceOS follows a layered architecture:



```text

CommerceOS

│

├── CommerceOS.Api

├── CommerceOS.Application

├── CommerceOS.Domain

└── CommerceOS.Infrastructure

```



\### CommerceOS.Domain



Contains the core business model:



\- Entities

\- Enums

\- Base entities

\- Domain exceptions



\### CommerceOS.Application



Contains application/business logic:



\- DTOs

\- Interfaces

\- Application services

\- Authentication services

\- Background job abstractions



\### CommerceOS.Infrastructure



Contains infrastructure implementations:



\- Entity Framework Core

\- MySQL persistence

\- Repository implementations

\- Database configurations

\- EF Core migrations

\- Redis caching

\- Background workers



\### CommerceOS.Api



Contains the HTTP API layer:



\- Controllers

\- JWT authentication

\- Authorization

\- Middleware

\- Global exception handling

\- Swagger/OpenAPI

\- Serilog configuration



\## Tech Stack



| Technology | Purpose |

|---|---|

| C# | Programming language |

| .NET 10 | Application platform |

| ASP.NET Core Web API | REST API |

| Entity Framework Core | ORM / data access |

| MySQL | Relational database |

| Redis | Caching |

| JWT | Authentication |

| Serilog | Application logging |

| Swagger / OpenAPI | API documentation |

| Git | Version control |

| GitHub | Source control / hosting |



\## API Modules



CommerceOS currently provides API endpoints for:



```text

Authentication

Customers

Products

Product Variants

Cart

Inventory

Inventory Reservations

Orders

Payments

```



\## Authentication



The API uses \*\*JWT Bearer Authentication\*\*.



The application also supports role-based authorization, including:



```text

User

Admin

```



Protected endpoints can require an authenticated user or a specific role.



\## Background Processing



CommerceOS includes background processing for asynchronous operations.



Background components include:



\- Order background worker

\- Inventory reservation expiration worker

\- Background job queue



\## Caching



Redis is used as the caching infrastructure.



This allows frequently accessed data to be cached instead of repeatedly querying the database.



\## Logging



The application uses \*\*Serilog\*\* with:



\- Console logging

\- File logging



Application logs are stored locally during development and should not be committed to source control.



\## Exception Handling



The API contains centralized exception handling through:



\- Global exception handler

\- Global exception middleware



This provides consistent API error responses instead of exposing unhandled exceptions directly to clients.



\## Database



CommerceOS uses \*\*Entity Framework Core\*\* with MySQL.



Database migrations are located in:



```text

CommerceOS.Infrastructure/Migrations

```



\## Getting Started



\### Prerequisites



Install:



\- .NET 10 SDK

\- MySQL

\- Redis

\- Visual Studio 2022 or later



\### Clone the Repository



```bash

git clone https://github.com/Mohammedkhalandar/CommerceOS.git

cd CommerceOS

```



\### Restore Dependencies



```bash

dotnet restore

```



\### Build



```bash

dotnet build

```



\### Configure Local Settings



Create your local database connection and JWT configuration using environment variables or local development configuration.



Do \*\*not\*\* commit:



\- Database passwords

\- JWT secrets

\- API keys

\- Other credentials



An example configuration file can be used as a reference without containing real secrets.



\### Run the API



```bash

cd CommerceOS.Api

dotnet run

```



Open the Swagger URL shown in the terminal.



\## Example Project Structure



```text

CommerceOS

│

├── CommerceOS.Api

│   ├── Controllers

│   ├── ExceptionHandling

│   ├── Middleware

│   ├── Program.cs

│   ├── appsettings.json

│   └── CommerceOS.Api.csproj

│

├── CommerceOS.Application

│   ├── BackgroundJobs

│   ├── DTOs

│   ├── Interfaces

│   ├── Services

│   └── CommerceOS.Application.csproj

│

├── CommerceOS.Domain

│   ├── Common

│   ├── Entities

│   ├── Enums

│   ├── Exceptions

│   └── CommerceOS.Domain.csproj

│

├── CommerceOS.Infrastructure

│   ├── BackgroundJobs

│   ├── Caching

│   ├── Migrations

│   ├── Persistence

│   └── CommerceOS.Infrastructure.csproj

│

├── CommerceOS.slnx

└── README.md

```



\## Development



The project was developed using:



\- Visual Studio

\- .NET CLI

\- MySQL Workbench

\- Swagger UI

\- Git

\- GitHub



\## Project Status



\*\*Portfolio-ready backend project\*\*



The project demonstrates practical experience with:



\- Clean layered architecture

\- REST API development

\- Authentication and authorization

\- Database design

\- Entity Framework Core

\- Repository pattern

\- Service layer

\- Caching

\- Background processing

\- Logging

\- Exception handling

\- API documentation

\- Git and GitHub



\## Author



\*\*Mohammed Khalandar\*\*



GitHub:  

https://github.com/Mohammedkhalandar

