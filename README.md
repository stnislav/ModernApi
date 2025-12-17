# ModernApi

Production-ready ASP.NET Core 8 Web API demonstrating clean architecture,
DTOs, DI, validation and global exception handling.

## Tech stack
- .NET 8 / ASP.NET Core
- Swagger / OpenAPI
- Custom exception middleware

## Run locally
dotnet run

Swagger: https://localhost:<port>/swagger

## Endpoints
- GET /api/items
- GET /api/items/{id}
- POST /api/items
- PUT /api/items/{id}

## Notes
- In-memory storage (Week 1)
- Next: Docker, EF Core, Kubernetes