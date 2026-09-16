# SATPrep

A full-stack SAT preparation platform with timed practice tests, adaptive flashcards, and progress tracking.

## Tech Stack

**Backend** (.NET 10 + EF Core + PostgreSQL)
- ASP.NET Core 10 Web API
- Entity Framework Core 10 + PostgreSQL 16
- JWT authentication via HttpOnly cookies + CSRF double-submit
- MediatR-style service architecture (Controllers → Services → Repositories → DTOs → Mappings)
- OpenAPI via built-in `AddOpenApi()` (no Swashbuckle)

**Frontend** (Next.js 16 + React 19 + TypeScript + Tailwind v4)
- Next.js 16 App Router (TypeScript strict mode)
- React 19 with React Compiler–compatible patterns
- Tailwind CSS v4 with custom design tokens
- Cookie-based auth with automatic refresh and CSRF handling

## Project Structure

```
SATPrep/
├── SATPrep/                        # .NET backend
│   ├── SATPrep.slnx                # Solution file
│   └── src/
│       ├── SATPrep.Api/            # ASP.NET Core API project
│       └── frontend/               # Next.js frontend
│           ├── src/app/            # Route pages (App Router)
│           ├── src/lib/api.ts      # Typed API client
│           └── src/contexts/       # React context providers
├── tests/SATPrep.Api.Tests/        # xUnit integration tests
└── .github/workflows/ci.yml        # GitHub Actions CI
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 22+
- PostgreSQL 16+ (or use Docker Compose)

### Quick Start with Docker
```bash
docker compose up -d                      # Start PostgreSQL
cd SATPrep
dotnet run --project src/SATPrep.Api      # Run API at http://localhost:5015
cd src/frontend
npm install && npm run dev                # Run frontend at http://localhost:3000
```

### Manual Setup

**Database:**
```bash
docker compose up -d postgres  # or use your own PostgreSQL instance
# Connection: Host=localhost;Port=5432;Database=satprep;Username=postgres;Password=postgres
```

**Backend secrets:**
```bash
cd SATPrep/src/SATPrep.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Secret" "your-super-secret-key-at-least-32-chars-long"
```

**Run:**
```bash
cd SATPrep
dotnet run --project src/SATPrep.Api
# API: http://localhost:5015
```

**Frontend:**
```bash
cd SATPrep/src/frontend
npm install
npm run dev
# Frontend: http://localhost:3000 (proxies /api/* to backend)
```

## Features

- **Auth:** Register/login with JWT HttpOnly cookies + CSRF double-submit protection
- **Quiz Taking:** Timed practice tests with question palette, auto-submit on timeout, live progress tracking
- **Results & Grading:** Automatic scoring, section breakdowns, question-by-question review
- **Dashboard:** Study streaks, average scores, weak areas analysis, recent activity
- **Flashcards:** SM-2 spaced repetition, flip-to-reveal, quality-based scheduling
- **History:** Searchable practice attempt history with duration and score tracking

## Running Tests
```bash
cd tests/SATPrep.Api.Tests
dotnet test
```
