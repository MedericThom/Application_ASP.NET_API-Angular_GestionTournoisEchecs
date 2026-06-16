# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build

# Run the API
dotnet run --project ChessAppli/API.csproj

# Run with HTTPS
dotnet run --project ChessAppli/API.csproj --launch-profile https
```

**Required before running:** Set the `CHESS_CONNECTION_STRING` environment variable (SQL Server, Windows Authentication):
```
Data Source=<SERVER>\TFTIC;Initial Catalog=asp_api_ChessTournament;Integrated Security=True;TrustServerCertificate=True;
```

Swagger UI is available at `/swagger` when running in Development mode.

## Architecture

Four-layer solution (`ChessAppli.slnx`):

- **API** (`ChessAppli/`) — ASP.NET Core 10 Web API. Controllers route HTTP requests and inject BLL services.
- **BLL** (`BLL/`) — Business logic services. Each entity has an `IXxxService` interface and `XxxService` implementation. Services call DAL repositories.
- **DAL** (`DAL/`) — Data access using raw ADO.NET (no ORM). `DbConnection` reads the connection string from the `CHESS_CONNECTION_STRING` env var. Each entity has an `IXxxRepository` interface and `XxxRepository` implementation using `SqlConnection`/`SqlCommand` with parameterized queries.
- **DOMAIN** (`DOMAIN/Entities/`) — Plain C# entity classes (POCOs). No dependencies on other layers.

Dependency flow: `API → BLL → DAL → DOMAIN`. All layers reference DOMAIN; BLL references DAL interfaces; API references BLL interfaces. DI registration is in `Program.cs` using `AddScoped`.

## Domain Model

Entities: `ChessClub`, `Player` (with Elo rating, gender, club FK), `Tournament` (name, location, Elo range, women-only flag, deadline, player limits), `Registration` (player-tournament with score), `Match_` (white/black player FKs, result), `Category`, `TournamentCategory` (junction).

## Implementation Status

Only `ChessClub` is fully implemented end-to-end (repository → service → controller). All other entities (`Player`, `Tournament`, `Registration`, `Match_`, `Category`) have interface stubs and empty implementations waiting to be built following the same pattern as `ChessClubRepository` and `ChessClubService`.

When implementing a new entity, follow this order:
1. Add methods to the DAL interface (`IXxxRepository`)
2. Implement in `XxxRepository` using ADO.NET parameterized queries (see `ChessClubRepository` for the pattern)
3. Add methods to the BLL interface (`IXxxService`)
4. Implement in `XxxService` delegating to the repository
5. Add controller actions in the API layer
6. Register new scopes in `Program.cs`
