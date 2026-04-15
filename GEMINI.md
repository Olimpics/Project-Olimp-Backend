# Project Olimp Backend (OlimpBack)

This project is the backend API for the "Olimp" system, an educational or competition management platform. It is built using ASP.NET Core 8.0 and follows a layered architecture with a focus on student and academic data management.

## Project Overview

The Olimp API manages various aspects of an educational institution, including:
- **User Management**: Authentication and Role-Based Access Control (RBAC).
- **Academic Structure**: Faculties, Departments, Groups, and Educational Programs.
- **Student Data**: Personal information, achievements, and academic status.
- **Curriculum**: Main and additional disciplines, elective choice periods.
- **Communication**: Notification system with templates.
- **Auditing**: Administrative logs for tracking changes.

## Architecture

The project follows a typical layered architecture:
- **Controllers**: RESTful API endpoints handling HTTP requests and responses.
- **Application**:
  - **Services**: Business logic implementation.
  - **DTOs**: Data Transfer Objects for decoupled data exchange.
- **Infrastructure**:
  - **Database**: `AppDbContext` using Entity Framework Core.
  - **Repositories**: Data access abstraction.
- **Models**: Domain entities representing database tables.
- **MappingProfiles**: AutoMapper configurations for entity-to-DTO transformations.
- **Utils**: Utility classes for JWT, passwords, and background tasks.

## Technology Stack

- **Framework**: .NET 8.0 (ASP.NET Core)
- **Database**: MariaDB/MySQL (via `Pomelo.EntityFrameworkCore.MySql`)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens (and Cookies for frontend integration)
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Hashing**: BCrypt.Net-Next
- **Cache/Messaging**: Redis (via `StackExchange.Redis`)

## Key Modules

- **AuthModule**: Handles login (including a development "stub" mode), password changes, and permission management.
- **DisciplineModule**: Manages academic disciplines and elective choice periods.
- **StudentModule**: Comprehensive student profile and academic record management.
- **NotificationModule**: System for sending and managing notifications.

## Building and Running

### Prerequisites
- .NET 8.0 SDK
- MySQL/MariaDB server (default port 3307 as per `appsettings.json`)
- Redis server (optional, depending on active services)

### Commands
- **Build**: `dotnet build`
- **Run**: `dotnet run`
- **Watch**: `dotnet watch run`
- **Restore Dependencies**: `dotnet restore`

### Configuration
The main configuration is in `appsettings.json`. Key sections:
- `ConnectionStrings:DefaultConnection`: Database connection string.
- `Jwt`: Configuration for token generation (Key, Issuer, Audience).
- `UseStubLogin`: A boolean flag to enable/disable stub authentication for development.

## Development Conventions

- **Surgical Updates**: When modifying existing code, adhere strictly to the established Repository-Service-Controller pattern.
- **Naming**: Use PascalCase for C# classes and methods, camelCase for local variables.
- **DI**: Register all services and repositories in `Program.cs`.
- **Mapping**: Use `MappingProfile.cs` for all DTO mappings; avoid manual mapping in services.
- **Nullable**: Nullable reference types are enabled (`<Nullable>enable</Nullable>`).
- **Validation**: Use DTOs for input validation.

## Database Management

The project uses EF Core migrations.
- **Add Migration**: `dotnet ef migrations add <MigrationName>`
- **Update Database**: `dotnet ef database update`
- **Context**: `AppDbContext` is located in `Infrastructure/Database/AppDbContext.cs`.

## TODOs / Known Patterns
- The project includes a `start-tunnel.bat` and logic in `Program.cs` to start a tunnel process, likely for exposing the local server during development.
- Stub login is frequently used for rapid frontend development without a full DB setup.
