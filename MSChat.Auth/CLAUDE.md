# MSChat.Auth Project

This file provides guidance specific to the MSChat.Auth authentication service.

## Project Overview

MSChat.Auth is an OrchardCore CMS-based authentication service that provides OpenID Connect and OAuth2 capabilities for the MSChat application ecosystem.

## Technology Stack

- **.NET 8.0** with ASP.NET Core
- **OrchardCore CMS 2.1.7** - Content management and authentication framework
- **SQL Server** - Database (LocalDB for dev, containerized for production)
- **OpenID Connect** - Authentication protocol implementation

## Development Commands

```bash
# Run the authentication service
dotnet run --project MSChat.Auth

# Build the project
dotnet build MSChat.Auth

# Run with Docker
docker compose up mschat-auth
```

## Configuration

### Database Configuration
- **Database**: SQLite for development and production (file-based storage)
- **Location**: App_Data/Sites/Default_db/mschat.db
- **Multi-tenancy**: OrchardCore tenant system with SQLite backend
- **Auto-migration**: Database created and migrated automatically on startup

### Authentication Features
- OpenID Connect server and client support
- JWT token generation and validation
- User management and role-based authorization
- Application registration and management
- Automatic certificate generation for JWT signing
