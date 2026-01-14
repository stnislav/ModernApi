# ModernApi

Production-ready ASP.NET Core 10 Web API demonstrating clean architecture,
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

## Run with Docker

The API can be built and run using Docker without installing .NET locally.

### Build image
```bash
docker build -t modernapi .
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8081:8080 modernapi

Note: If port 8081 is already in use, map another host port:
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8082:8080 modernapi

## Run with Docker Compose (API + PostgreSQL)

Start:
```bash
docker compose up --build
