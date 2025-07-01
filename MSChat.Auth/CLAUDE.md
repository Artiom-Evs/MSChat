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
docker-compose up mschat-auth
```

## Configuration

### Database Configuration
- **Development**: SQL Server LocalDB connection
- **Production**: SQL Server container via docker-compose
- **Multi-tenancy**: Separate table prefixes per tenant
- **Migration**: Automatic schema creation on startup

### Authentication Features
- OpenID Connect server and client support
- JWT token generation and validation
- User management and role-based authorization
- Application registration and management
- Automatic certificate generation for JWT signing

### Ports and URLs
- **Development**: HTTP 5000, HTTPS 5001
- **Docker**: HTTP 8080, HTTPS 8081
- **Admin Interface**: Available at root URL

## OrchardCore Modules

The service uses these OrchardCore features:
- User authentication and management
- Role-based authorization
- OpenID Connect server
- OpenID applications management
- Administrative interface
- Multi-tenant architecture

## Security Features

- Automatic JWT signing and encryption certificates
- Data protection key management in App_Data directory
- Environment-specific connection strings
- Secure certificate storage and rotation

## Localization

Comprehensive multi-language support with pre-configured translations for:
- English, Spanish, French, German, Italian, Portuguese, Russian, Chinese, and more
- GNU gettext (.po) file format in `App_Data/Localization/`

## Integration Points

- Serves as central authentication provider for MSChat.WebBFF
- Provides JWT tokens for API authorization
- Supports standard OIDC flows for external applications
- Multi-tenant capability for enterprise deployments

## Development Notes

- Clean, minimal codebase focusing on authentication
- Leverages OrchardCore's mature authentication infrastructure
- Environment-specific configurations in appsettings files
- Docker multi-stage build for production deployment