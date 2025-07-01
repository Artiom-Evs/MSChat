# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

MSChat is a multi-service application implementing the Backend for Frontend (BFF) pattern:

- **MSChat.Auth** - OrchardCore CMS-based OpenID Connect authentication server
- **MSChat.WebBFF** - ASP.NET Core Web API that serves as the BFF and hosts the React frontend
- **mschat.webclient** - React + TypeScript + Vite frontend application

The authentication service provides JWT tokens, while the WebBFF handles API requests and serves the React SPA in production.

## Development Commands

### Build and Run
```bash
# Build entire solution
dotnet build

# Run individual services
dotnet run --project MSChat.Auth
dotnet run --project MSChat.WebBFF

# Run with Docker
docker-compose up

# Frontend development
cd mschat.webclient
npm run dev
npm run build
npm run lint
```

### Testing
```bash
# Run tests (when available)
dotnet test
```

## Port Configuration

- **MSChat.Auth**: 5000 (HTTP), 5001 (HTTPS), 8080/8081 (Docker)
- **MSChat.WebBFF**: 5002 (HTTP), 5003 (HTTPS), 8080/8081 (Docker)  
- **React Dev Server**: 51188 (HTTPS)

## Key Technical Details

### Authentication Flow
- OrchardCore serves as the OpenID Connect provider
- Multi-tenant architecture with database-per-tenant support
- JWT tokens used for API authentication
- Development certificates auto-generated for HTTPS

### Frontend Integration
- React app integrated via SpaProxy in WebBFF project
- Vite proxy configuration routes API calls during development
- Production builds served as static files through ASP.NET Core

### Database
- SQL Server 2022 (containerized)
- Entity Framework Core with OrchardCore data layer
- Connection strings configured in docker-compose.yml

## Project Structure Notes

- Both .NET projects use multi-stage Docker builds
- OrchardCore configuration includes extensive localization support
- Frontend follows standard React + TypeScript + Vite patterns
- Launch profiles configured for HTTP/HTTPS/Docker environments

## Project-Specific Documentation

For detailed information about each project, see their specific CLAUDE.md files:

- **[MSChat.Auth/CLAUDE.md](MSChat.Auth/CLAUDE.md)** - Authentication service details, OrchardCore configuration, and security features
- **[MSChat.WebBFF/CLAUDE.md](MSChat.WebBFF/CLAUDE.md)** - BFF service architecture, API endpoints, and React integration
- **[mschat.webclient/CLAUDE.md](mschat.webclient/CLAUDE.md)** - React frontend architecture, TypeScript configuration, and component patterns