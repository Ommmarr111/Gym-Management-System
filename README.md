# 🏋️ Gym Management System

A comprehensive RESTful API for gym management built with **ASP.NET Core Web API** and **Entity Framework Core**, featuring clean architecture, advanced business logic, and production-ready patterns.

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
- [API Endpoints](#-api-endpoints)
- [Getting Started](#-getting-started)
- [Database Schema](#-database-schema)
- [Design Patterns](#-design-patterns)
- [Business Rules](#-business-rules)

---

## 🎯 Overview

This system manages the complete lifecycle of a gym business, including:
- **Multi-location management** (gym branches)
- **Member registration and subscriptions**
- **Payment tracking**
- **Access control and check-ins**
- **Membership lifecycle** (Active, Frozen, Expired, Cancelled)

Built with clean architecture principles and industry-standard patterns, this project demonstrates production-ready code suitable for enterprise environments.

---

## ✨ Features

### Core Features
- ✅ **Gym Management** - Create and manage multiple gym locations with capacity limits
- ✅ **Member Management** - Register members, track join dates, and manage profiles
- ✅ **Membership Plans** - Flexible pricing plans with different durations per gym
- ✅ **Subscription Management** - Full lifecycle support (Active → Frozen → Expired → Cancelled)
- ✅ **Payment Tracking** - Automatic payment records with transaction references
- ✅ **Attendance System** - Check-in validation with subscription verification
- ✅ **Freeze/Unfreeze** - Pause memberships with automatic end-date extension

### Advanced Features
- ✅ **Capacity Management** - Prevent gym overcrowding and enforce member limits
- ✅ **Email Uniqueness** - Prevent duplicate member registrations
- ✅ **Soft Deletes** - Preserve data integrity with logical deletion
- ✅ **Global Query Filters** - Automatic filtering of deleted records
- ✅ **Transaction Safety** - Subscription + Payment created atomically

---

## 💼 Business Model

### Member Journey
```
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
6. Option to Freeze (vacation, injury)
   ↓
7. Renew or Let Expire
```

### Subscription Lifecycle
```
Pending → Active → Frozen → Active → Expired
                      ↓
                  Cancelled
```

### Key Business Rules
- 🚫 Cannot delete gym with active members
- 🚫 Cannot delete membership plan with active subscriptions
- 🚫 Cannot add member if gym is at capacity
- 🚫 Cannot reduce gym capacity below current member count
- 🚫 Cannot check in with frozen, expired, or cancelled subscription
- 🚫 Email addresses must be unique across all members
- ✅ Frozen subscriptions automatically extend end date
- ✅ Payment automatically created with each subscription

---

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core Web API 9.0
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Validation:** FluentValidation
- **Mapping:** AutoMapper

### Architecture & Patterns
- Clean Architecture (Application/Domain/Infrastructure layers)
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection
- Global Exception Handling
- Custom Exception Types

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns across four layers:

```
GymManagementSystem/
│
├── 📁 GymManagementSystem.Api/              (Presentation Layer)
│   ├── Controllers/                         → API endpoints
│   ├── Middleware/                          → Exception handlers
│   ├── Models/                              → View models (if any)
│   ├── Program.cs                           → Application entry point
│   └── appsettings.json                     → Configuration
│
├── 📁 GymManagementSystem.Application/      (Application Layer)
│   ├── DTOs/                                → Data Transfer Objects
│   ├── Exceptions/                          → Custom exception types
│   ├── Interfaces/                          → Service & Repository contracts
│   ├── Mappings/                            → AutoMapper profiles
│   ├── Services/                            → Business logic implementation
│   └── Validators/                          → FluentValidation rules
│
├── 📁 GymManagementSystem.Domain/           (Domain Layer)
│   └── Entities/                            → Domain models (Gym, Member, etc.)
│
└── 📁 GymManagementSystem.Infrastructure/   (Infrastructure Layer)
    ├── Migrations/                          → EF Core migrations
    ├── Persistence/                         → DbContext
    ├── Repositories/                        → Data access implementation
    └── Seeding/                             → Initial data (if any)
```
---

## 📡 API Endpoints

### Gyms
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/gyms` | Get all gyms |
| GET | `/api/gyms/{id}` | Get gym by ID |
| POST | `/api/gyms` | Create new gym |
| PUT | `/api/gyms/{id}` | Update gym |
| DELETE | `/api/gyms/{id}` | Delete gym (soft) |

### Members
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/members` | Get all members |
| GET | `/api/members/{id}` | Get member by ID |
| POST | `/api/members` | Register new member |
| PUT | `/api/members/{id}` | Update member |
| DELETE | `/api/members/{id}` | Delete member (soft) |

### Membership Plans
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/plans` | Get all plans |
| GET | `/api/plans/{id}` | Get plan by ID |
| GET | `/api/plans/gym/{gymId}` | Get plans for specific gym |
| POST | `/api/plans` | Create new plan |
| PUT | `/api/plans/{id}` | Update plan |
| DELETE | `/api/plans/{id}` | Delete plan (soft) |

### Subscriptions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/subscriptions` | Get all subscriptions |
| GET | `/api/subscriptions/{id}` | Get subscription by ID |
| GET | `/api/subscriptions/member/{memberId}` | Get member's subscriptions |
| POST | `/api/subscriptions` | Create subscription + payment |
| POST | `/api/subscriptions/{id}/cancel` | Cancel subscription |
| POST | `/api/subscriptions/{id}/freeze` | Freeze subscription |
| POST | `/api/subscriptions/{id}/unfreeze` | Unfreeze subscription |

### Attendance
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/attendance/check-in` | Check in to gym |
| GET | `/api/attendance/gym/{gymId}` | Get gym attendance history |
| GET | `/api/attendance/history/{memberId}` | Get member attendance history |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/gym-management-system.git
cd gym-management-system
```

2. **Update connection string**

Edit `appsettings.json` in the API project:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=GymMS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. **Apply migrations**
```bash
dotnet ef database update --project Infrastructure --startup-project API
```

4. **Run the application**
```bash
dotnet run --project API
```

5. **Open Swagger UI**
```
https://localhost:5219/swagger
```

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
- Soft delete support

**Subscription**
- Links member to plan
- Tracks status (Active, Frozen, Expired, Cancelled)
- Supports freezing with duration tracking
- Auto-calculates end date

**Payment**
- Created automatically with subscriptions
- Tracks payment method, status, and transaction reference
- Immutable audit trail

**Attendance**
- Records gym check-ins
- Validates active subscription
- Tracks check-in timestamps

### Entity Relationships
```
Gym 1:N MembershipPlan
Gym 1:N Member
Member 1:N Subscription
MembershipPlan 1:N Subscription
Subscription 1:N Payment
Member 1:N Attendance
Gym 1:N Attendance
```

---

## 🎨 Design Patterns

### Repository Pattern
Abstracts data access logic, providing a collection-like interface for domain entities.

### Unit of Work Pattern
Maintains a list of objects affected by a business transaction and coordinates writing changes to the database.

**Example:**
```csharp
// Create subscription + payment in one transaction
await _unitOfWork.Subscriptions.AddAsync(subscription);
await _unitOfWork.SaveChangesAsync(); // Get subscription.Id

var payment = new Payment { SubscriptionId = subscription.Id, ... };
await _unitOfWork.Payments.AddAsync(payment);
await _unitOfWork.SaveChangesAsync(); // Atomic operation
```

### Global Exception Handling
Centralized exception handling using `IExceptionHandler` (.NET 9):
- `NotFoundException` → 404
- `ValidationException` → 400
- `BusinessRuleException` → 422
- `UnauthorizedException` → 401
- `ForbiddenException` → 403
- Default → 500

### AutoMapper
Eliminates repetitive mapping code:
```csharp
// Before
return new MemberDto
{
    Id = member.Id,
    FirstName = member.FirstName,
    // ... 10 more lines
};

// After
return _mapper.Map<MemberDto>(member);
```

### FluentValidation
Separates validation logic from business logic:
```csharp
RuleFor(x => x.Email)
    .NotEmpty()
    .EmailAddress()
    .WithMessage("Valid email is required");
```

---

## 📊 Business Rules

### Gym Management
- ✅ Gym capacity cannot be reduced below current member count
- ✅ Gyms with active members cannot be deleted
- ✅ Deleted gyms are filtered from all queries

### Member Management
- ✅ Email addresses must be unique
- ✅ Members can only be added if gym has available capacity
- ✅ Age must be 18+ years
- ✅ Members cannot be deleted if they have active subscriptions

### Subscription Management
- ✅ One active subscription per member per plan
- ✅ Frozen subscriptions extend end date by freeze duration
- ✅ Only active subscriptions can be frozen
- ✅ Only frozen subscriptions can be unfrozen
- ✅ Freeze duration limited to 90 days
- ✅ Payment automatically created with subscription

### Attendance
- ✅ Check-in requires active subscription
- ✅ Subscription must be for the gym being accessed
- ✅ Frozen, expired, or cancelled subscriptions are rejected
- ✅ Check-in time is validated against gym operating hours (coming soon)

---

## 🔜 Roadmap

- [ ] JWT Authentication & Authorization
- [ ] Pagination support for list endpoints
- [ ] Advanced filtering and search
- [ ] Gym operating hours validation
- [ ] Multi-gym access control (Premium plans)
- [ ] Email notifications
- [ ] Reporting dashboard
- [ ] Export data (CSV/Excel)

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Your Name**
- GitHub: [@Ommmarr111](https://github.com/Ommmarr111)
- LinkedIn: [Omar Ahmed](https://linkedin.com/in/omarahmed111)

---

## 🙏 Acknowledgments

Built with modern .NET practices and clean architecture principles. Special focus on:
- Production-ready code quality
- Comprehensive business logic
- Industry-standard design patterns
- Scalable architecture

---

⭐ **Star this repository if you find it helpful!**
