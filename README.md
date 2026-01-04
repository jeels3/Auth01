# Auth01 - Authentication Service

**Auth01** is a lightweight, dedicated authentication service refactored from a legacy application. It provides secure user registration, login, token management (JWT), and role-based access control using ASP.NET Core Identity.

## Features

- **User Registration**: Sign up with username, email, password, and role.
- **Authentication**: JWT-based login (Access Token + Refresh Token).
- **Token Management**:
    - Automatic token refreshing.
    - Secure token revocation.
- **Role-Based Authorization**: Support for roles (e.g., USER, ADMIN).
- **Swagger Documentation**: Interactive API documentation.

## Architecture

The project follows a clean architecture with the following layers:

- **Auth01.Domain**: Core entities (`ApplicationUser`, `RefreshToken`).
- **Auth01.Application**: DTOs, Interfaces, and Business Logic (`ITokenService`).
- **Auth01.Infrastructure**: Data Access (EF Core), Repository implementations (`GenericRepository`), and Service implementations (`TokenService`).
- **Auth01.Web**: API Controllers and configuration.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server

### Configuration

Update `Auth01.Web/appsettings.json` with your database string and JWT settings.

```json
"ConnectionStrings": {
  "SQLServerIdentityConnection": "Server=localhost;Database=Auth01;Trusted_Connection=True;TrustServerCertificate=True;"
},
"JwtSettings": {
  "SecretKey": "YOUR_SECRET_KEY",
  "Issuer": "auth01-api",
  "Audience": "auth01-client"
}
```

### Running the Service

1. Navigate to the `Auth01.Web` directory:
   ```bash
   cd Auth01.Web
   ```
2. Apply migrations (database will be created):
   ```bash
   dotnet ef migrations add InitialAuthMigration -p ../Auth01.Infrastructure -s .
   dotnet ef database update
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open Swagger UI at `https://localhost:7087/swagger` (or configured port).

## API Endpoints

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `POST /api/Auth/refresh-token`
- `POST /api/Auth/revoke-token`
- `POST /api/Auth/logout`

## License

MIT
