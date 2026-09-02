# Gym Management System

A REST API for managing gym operations — members, subscriptions, payments, attendance — built with ASP.NET Core 9 and EF Core, following Clean Architecture with Repository + Unit of Work.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)](https://learn.microsoft.com/en-us/ef/core/)

---

## What this is

A gym has locations, members, membership plans, and subscriptions that move through a lifecycle (`Active → Frozen → Expired/Cancelled`). Members check in, which is validated against an active subscription at the correct gym. Collection endpoints (members, subscriptions, attendance) compose filters/search/pagination as `IQueryable`, so EF Core translates them to SQL instead of loading full tables into memory.

This is an internal operations API for gym staff — not a customer-facing app. There's no member login; members are records that staff create and manage.

---

## Tech stack

**API:** ASP.NET Core 9 · EF Core · SQL Server · ASP.NET Core Identity + JWT
**Validation/Mapping:** FluentValidation · AutoMapper
**Caching:** `IMemoryCache` (most endpoints) and `IDistributedCache`/Redis (gym-scoped plan lookups) — mixed as part of an in-progress migration, see Caching section
**Testing:** xUnit · `WebApplicationFactory`
**Protection:** ASP.NET Core rate limiting on `/api/auth/login`

---

## Architecture

```mermaid
graph TD
    Api["Api<br/>Controllers, exception handling"]
    App["Application<br/>DTOs, services, validators"]
    Domain["Domain<br/>Gym, Member, Subscription..."]
    Infra["Infrastructure<br/>EF Core, repositories, migrations"]

    Api --> App
    App --> Domain
    Infra -.implements.-> App

    style Api fill:#E6F1FB,stroke:#185FA5,color:#0C447C
    style App fill:#E6F1FB,stroke:#185FA5,color:#0C447C
    style Domain fill:#E6F1FB,stroke:#185FA5,color:#0C447C
    style Infra fill:#E1F5EE,stroke:#0F6E56,color:#085041
```

- **Api** — controllers, global exception handling middleware
- **Application** — DTOs, services, FluentValidation validators, repository/UoW interfaces, AutoMapper profiles
- **Domain** — entities: `Gym`, `Member`, `MembershipPlan`, `Subscription`, `Payment`, `Attendance`, `RefreshToken`
- **Infrastructure** — EF Core `ApplicationDbContext`, repository implementations, migrations, Identity persistence

---

### Auth

Login issues a short-lived **JWT access token** and a **refresh token**. `POST /api/auth/refresh` exchanges a valid refresh token for a new pair.

Refresh tokens are generated using `RandomNumberGenerator` and stored only as **SHA-256 hashes** — the raw token is never persisted.

#### Rotation & Concurrency Safety

Each refresh token is **single-use**. Refreshing performs an atomic conditional update:

`UPDATE ... WHERE Id = @id AND RevokedOn IS NULL AND ExpiresOn > <current UTC time>`

This prevents two concurrent requests from both successfully reusing the same token — only one can win the conditional update.

Token revocation is committed independently of new token issuance. If issuing the new token pair fails after the old token has been revoked, that revocation is **not rolled back** — a failed issuance never re-validates an already-revoked token.

#### Reuse Detection

If a refresh token that was already revoked (and not simply expired) is presented again, the system treats it as a potential token theft: **all active refresh tokens for that user are revoked**, forcing re-authentication.

**Note:** Revocation on reuse detection is user-wide, not session- or device-specific. If reuse is detected from one compromised session, all of that user's active sessions — including unaffected devices — are revoked.

#### Design Trade-offs

- **No session/family grouping:** Reuse detection cannot currently distinguish which session was compromised, so it revokes broadly rather than narrowly. This favors security over convenience.
- **Independent revocation:** A rare transient failure during token issuance (e.g. user lookup fails) can leave a revoked-but-not-replaced token, forcing the legitimate user to log in again. This is accepted to guarantee that revocation can never be silently undone.
Roles (`Admin`, `Manager`, `Receptionist`) are seeded and included as JWT claims. Endpoints are role-gated by what the action actually affects, not uniformly:

| Role | Can do |
|---|---|
| `Admin` | Everything, plus structural/destructive actions: delete a gym or plan, register new staff |
| `Manager` | Revenue-affecting actions: create plans, cancel/freeze/unfreeze subscriptions |
| `Receptionist` | Front-desk actions: enroll members, sell subscriptions, check members in |

Reads are open to any authenticated staff role. `/api/auth/login` is rate-limited to blunt credential-stuffing attempts.

### Entity relationships

```mermaid
erDiagram
    GYM ||--o{ MEMBER : hosts
    GYM ||--o{ MEMBERSHIP_PLAN : offers
    GYM ||--o{ ATTENDANCE : records
    MEMBER ||--o{ SUBSCRIPTION : holds
    MEMBER ||--o{ ATTENDANCE : checks_in
    MEMBERSHIP_PLAN ||--o{ SUBSCRIPTION : defines
    SUBSCRIPTION ||--o{ PAYMENT : generates
    APPLICATION_USER ||--o{ REFRESH_TOKEN : owns

    GYM {
        int Id PK
        string Name
        int Capacity
    }
    MEMBER {
        int Id PK
        int GymId FK
        string Email
        datetime JoinDate
    }
    SUBSCRIPTION {
        int Id PK
        int MemberId FK
        int MembershipPlanId FK
        string Status
        datetime EndDate
    }
    PAYMENT {
        int Id PK
        int SubscriptionId FK
        string Status
        string TransactionReference
    }
```

---

## Concurrency handling

The part of this project that isn't standard CRUD: three write paths had race conditions under concurrent requests, and each needed a different fix because they're different problems wearing the same "add a transaction" disguise.

**Subscription creation writes to two tables (`Subscription`, then `Payment`) and both need to succeed or neither should.** This is wrapped in an explicit `IDbContextTransaction`, committed only after both inserts succeed, rolled back on any failure — so a payment-insert failure can't leave an orphaned active subscription with no payment record.

**Refresh token rotation had a read-then-write gap.** The old code checked `IsActive` in application code, then separately wrote `RevokedOn`. Two concurrent refresh requests using the same token could both read "active" before either wrote the revocation, producing two valid token pairs from one single-use token. Fixed with a single atomic conditional update instead of a transaction:
```csharp
await _context.RefreshTokens
    .Where(rt => rt.Id == id && rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow)
    .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.RevokedOn, DateTime.UtcNow));
```
The database enforces the check-and-write as one indivisible statement — a losing concurrent request updates zero rows and is rejected, rather than the race being possible at all.

**Gym capacity enforcement is a read (member count) followed by a write (insert) with nothing linking them.** Under the database's default isolation level, two concurrent registrations can both read the count as under capacity before either commits — both pass the check, capacity gets exceeded. This needed `Serializable` isolation specifically (not just any transaction — default `Read Committed` doesn't block this), so the database itself detects the conflict and rejects one of the two competing transactions:
```csharp
await using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
```

The distinction that mattered across all three: an explicit transaction, an atomic conditional update, and a raised isolation level are three different tools for three different shapes of race condition — not one fix copy-pasted three times.

---

## Caching

Membership plan reads use a cache-aside pattern, currently split across two backends as part of an incremental migration:

- `GetAllPlansAsync` and `GetPlanByIdAsync` use `IMemoryCache.GetOrCreateAsync` (in-process, single-instance) — the built-in method locks per-key, so concurrent requests during a cache miss don't all hit the database at once.
- `GetPlansByGymIdAsync` uses `IDistributedCache` (Redis) via a small `GetOrSetAsync` helper, since this was the endpoint targeted for a shared, multi-instance-safe cache.

Writes (create/update/delete) invalidate all three cache keys across both backends — the in-memory "all plans" and "single plan" keys, and the Redis-backed gym-scoped key — rather than relying on TTL expiry alone.

Manual measurement on the gym-scoped endpoint: cold reads (DB hit) consistently over 100ms, dropping to single-digit ms on a cache hit.

**Known gaps, left deliberate rather than hidden:**
- The two `IMemoryCache` endpoints haven't been migrated to Redis yet — in a real multi-instance deployment they'd serve stale/inconsistent data across instances, unlike the Redis-backed endpoint.
- The Redis path doesn't yet guard against a cache stampede (concurrent requests all missing the cache at once and hitting the DB together). `IMemoryCache.GetOrCreateAsync` handles this per-key locking for free; `IDistributedCache` has no built-in equivalent, so this needs a distributed lock (e.g. Redis `SETNX` or RedLock) as a follow-up.

Cache invalidation across all three keys is covered by integration tests (see Testing) — writing them earlier caught a real bug: the member repository was missing `.Include(m => m.Gym)`, so `GymName` silently returned a fallback value instead of the actual gym.

---
## Background Jobs

A Hangfire recurring job (`expire-overdue-subscriptions`) runs hourly, checking for subscriptions still marked `Active` past their `EndDate` and flipping them to `Expired`.

The job is idempotent by construction: once a subscription is expired, it no longer matches the `Status == Active` filter the job queries against, so re-running it — on overlap, a server restart, or a manual trigger from the Hangfire dashboard — is always a safe no-op for records already processed. No separate tracking flag is needed to prevent double-processing.

Hangfire's dashboard (`/hangfire`) exposes the job's schedule and run history, and supports manually triggering a run for testing without waiting for the next hourly execution.

---

## API surface

| Method | Endpoint | Auth | Notes |
|---|---|---|---|
| POST | `/api/auth/login` | — | Rate-limited |
| POST | `/api/auth/register` | Admin | |
| POST | `/api/auth/refresh` | — | Rotates refresh token |
| GET | `/api/gyms`, `/api/gyms/{id}` | Any staff | |
| POST/PUT/DELETE | `/api/gyms` | Admin | Soft delete |
| GET | `/api/members` | Any staff | Paginated, searchable, filterable by gym |
| POST | `/api/members` | Any staff | |
| PUT/DELETE | `/api/members/{id}` | Admin | |
| GET | `/api/plans`, `/api/plans/{id}` | Any staff | Cached — `IMemoryCache` (see Caching) |
| GET | `/api/plans/gym/{gymId}` | Any staff | Cached — Redis (see Caching) |
| POST/PUT | `/api/plans` | Admin, Manager | Invalidates cache across both backends |
| DELETE | `/api/plans/{id}` | Admin | Soft delete, invalidates cache across both backends |
| GET | `/api/subscriptions`, `/{id}` | Any staff | Filter by status, member, plan, date range |
| POST | `/api/subscriptions` | Admin, Manager, Receptionist | Creates subscription + payment atomically |
| POST | `/api/subscriptions/{id}/cancel` \| `freeze` \| `unfreeze` | Admin, Manager | State transitions |
| POST | `/api/attendance/check-in` | Admin, Manager, Receptionist | Validates active subscription for that gym |
| GET | `/api/attendance/gym/{gymId}`, `/history/{memberId}` | Admin, Manager | Paginated |

**Example — paginated member search:**

```
GET /api/members?PageNumber=1&PageSize=10&SearchTerm=ahmed&GymId=3
```

```json
{
  "items": [ /* MemberDto[] */ ],
  "totalCount": 47,
  "currentPage": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

## Business rules

- Subscription transitions are guarded: only `Active` can freeze, only `Frozen` can unfreeze
- Freeze duration capped at 1–90 days
- A member can't hold two active subscriptions to the same plan
- Gym capacity can't be exceeded — enforced under `Serializable` isolation, see above
- Check-in rejected if the subscription is frozen/expired/cancelled, or belongs to a different gym
- Member emails are unique, enforced by a DB index, not just application logic

---

## Testing

Integration tests run through `WebApplicationFactory` against the real middleware pipeline, including a test verifying rate-limited requests are rejected with `429` before reaching the controller. A stubbed authentication handler replaces JWT in the test host, so protected endpoints can be exercised without a real login flow, against an isolated in-memory database per test run.

**Current coverage:** Member CRUD (controller unit tests + integration tests), rate limiting, and cache-aside invalidation for membership plans (create/update/delete correctly bust the cache across both `IMemoryCache` and Redis, and repeated reads return consistent data). Auth flows, Subscriptions, and Attendance — including the concurrency fixes above — aren't covered by automated tests yet; verified manually. Next priority: concurrent-request tests against the capacity check and refresh-token rotation, since those are exactly the bug class a single-threaded test won't catch.

---

## Running it locally

```bash
git clone https://github.com/Ommmarr111/Gym-Management-System.git
cd Gym-Management-System
dotnet ef database update --project GymManagementSystem.Infrastructure --startup-project GymManagementSystem.Api
dotnet run --project GymManagementSystem.Api
```

Set `ConnectionStrings:DefaultConnection` and `Jwt:Key`/`Jwt:Issuer`/`Jwt:Audience` via user secrets or environment variables, not committed config.

Swagger: `https://localhost:<port>/swagger`

---

## Author

**Omar Ahmed** — [GitHub](https://github.com/Ommmarr111) · [LinkedIn](https://linkedin.com/in/Ommmarr111)
