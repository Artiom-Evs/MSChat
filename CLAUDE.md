# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

MSChat is a multi-service application implementing microservices architecture with a Backend for Frontend (BFF) pattern:

- **MSChat.Auth** - OrchardCore CMS-based OpenID Connect authentication server (.NET 8.0)
- **MSChat.WebBFF** - ASP.NET Core minimal API serving React SPA and runtime configuration (.NET 9.0)
- **MSChat.ChatAPI** - Dedicated chat API service handling all chat functionality (.NET 9.0)
- **MSChat.PresenceAPI** - User presence tracking service (.NET 9.0)
- **MSChat.NotificationWorker** - Background notification service (.NET 9.0)
- **MSChat.Shared** - Shared contracts and configurations (.NET 9.0)
- **mschat.webclient** - React + TypeScript + Material-UI frontend application

The authentication service provides JWT tokens, the Chat API handles all business logic, the Presence API tracks user status, and the WebBFF serves the React SPA with minimal API surface.

## Development Commands

### Build and Run
```bash
# Build entire solution
dotnet build

# Run individual services
dotnet run --project MSChat.Auth
dotnet run --project MSChat.WebBFF
dotnet run --project MSChat.ChatAPI
dotnet run --project MSChat.PresenceAPI
dotnet run --project MSChat.NotificationWorker

# Run with Docker
docker compose up

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

- **MSChat.Auth**: 5000 (HTTP), 5001 (HTTPS)
- **MSChat.WebBFF**: 5002 (HTTP), 5003 (HTTPS)
- **MSChat.ChatAPI**: 5004 (HTTP), 5005 (HTTPS), 5014 (gRPC)
- **MSChat.PresenceAPI**: 5006 (HTTP), 5007 (HTTPS), 5016 (gRPC)
- **React Dev Server**: 51188 (HTTPS)
- **SQL Server**: 1433 (Docker)
- **Redis**: 6379 (Docker)
- **RabbitMQ**: 5672 (Docker), 15672 (Management UI)

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
- **MSChat.Auth**: SQLite database with OrchardCore schema
- **MSChat.ChatAPI**: SQL Server 2022 (containerized) with Entity Framework Core
- Chat service includes: Chat, Message, ChatMember, and ChatMemberLink entities
- Connection strings configured in docker-compose.yml

### Infrastructure
- **Redis**: Caching and user presence data storage
- **RabbitMQ**: Message queuing for background notifications
- **gRPC**: Inter-service communication for ChatAPI and PresenceAPI

## Project-Specific Documentation

For detailed information about each project, see their specific CLAUDE.md files:

- **[MSChat.Auth/CLAUDE.md](MSChat.Auth/CLAUDE.md)** - OrchardCore-based OpenID Connect authentication server
- **[MSChat.WebBFF/CLAUDE.md](MSChat.WebBFF/CLAUDE.md)** - Minimal BFF serving React SPA and runtime configuration
- **[MSChat.ChatAPI/CLAUDE.md](MSChat.ChatAPI/CLAUDE.md)** - Full-featured chat API with messaging, participants, and JWT auth
- **[MSChat.PresenceAPI/CLAUDE.md](MSChat.PresenceAPI/CLAUDE.md)** - User presence tracking service
- **[MSChat.NotificationWorker/CLAUDE.md](MSChat.NotificationWorker/CLAUDE.md)** - Background notification service
- **[mschat.webclient/CLAUDE.md](mschat.webclient/CLAUDE.md)** - React frontend with Material-UI, real-time chat interface

## Current Implementation Status

### Fully Implemented Features
- **Authentication**: Complete OIDC flow with JWT tokens
- **Chat Management**: Create, edit, delete chats (Personal/Public types)
- **Messaging**: Send, edit, delete messages with pagination
- **User Management**: Member profiles, join/leave chats
- **User Presence**: Real-time presence tracking with Redis
- **Notifications**: Email notifications for unread messages
- **Frontend**: Complete Material-UI interface with routing and state management
- **Database**: SQL Server with Entity Framework migrations
- **Deployment**: Docker Compose setup for all services

### Architecture Decisions
- **Microservices**: Separate APIs for chat, presence, and notifications
- **Authentication**: OrchardCore for enterprise-grade OIDC
- **Frontend**: React 19.1.0 + Material-UI for modern UI
- **Database**: SQL Server for chat data, SQLite for auth
- **Caching**: Redis for presence data and performance
- **Messaging**: RabbitMQ for asynchronous notifications
- **Communication**: gRPC for inter-service communication
- **State Management**: React Query + Zustand for optimal performance

### Development Workflow
1. `docker compose up` - Start all services and database
2. `dotnet run --project <service>` - Individual service development
3. `cd mschat.webclient && npm run dev` - Frontend development with hot reload
4. Database migrations handled automatically on service startup