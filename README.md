# TaskFlow — Full-Stack To-Do Task Manager

A production-quality task management app built with **ASP.NET Core 10** and **React + TypeScript**.

---

## Tech Stack

| Layer      | Technology                                                              |
| ---------- | ----------------------------------------------------------------------- |
| Backend    | ASP.NET Core 10 Web API, C#                                              |
| ORM        | Entity Framework Core 10 (SQLite)                                        |
| Validation | FluentValidation (server) + Zod + React Hook Form (client)              |
| Frontend   | React 18, TypeScript, Vite                                              |
| Styling    | Tailwind CSS 3                                                          |
| HTTP       | Axios                                                                   |
| State      | React Context + useReducer                                              |
| Docs       | Swagger / OpenAPI (http://localhost:5000/swagger)                       |

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

- **CRUD tasks** — create, read, update, delete
- **Status workflow** — Todo → In Progress → Done (one-click transitions on each card)
- **Priority levels** — Low / Medium / High with color-coded badges
- **Due dates** — overdue dates highlighted in red, today in amber
- **Filtering** — by status, priority, and free-text search
- **Sorting** — by created date, due date, priority, or status (asc/desc)
- **Validation** — client-side (Zod) and server-side (FluentValidation), both return structured error messages
- **Global error handling** — consistent JSON error shape from the API; error banner in the UI
- **Loading states** — spinner on initial load; per-button loading indicators
- **Responsive layout** — 1-column on mobile, 3-column grid on desktop

---

## Project Structure

```
ToDoList/
├── backend/
│   └── ToDoList.Api/
│       ├── Controllers/        # HTTP layer only — no business logic
│       ├── Data/               # EF Core DbContext + migrations
│       ├── Domain/
│       │   ├── Entities/       # TaskItem entity
│       │   └── Enums/          # TaskItemStatus, TaskPriority
│       ├── DTOs/               # Request / response shapes
│       ├── Middleware/         # Global exception handler
│       ├── Services/           # Business logic + ITaskService interface
│       ├── Validators/         # FluentValidation rules
│       └── Program.cs          # Composition root
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

## Design Decisions & Trade-offs

### Backend

- **No AutoMapper** — manual `MapToDto()` in `TaskService` is explicit, fast, and avoids a heavyweight dependency for a small model. Tradeoff: add AutoMapper once there are many entities.
- **Enums stored as strings** (`HasConversion<string>()`) — SQLite stays human-readable and is resilient to enum member reordering.
- **FluentValidation** over Data Annotations — validation rules are colocated, composable, and easy to unit-test independently. The `AddFluentValidationAutoValidation()` hook means controllers stay thin.
- **PATCH for status** — separating the status-update endpoint mirrors real-world Kanban UX (drag-and-drop) and avoids requiring the full task payload just to mark something done.
- **Auto-migrate on startup** — suitable for a single-instance dev/demo app. In production this would be replaced by a CI step or a dedicated migration job.
- **No authentication in MVP** — explicitly out of scope. The architecture slots JWT auth in cleanly: add `[Authorize]` attributes and a middleware layer without touching business logic.

### Frontend

- **Context + useReducer** over Redux — the state shape is simple (one entity type, one list). Redux would be over-engineering at this scale. `useReducer` gives the same predictability with zero boilerplate.
- **Server-side filtering** — filters are sent as query params so the API does the work. This is the correct default for lists that may grow large. Client-side filtering would be wrong if we later paginate.
- **Zod + React Hook Form** — schema-based validation gives a single source of truth for form types (`z.infer`), eliminates manual type duplication, and integrates cleanly with RHF's `zodResolver`.
- **Axios interceptor** for error normalization — one place to transform any `{ title, detail }` API error into a plain `Error` message, so every component's `catch` block just receives a string.

---

## What I Would Add in a Production V2

| Feature                 | Rationale                                                              |
| ----------------------- | ---------------------------------------------------------------------- |
| Authentication (JWT)    | Multi-user support; tasks scoped to the logged-in user                 |
| Pagination              | The list endpoint returns all records — needs cursor or page-based pagination for scale |
| Optimistic updates      | Update UI immediately, roll back on API failure for snappier feel       |
| Task categories / tags  | Common request; modeled as a many-to-many relationship                 |
| Drag-and-drop Kanban    | The PATCH `/status` endpoint already supports it; needs a Kanban view in the UI |
| Due date reminders      | Email/push notifications via a background job (Hangfire or a queue)    |
| Unit + integration tests | Backend: xUnit + WebApplicationFactory; Frontend: Vitest + RTL         |
| Docker Compose          | Single `docker-compose up` to run both services + SQLite volume        |
| CI/CD pipeline          | GitHub Actions: build, test, lint on every PR                          |
