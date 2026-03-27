# Full-Stack To-Do Task Manager

A task management app built with **ASP.NET Core 10** and **React + TypeScript**.

---

## Tech Stack

| Layer      | Technology                                                              |
| ---------- | ----------------------------------------------------------------------- |
| Backend    | ASP.NET Core 10 Web API, C#                                              |
| ORM        | Entity Framework Core 10 (SQLite)                                        |
| Frontend   | React 19, TypeScript, Vite                                              |
| Styling    | Tailwind CSS 3                                                          |
| HTTP       | Axios                                                                   |
| State      | React Context + useReducer                                              |
| Docs       | Swagger / OpenAPI (http://localhost:5000/swagger)                       |
| Testing    | xUnit, `WebApplicationFactory`, SQLite in-memory                        |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- npm 9+

---

## Getting Started

### 1. Backend

```bash
cd backend/ToDoList.Api
dotnet restore
dotnet run
```

The API starts at **http://localhost:5000**.
EF Core migrations are applied automatically on startup — `todolist.db` is created in the project directory.

Swagger UI: http://localhost:5000/swagger

### 2. Frontend

```bash
cd frontend
npm install
npm run dev
```

The app opens at **http://localhost:5173**.

---

## API Reference

| Method   | Endpoint                    | Description                         |
| -------- | --------------------------- | ----------------------------------- |
| `POST`   | `/api/auth/google`          | Exchange Google ID token for JWT    |
| `GET`    | `/api/tasks`                | List tasks (filterable, sortable)   |
| `GET`    | `/api/tasks/{id}`           | Get a single task                   |
| `POST`   | `/api/tasks`                | Create a task                       |
| `PUT`    | `/api/tasks/{id}`           | Replace a task                      |
| `PATCH`  | `/api/tasks/{id}/status`    | Update task status only             |
| `DELETE` | `/api/tasks/{id}`           | Delete a task                       |

### Query Parameters for `GET /api/tasks`

| Param       | Values                                  | Default      |
| ----------- | --------------------------------------- | ------------ |
| `status`    | `Todo` \| `InProgress` \| `Done`        | (all)        |
| `priority`  | `Low` \| `Medium` \| `High`             | (all)        |
| `search`    | any string                              | (none)       |
| `sortBy`    | `createdAt` \| `dueDate` \| `priority` \| `status` | `createdAt` |
| `sortOrder` | `asc` \| `desc`                         | `desc`       |

---

## Features

- **Google Sign-In** — OAuth2 via Google; JWT issued on login, auto-logout on token expiry
- **Per-user isolation** — tasks are scoped to the authenticated user; no cross-user data leakage
- **CRUD tasks** — create, read, update, delete
- **Status workflow** — Todo → In Progress → Done (one-click transitions on each card)
- **Priority levels** — Low / Medium / High with color-coded badges
- **Due dates** — overdue dates highlighted in red, today in amber
- **Filtering** — by status, priority, and free-text search
- **Sorting** — by created date, due date, priority, or status (asc/desc)
- **Validation** — client-side (Zod) and server-side, both return structured error messages
- **Global error handling** — consistent JSON error shape from the API; error popup in the UI
- **Loading states** — spinner on initial load; per-button loading indicators
- **Responsive layout** — 1-column on mobile, 3-column grid on desktop

---

## Project Structure

```
ToDoList/
├── backend/
│   ├── ToDoList.Api/
│   │   ├── Controllers/        # HTTP layer only — no business logic
│   │   ├── Data/               # EF Core DbContext + migrations
│   │   ├── Domain/
│   │   │   ├── Entities/       # TaskItem entity
│   │   │   └── Enums/          # TaskItemStatus, TaskPriority
│   │   ├── DTOs/               # Request / response shapes
│   │   ├── Middleware/         # Global exception handler
│   │   ├── Services/           # Business logic
│   │   └── Program.cs          # Composition root
│   └── ToDoList.Api.IntegrationTests/
│       ├── Infrastructure/     # WebApplicationFactory, FakeAuthHandler, TasksTestBase
│       └── Tasks/              # Integration tests per endpoint (GetAll, GetById, Create, …)
└── frontend/
    └── src/
        ├── api/                # Axios client + typed API functions
        ├── components/
        │   ├── layout/         # Header, PageLayout
        │   ├── tasks/          # TaskList, TaskCard, TaskForm, TaskFilters
        │   └── ui/             # Button, Input, Select, Modal, etc.
        ├── context/            # TaskContext (state + actions)
        ├── hooks/              # useTasks, useTaskForm (Zod schema)
        ├── types/              # TypeScript interfaces
        └── utils/              # Date formatting helpers
```

---

## Assumptions & Scalability

### Assumptions

- **Google OAuth only** — authentication is handled via Google ID tokens. There is no username/password flow. Users are identified by their Google account; a `UserId` FK on every task enforces per-user data isolation.
- **Moderate task volume** — the design assumes hundreds to low-thousands of tasks. Filtering and sorting are done server-side (correct default), but `GET /api/tasks` returns all matching records with no pagination, which would break at high volume.
- **Single-instance deployment** — SQLite is a file-based database that cannot handle concurrent writes from multiple processes. The app is designed for a single server instance (dev/demo).
- **Eventual consistency is acceptable** — there is no optimistic concurrency control. If two users edited the same task simultaneously, the last write wins silently.
- **Trusted network** — no rate limiting, no throttling, and CORS is locked to `localhost:5173`. Suitable for a local or internal environment; not safe for the open internet without adding those layers.

### Path to Scale

The biggest scalability constraint is **SQLite + no pagination** — these are what prevent the app from serving real users at scale. Everything else is multiplier work.

**Step 1 — Switch to production ready database**
- Replace SQLite with **PostgreSQL**. SQLite cannot handle concurrent writes; Postgres scales to millions of rows and supports real connection pooling.

**Step 2 — Handle growing data volume**
- Add **cursor-based pagination** to `GET /api/tasks`. The current design downloads all records on every page load — a user with 10 000 tasks would feel this immediately.
- Replace the `LIKE '%…%'` search with **Postgres `tsvector`** or Elasticsearch. A leading-wildcard `LIKE` cannot use an index.

**Step 3 — Handle growing traffic**
- Add a **Redis cache** in front of task-list reads. Each user's filtered result set can be cached for a short TTL and invalidated on write. Even a 1-second cache eliminates most DB read load at scale.
- Add **rate limiting** per user (ASP.NET Core `RateLimiter` middleware is built-in since .NET 7).
- The API layer is already stateless — run **multiple instances** behind a load balancer (K8s HPA, AWS ALB) with zero code changes.

**Step 4 — Global scale**
- Add a **read replica per region** for `GET` queries. `TaskService` is almost entirely reads; routing them to replicas removes load from the primary.
- For non-critical writes (status updates, deletes), consider an **async queue** (Kafka, SQS): return `202 Accepted` immediately and process in the background to decouple API latency from DB write latency.
- Add **structured logging + distributed tracing** (OpenTelemetry → Datadog/Grafana). Console logs don't survive a multi-instance, multi-region deployment.

See the [Production V2 — Changes & Priorities](#production-v2--changes--priorities) table below for a full prioritised list.

---

## Design Decisions & Trade-offs

### Backend

- **No AutoMapper** — manual `MapToDto()` in `TaskService` is explicit, fast, and avoids a heavyweight dependency for a small model. Tradeoff: add AutoMapper once there are many entities.
- **PATCH for status** — separating the status-update endpoint mirrors real-world Kanban UX (drag-and-drop) and avoids requiring the full task payload just to mark something done.
- **Auto-migrate on startup** — suitable for a single-instance dev/demo app. In production this would be replaced by a CI step or a dedicated migration job.
- **Google OAuth + JWT** — users authenticate via Google ID token (`POST /api/auth/google`); the API issues a signed JWT. `[Authorize]` on all task endpoints; `UserId` is extracted from the JWT claim and injected into every query, ensuring strict per-user data isolation.

### Frontend

- **Context + useReducer** over Redux — the state shape is simple (one entity type, one list). Redux would be over-engineering at this scale. `useReducer` gives the same predictability with zero boilerplate.
- **Server-side filtering** — filters are sent as query params so the API does the work. This is the correct default for lists that may grow large. Client-side filtering would be wrong if we later paginate.
- **Zod + React Hook Form** — schema-based validation gives a single source of truth for form types (`z.infer`), eliminates manual type duplication, and integrates cleanly with RHF's `zodResolver`.
- **Axios interceptor** for error normalization — one place to transform any `{ title, detail }` API error into a plain `Error` message, so every component's `catch` block just receives a string.

---

## Production V2 — Changes & Priorities

| Priority | Feature / Change | Area | Rationale |
|---|---|---|---|
| P1 — Critical | SQLite → PostgreSQL (or CockroachDB or AWS DynamoDB) | Database | SQLite cannot handle concurrent writes; hits a wall at a few thousand concurrent users. Highest-leverage single change. |
| P2 — High | Unit + integration tests | Quality | xUnit + WebApplicationFactory on backend; Vitest + RTL on frontend. Needed before CI/CD is useful. |
| P2 — High | CI/CD pipeline | Infrastructure | GitHub Actions: build, test, lint on every PR. Blue/green deployments + DB migration strategy. |
| P2 — High | Pagination (cursor / keyset) on GET /api/tasks | API layer | Endpoint currently returns all records. A user with millions of tasks would download everything on each request. |
| P2 — High | Rate limiting per user | API layer | Without it, a single user can DoS the service. ASP.NET Core RateLimiter middleware is built-in. |
| P2 — High | Redis cache for task list reads | Caching | Every request currently hits the DB. Even a 1-second cache per user+filter eliminates most DB load at scale; invalidate on writes. |
| P3 — Medium | Read replica per region for GET queries | Database | TaskService queries are almost all reads; routing to replicas removes load from the primary at scale. |
| P3 — Medium | Horizontal scaling behind load balancer | Infrastructure | App is already stateless — just run multiple instances behind K8s HPA or AWS ALB. No code changes required. |
| P3 — Medium | Async write path via message queue | Write path | For non-critical writes (status updates, deletes), return 202 immediately and process via Kafka/SQS. Decouples API latency from DB write latency. |
| P3 — Medium | Optimistic concurrency (EF Core rowversion) | Write path | Handles concurrent edits on creates/updates where the user needs the result back synchronously. |
| P3 — Medium | Docker Compose | Infrastructure | Single `docker-compose up` to run both services + DB volume. Reduces onboarding friction and environment drift. |
| P3 — Medium | Full-text search (Postgres tsvector / Elasticsearch) | Database | Current LIKE '%…%' has a leading wildcard — unindexable. Must be replaced before search becomes a bottleneck. |
| P3 — Medium | Optimistic UI updates | Frontend | Update UI immediately, roll back on API failure. Improves perceived performance with no backend changes. |
| P3 — Medium | Structured logs + observability (OpenTelemetry) | Infrastructure | Console logs don't scale. Structured logs → Datadog/Grafana + distributed tracing needed for production diagnosis. |
| P4 — Nice to have | Task categories / tags | Feature | Many-to-many relationship. Commonly requested but purely additive — no existing functionality depends on it. |
| P4 — Nice to have | Drag-and-drop Kanban view | Feature | PATCH /status already supports it; needs only a Kanban UI. Frontend-only work once auth and pagination are in place. |
| P4 — Nice to have | Due date reminders | Feature | Email/push notifications via background job (Hangfire or a queue). Useful but requires notification infrastructure not yet present. |
| P4 — Nice to have | CDN for static frontend assets | Infrastructure | Low effort, meaningful latency win for geographically distributed users once multi-region deployment is in place. |
