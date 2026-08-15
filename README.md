# 🏋️ Gym Management System

A comprehensive **RESTful API for gym management** built with **ASP.NET Core Web API 9.0** and **Entity Framework Core**, designed as a practical backend engineering project with Clean Architecture, explicit business rules, secure authentication, efficient database querying, API protection, and automated integration testing.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4)](https://learn.microsoft.com/en-us/ef/core/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Business Model](#-business-model)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Authentication & Security](#-authentication--security)
- [Pagination, Searching, Filtering & Sorting](#-pagination-searching-filtering--sorting)
- [Rate Limiting](#-rate-limiting)
- [Testing](#-testing)
- [Getting Started](#-getting-started)
- [Database Schema](#-database-schema)
- [Design Patterns](#-design-patterns)
- [Business Rules](#-business-rules)
- [Development Notes](#-development-notes)
- [Roadmap](#-roadmap)
- [License](#-license)
- [Author](#-author)

---

## 🎯 Overview

The Gym Management System models the core lifecycle of a real gym business, including:

- **Multi-location management** — gym branches and capacity limits
- **Member registration and management**
- **Membership plans** — gym-specific pricing and durations
- **Subscription lifecycle** — Active, Frozen, Expired, Cancelled
- **Payment tracking** — payments associated with subscriptions
- **Attendance and check-ins** — validated against active subscriptions
- **Authentication and authorization** — ASP.NET Core Identity, JWT, and roles
- **Session security** — short-lived access tokens and rotating refresh tokens
- **API protection** — rate limiting
- **Efficient collection APIs** — pagination, searching, filtering, and sorting
- **Automated integration testing** — API and middleware behavior verification

The project is intentionally more than a CRUD application. It focuses on applying backend engineering concepts to realistic problems such as **business-rule enforcement, database-efficient querying, authentication state management, API abuse protection, and testable application architecture**.

---

## ✨ Features

### Core Features

- ✅ **Gym Management** — Create and manage multiple gym locations with capacity limits
- ✅ **Member Management** — Register members, track join dates, and manage profiles
- ✅ **Membership Plans** — Flexible pricing plans with different durations per gym
- ✅ **Subscription Management** — Full lifecycle support (Active → Frozen → Expired → Cancelled)
- ✅ **Payment Tracking** — Payment records with transaction references
- ✅ **Attendance System** — Check-in validation with subscription verification
- ✅ **Freeze/Unfreeze** — Pause memberships with automatic end-date extension
- ✅ **Soft Deletes** — Preserve records while excluding deleted data from normal queries
- ✅ **Global Query Filters** — Automatic filtering of soft-deleted entities where configured

### API & Data Access

- ✅ **Pagination** — Generic `PagedResult<T>` response with pagination metadata
- ✅ **Searching** — Query collection endpoints using search terms
- ✅ **Filtering** — Filter collection data using request parameter objects
- ✅ **Sorting** — Dynamic sorting of collection results
- ✅ **Database-side Querying** — Uses `IQueryable` and deferred execution so filtering, sorting, `Skip()`, and `Take()` are translated to SQL instead of loading the complete dataset into memory
- ✅ **Request Encapsulation** — Query parameters are grouped into dedicated request/parameter objects such as `MemberRequestParams`

### Authentication & Security

- ✅ **ASP.NET Core Identity** — User management and role management
- ✅ **JWT Authentication** — Short-lived bearer access tokens
- ✅ **Role-Based Authorization** — Admin, Manager, and Receptionist roles
- ✅ **Dual-Token Architecture** — Short-lived access token + long-lived refresh token
- ✅ **Cryptographically Secure Refresh Tokens** — Generated using `RandomNumberGenerator`
- ✅ **Refresh Token Hashing** — SHA-256 hashes are stored instead of raw refresh tokens
- ✅ **Refresh Token Rotation** — Each successful refresh revokes the old refresh token and issues a new one
- ✅ **Token Revocation** — Refresh-token sessions can be invalidated through persisted state
- ✅ **Refresh Token Expiration** — Server-side validation of refresh-token lifetime

### API Protection & Testing

- ✅ **Rate Limiting** — ASP.NET Core rate limiting middleware protects endpoints from excessive requests
- ✅ **Integration Testing** — xUnit tests using `WebApplicationFactory`
- ✅ **Middleware Testing** — Verifies rate-limited requests are rejected with `429 Too Many Requests` before reaching the controller pipeline
- ✅ **Global Exception Handling** — Centralized mapping of application exceptions to HTTP responses

---

## 💼 Business Model

### Member Journey

```text
1. Member Registration
       ↓
2. Choose Membership Plan
       ↓
3. Make Payment → Subscription Created (Active)
       ↓
4. Check In to Gym (validated against subscription)
       ↓
5. Use Gym Facilities
       ↓
6. Option to Freeze (vacation, injury, etc.)
       ↓
7. Renew or Let Expire
```

### Subscription Lifecycle

```text
Pending → Active → Frozen → Active → Expired
                       ↓
                    Cancelled
```

### Authentication Lifecycle

```text
Login / Register
      ↓
Access Token + Refresh Token
      ↓
Protected API Requests
      ↓
Access Token Expires
      ↓
Refresh Token Request
      ↓
Old Refresh Token Revoked
      ↓
New Access Token + New Refresh Token
```

---

## 🛠️ Tech Stack

### Backend

- **Framework:** ASP.NET Core Web API 9.0
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Authentication:** ASP.NET Core Identity + JWT Bearer Authentication
- **Validation:** FluentValidation
- **Mapping:** AutoMapper

### Architecture & Patterns

- Clean Architecture
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection
- DTO-based API contracts
- Envelope Pattern (`PagedResult<T>`)
- Global Exception Handling
- Custom Exception Types
- `IQueryable` / Deferred Execution

### Testing & API Protection

- xUnit
- `Microsoft.AspNetCore.Mvc.Testing` / `WebApplicationFactory`
- Integration Testing
- ASP.NET Core Rate Limiting

---

## 🏗️ Architecture

The project follows **Clean Architecture** with clear separation of responsibilities and dependency direction.

```text
GymManagementSystem/
│
├── 📁 GymManagementSystem.Api/              (Presentation Layer)
│   ├── Controllers/                         → HTTP API endpoints
│   ├── Middleware/                          → Exception handling / pipeline concerns
│   ├── Models/                              → API-specific models, if any
│   ├── Program.cs                           → Application entry point / DI configuration
│   └── appsettings.json                     → Application configuration
│
├── 📁 GymManagementSystem.Application/      (Application Layer)
│   ├── DTOs/                                → API request/response contracts
│   ├── Exceptions/                          → Custom application exceptions
│   ├── Extensions/                          → Query / IQueryable extensions
│   ├── Interfaces/                          → Service and repository contracts
│   ├── Mappings/                            → AutoMapper profiles
│   ├── Services/                            → Application/business orchestration
│   └── Validators/                          → FluentValidation rules
│
├── 📁 GymManagementSystem.Domain/           (Domain Layer)
│   └── Entities/                            → Core domain entities
│
├── 📁 GymManagementSystem.Infrastructure/   (Infrastructure Layer)
│   ├── Migrations/                          → EF Core migrations
│   ├── Persistence/                         → DbContext / EF Core configuration
│   ├── Repositories/                        → Repository implementations
│   └── Seeding/                             → Roles and development seed data
│
└── 📁 Tests/                                (Testing)
    └── GymManagementSystem.Tests/            → Automated tests
```

### Dependency Direction

```text
API
 ↓
Application
 ↓
Domain

Infrastructure ──implements──> Application contracts
```

The Application layer defines abstractions such as repositories and Unit of Work contracts. Infrastructure provides their EF Core implementations.

---

## 📂 Project Structure

### API Layer

Responsible for the HTTP boundary:

- Controllers
- Middleware
- Request handling
- HTTP responses
- Dependency injection configuration

### Application Layer

Contains application use cases and contracts:

- DTOs
- Services
- Repository interfaces
- Unit of Work interface
- Validators
- Application exceptions
- Query extensions

### Domain Layer

Contains the core business entities and concepts without depending on infrastructure concerns.

Examples include:

```text
Gym
Member
MembershipPlan
Subscription
Payment
Attendance
RefreshToken
ApplicationUser
```

### Infrastructure Layer

Contains implementation details such as:

- EF Core `ApplicationDbContext`
- SQL Server persistence
- Repository implementations
- Unit of Work implementation
- ASP.NET Core Identity persistence
- EF Core migrations
- Role/user seeding

---

## 📡 API Endpoints

> **Note:** Collection endpoints that support querying can receive pagination, searching, filtering, and sorting parameters according to their request parameter model.
>
> Example:
>
> `?PageNumber=1&PageSize=10&SearchTerm=Ahmed`

### Authentication

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| POST | `/api/auth/register` | Register a new user and issue authentication tokens |
| POST | `/api/auth/login` | Authenticate a user and issue authentication tokens |
| POST | `/api/auth/refresh-token` | Rotate the refresh token and issue a new token pair |

### Gyms

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| GET | `/api/gyms` | Get all gyms (Paginated) |
| GET | `/api/gyms/{id}` | Get gym by ID |
| POST | `/api/gyms` | Create new gym |
| PUT | `/api/gyms/{id}` | Update gym |
| DELETE | `/api/gyms/{id}` | Delete gym (soft) |

### Members

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| GET | `/api/members` | Get members (Paginated, Searchable, Filterable, Sortable) |
| GET | `/api/members/{id}` | Get member by ID |
| POST | `/api/members` | Register new member |
| PUT | `/api/members/{id}` | Update member |
| DELETE | `/api/members/{id}` | Delete member (soft) |

### Membership Plans

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| GET | `/api/plans` | Get all plans (Paginated) |
| GET | `/api/plans/{id}` | Get plan by ID |
| GET | `/api/plans/gym/{gymId}` | Get plans for specific gym |
| POST | `/api/plans` | Create new plan |
| PUT | `/api/plans/{id}` | Update plan |
| DELETE | `/api/plans/{id}` | Delete plan (soft) |

### Subscriptions

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| GET | `/api/subscriptions` | Get all subscriptions (Paginated) |
| GET | `/api/subscriptions/{id}` | Get subscription by ID |
| GET | `/api/subscriptions/member/{memberId}` | Get member's subscriptions |
| POST | `/api/subscriptions` | Create subscription + payment |
| POST | `/api/subscriptions/{id}/cancel` | Cancel subscription |
| POST | `/api/subscriptions/{id}/freeze` | Freeze subscription |
| POST | `/api/subscriptions/{id}/unfreeze` | Unfreeze subscription |

### Attendance

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| POST | `/api/attendance/check-in` | Check in to gym |
| GET | `/api/attendance/gym/{gymId}` | Get gym attendance history (Paginated) |
| GET | `/api/attendance/history/{memberId}` | Get member attendance history (Paginated) |

---

## 🔐 Authentication & Security

The API uses **ASP.NET Core Identity** for user/role management and **JWT Bearer Authentication** for protecting API endpoints.

### Roles

The current seeded roles are:

```text
Admin
Manager
Receptionist
```

Roles are included as JWT role claims and can be used with ASP.NET Core authorization.

### Dual-Token Architecture

The authentication system separates API authorization from session renewal:

| Token | Purpose | Lifetime | Stored in DB |
|---|---|---:|---|
| Access Token | Authorize API requests | ~15 minutes | No |
| Refresh Token | Obtain a new token pair | 7 days | SHA-256 hash only |

The access token is intentionally short-lived, while the refresh token provides a controlled way to establish a new authenticated session.

### Refresh Token Generation

Refresh tokens are generated using a cryptographically secure random number generator:

```text
RandomNumberGenerator.GetBytes(64)
          ↓
     Raw refresh token
          ↓
      SHA-256 hash
          ↓
    Store hash in DB
```

The **raw token is returned only to the client**. The database stores the hash rather than the usable token value.

### Refresh Token Rotation

Refresh tokens are single-use.

A successful refresh performs the following:

```text
Client sends raw refresh token
            ↓
Hash incoming token
            ↓
Find matching TokenHash
            ↓
Validate existence / expiration / revocation
            ↓
Find associated user
            ↓
Revoke old refresh token
            ↓
Generate new access token
            ↓
Generate new refresh token
            ↓
Hash and persist new refresh token
            ↓
Return new access + refresh tokens
```

Therefore, after a successful refresh:

```text
Old Refresh Token → Revoked
New Refresh Token → Active
```

Trying to reuse the old token returns `401 Unauthorized`.

### Token Revocation

Refresh-token records contain state that allows the application to determine whether a token is active:

```text
IsActive = !IsRevoked && !IsExpired
```

The old token is revoked during rotation and the new token is persisted through the `IRefreshTokenRepository` and `IUnitOfWork`.

### Authorization Header

Protected requests use:

```http
Authorization: Bearer <access-token>
```

---

## 📄 Pagination, Searching, Filtering & Sorting

Collection endpoints use query parameters to shape database queries before executing them.

### Request Encapsulation

Instead of placing many query parameters directly in controller methods, parameters are grouped into dedicated request models such as:

```text
MemberRequestParams
```

This keeps controller signatures clean and provides a single place for collection-query configuration.

### Generic Pagination Envelope

The API uses a generic `PagedResult<T>` response to return both data and pagination metadata.

Typical metadata includes:

- Total count
- Current page
- Page size
- Total pages
- Next/previous-page information where applicable

Example conceptually:

```json
{
  "items": [],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10
}
```

### Database-Efficient Execution

Repositories expose `IQueryable` for collection queries where query shaping is required.

The application can compose:

```text
Search
  ↓
Filter
  ↓
Sort
  ↓
Count
  ↓
Skip
  ↓
Take
  ↓
Execute SQL
```

This is important because the complete dataset does **not** need to be loaded into application memory before pagination is applied.

For example:

```csharp
query
    .Where(...)
    .OrderBy(...)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize);
```

Entity Framework Core translates the composed query into SQL so the database performs the filtering and pagination.

---

## 🚦 Rate Limiting

The API uses **ASP.NET Core rate limiting middleware** to restrict excessive requests and protect endpoints from abusive traffic.

Depending on the endpoint/configuration, the application can use rate-limiting algorithms such as:

- Token Bucket
- Sliding Window

A request that exceeds the configured limit is rejected with:

```http
429 Too Many Requests
```

### Why it matters

Rate limiting provides a first layer of protection against scenarios such as:

- Brute-force authentication attempts
- Excessive API calls
- Accidental request storms
- Resource exhaustion caused by uncontrolled clients

Rate limiting is implemented at the middleware/pipeline level, so rejected requests can be stopped before reaching the controller logic.

---

## 🧪 Testing

The project uses **xUnit** and ASP.NET Core integration testing infrastructure.

### Integration Testing

Tests use `WebApplicationFactory` to run the API through a realistic application pipeline rather than testing controllers as isolated methods only.

This allows the project to verify behavior involving:

- Routing
- Middleware
- Dependency injection
- Authentication infrastructure
- Application services
- HTTP responses

### Rate Limiting Integration Test

A rate-limiting integration test verifies that excessive requests are rejected by the middleware and return:

```http
429 Too Many Requests
```

The important behavior being tested is that the rejected request is short-circuited **before reaching the controller pipeline**.

### Testing Direction

The test suite is being expanded as the API surface and application behavior continue to grow.

---

## 🗄️ Database Schema

### Core Entities

**Gym**

- Manages gym locations with capacity limits
- Tracks name, address, phone, and capacity

**Member**

- Stores member information
- Links to home gym
- Tracks join date and contact details

**MembershipPlan**

- Defines pricing and duration
- Gym-specific plans
- Soft-delete support

**Subscription**

- Links member to plan
- Tracks status (Active, Frozen, Expired, Cancelled)
- Supports freezing with duration tracking
- Calculates/updates membership end date according to business rules

**Payment**

- Created automatically with subscriptions
- Tracks payment method, status, and transaction reference
- Provides a persisted payment record for the subscription workflow

**Attendance**

- Records gym check-ins
- Validates active subscription
- Tracks check-in timestamps

**RefreshToken**

- Links a refresh-token session to an `ApplicationUser`
- Stores a SHA-256 token hash rather than the raw token
- Tracks creation and expiration timestamps
- Tracks revocation state

### Entity Relationships

```text
Gym 1:N MembershipPlan
Gym 1:N Member
Gym 1:N Attendance

Member 1:N Subscription
Member 1:N Attendance

MembershipPlan 1:N Subscription
Subscription 1:N Payment

ApplicationUser 1:N RefreshToken
```

---

## 🎨 Design Patterns

### Repository Pattern

The Repository Pattern abstracts persistence operations behind application-defined interfaces.

For example:

```text
IRefreshTokenRepository
        ↓
RefreshTokenRepository
        ↓
ApplicationDbContext
        ↓
SQL Server
```

The Application layer depends on the contract, while Infrastructure contains the EF Core implementation.

### Unit of Work Pattern

`IUnitOfWork` coordinates repositories and commits changes through a single `SaveChangesAsync()` operation.

Refresh-token rotation demonstrates why this is useful:

```text
Old RefreshToken → Revoked
        +
New RefreshToken → Added
        ↓
SaveChangesAsync()
```

The same pattern is used for business workflows involving multiple persistence operations, such as subscription and payment creation.

### Envelope Pattern

`PagedResult<T>` standardizes collection responses and keeps pagination metadata separate from the returned item collection.

### Dependency Injection

Services depend on abstractions such as:

```csharp
IUnitOfWork
IRefreshTokenRepository
```

Infrastructure implementations are supplied through ASP.NET Core dependency injection.

### Global Exception Handling

Centralized exception handling maps application exceptions to consistent HTTP responses:

| Exception | HTTP |
|---|---:|
| `NotFoundException` | 404 |
| `ValidationException` | 400 |
| `BusinessRuleException` | 422 |
| `UnauthorizedException` | 401 |
| `ForbiddenException` | 403 |
| Unhandled exception | 500 |

### AutoMapper

Used to reduce repetitive entity-to-DTO mapping where appropriate.

### FluentValidation

Keeps request validation rules separate from application service logic.

Example:

```csharp
RuleFor(x => x.Email)
    .NotEmpty()
    .EmailAddress()
    .WithMessage("Valid email is required");
```

---

## 📊 Business Rules

### Gym Management

- 🚫 Gym capacity cannot be reduced below the current member count
- 🚫 Gyms with active members cannot be deleted
- ✅ Deleted gyms are filtered from normal queries

### Member Management

- 🚫 Email addresses must be unique
- 🚫 Members cannot be added when gym capacity is full
- 🚫 Members with active subscriptions cannot be deleted
- ✅ Member age must satisfy the configured business rule

### Membership Plans

- 🚫 Membership plans with active subscriptions cannot be deleted
- ✅ Plans are associated with specific gyms

### Subscription Management

- 🚫 Only valid subscription states can transition between lifecycle operations
- 🚫 Only active subscriptions can be frozen
- 🚫 Only frozen subscriptions can be unfrozen
- 🚫 Freeze duration is limited to 90 days
- ✅ Frozen subscriptions automatically extend their end date
- ✅ Payment is automatically created with each subscription

### Attendance

- 🚫 Check-in requires an active subscription
- 🚫 Subscription must belong to the gym being accessed
- 🚫 Frozen, expired, or cancelled subscriptions cannot be used for check-in

### Authentication

- 🚫 Invalid credentials return `401 Unauthorized`
- 🚫 Missing, expired, or revoked refresh tokens cannot be used
- 🚫 A successfully consumed refresh token cannot be reused
- ✅ Successful refresh rotates the refresh token

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### 1. Clone the Repository

```bash
git clone https://github.com/Ommmarr111/gym-management-system.git
cd gym-management-system
```

### 2. Configure the Connection String

Edit the API configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=GymMS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Configure JWT

```json
{
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY",
    "Issuer": "GymMS_API",
    "Audience": "GymMS_Clients",
    "DurationInMinutes": "15"
  }
}
```

> ⚠️ Never commit real JWT secrets or database credentials. Use .NET User Secrets, environment variables, or a proper secret manager for sensitive configuration.

### 4. Apply Migrations

From the solution directory:

```bash
dotnet ef database update
```

If your solution requires explicit project selection:

```bash
dotnet ef database update --project GymManagementSystem.Infrastructure --startup-project GymManagementSystem.Api
```

### 5. Run the Application

```bash
dotnet run --project GymManagementSystem.Api
```

### 6. Open Swagger

```text
https://localhost:<port>/swagger
```

Swagger can be used to explore and manually test the API, including authentication and protected endpoints.

---

## 🌱 Development Seeding

The infrastructure contains role and development-user seeding.

### Roles

```text
Admin
Manager
Receptionist
```

Development users are seeded for these roles through the existing `RoleSeeder`.

> ⚠️ Seeded development credentials should never be reused in a production environment.

---

## 🔄 Authentication Example Flow

### Login

```text
POST /api/auth/login
        ↓
Validate credentials
        ↓
Generate JWT access token
        ↓
Generate cryptographically random refresh token
        ↓
Hash refresh token
        ↓
Store hash in database
        ↓
Return access + raw refresh token
```

### Refresh

```text
POST /api/auth/refresh-token
        ↓
Hash supplied refresh token
        ↓
Find matching database record
        ↓
Check expiration/revocation
        ↓
Find user
        ↓
Revoke old token
        ↓
Generate new access token
        ↓
Generate and persist new refresh token
        ↓
Return new pair
```

---

## 🧠 Engineering Highlights

The project has progressively evolved from core gym CRUD operations into a backend system that demonstrates several important engineering concerns:

### API Design

- RESTful endpoints
- DTO-based contracts
- Pagination
- Searching
- Filtering
- Sorting
- Standardized paginated responses

### Data Access

- Repository abstraction
- Unit of Work
- EF Core
- SQL Server
- `IQueryable`
- Deferred execution
- Database-side filtering and pagination

### Authentication & Security

- ASP.NET Core Identity
- JWT Bearer authentication
- Role-based authorization
- Short-lived access tokens
- Long-lived refresh tokens
- Cryptographically secure token generation
- SHA-256 refresh-token hashing
- Refresh-token rotation
- Stateful token revocation

### API Protection

- ASP.NET Core rate limiting
- `429 Too Many Requests` handling
- Middleware-level request rejection

### Testing

- xUnit
- `WebApplicationFactory`
- Integration testing
- Middleware/pipeline testing

### Maintainability

- Clean Architecture
- Separation of concerns
- Dependency Injection
- Custom exceptions
- Global exception handling
- FluentValidation
- AutoMapper

---

## 🗺️ Roadmap

The project is being developed incrementally toward a more production-oriented backend.

Potential next areas include:

- Expand integration and unit-test coverage across the remaining application surface
- Complete authorization policies and endpoint-level authorization rules
- Structured logging and correlation IDs
- Observability and health checks
- Docker/containerization
- Redis/caching
- Background processing
- API versioning
- Further database/query optimization
- Resilience and scalability improvements

These are future directions and are **not represented as currently implemented features**.

---

## 📝 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Omar Ahmed**

- GitHub: [@Ommmarr111](https://github.com/Ommmarr111)
- LinkedIn: [Omar Ahmed](https://linkedin.com/in/Ommmarr111)

---

## 🙏 Acknowledgments

Built as a practical backend engineering project with a focus on:

- Production-oriented .NET practices
- Clean Architecture
- Explicit business logic
- Secure authentication and session management
- Efficient database querying
- API protection
- Automated integration testing
- Maintainable and testable application design

---

⭐ **Star this repository if you find it useful!**
