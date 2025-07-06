# MSChat

A modern microservices-based chat application built with .NET, React, and TypeScript. MSChat provides real-time messaging, user authentication, and chat management capabilities with a scalable architecture.

## Quick Start

### Prerequisites
- Docker and Docker Compose
- .NET 8.0/.NET 9.0 SDK (for local development)
- Node.js 18+ (for frontend development)

### Running with Docker Compose

```bash
# Clone the repository
git clone https://github.com/artiom-evs/mschat
cd MSChat

# Start all services
docker compose up -d

# The application will be available at:
# - Frontend: http://localhost:5002 (via WebBFF)
# - Auth Server: http://localhost:5000
# - Chat API: http://localhost:5004
# - SQL Server: localhost:1433
```

The Docker setup includes:
- SQL Server 2022 database
- Authentication service (OrchardCore)
- Chat API service
- Web frontend (React SPA)
- Automatic service orchestration and networking

## Architecture Overview

MSChat implements a microservices architecture with the following components:

### MSChat.Auth
**OrchardCore-based OpenID Connect Authentication Server**
- **Technology**: .NET 8.0, OrchardCore CMS 2.1.7, SQLite
- **Purpose**: Centralized authentication and authorization service
- **Features**: 
  - OpenID Connect server with JWT token issuance
  - User registration and management
  - Multi-language support (20+ languages)
  - Administrative interface for user and application management
  - Automatic certificate management for JWT signing
- **Ports**: 5000 (HTTP), 5001 (HTTPS)

### MSChat.Chat
**RESTful Chat API Service**
- **Technology**: .NET 9.0, ASP.NET Core, Entity Framework Core, SQL Server
- **Purpose**: Core chat functionality and business logic
- **Features**:
  - Complete CRUD operations for chats, messages, and participants
  - JWT authentication with scope-based authorization
  - Real-time messaging support
  - Chat types: Personal and Public
  - Message editing and deletion with soft deletes
  - Member and participant management
  - RESTful API with OpenAPI/Swagger documentation
- **Ports**: 5004 (HTTP), 5005 (HTTPS)
- **Database**: SQL Server with Entity Framework migrations

### MSChat.WebBFF
**Backend for Frontend (BFF) Service**
- **Technology**: .NET 9.0, ASP.NET Core Minimal APIs
- **Purpose**: Serve React SPA and provide runtime configuration
- **Features**:
  - Static file serving for React production builds
  - SPA proxy for development workflow
  - Runtime configuration endpoint (`/app-config`)
  - Client-side routing support
  - HTTPS redirection and CORS configuration
- **Ports**: 5002 (HTTP), 5003 (HTTPS)

### mschat.webclient
**React Frontend Application**
- **Technology**: React 19.1.0, TypeScript 5.8.3, Material-UI 7.2.0, Vite 7.0.0
- **Purpose**: User interface for chat application
- **Features**:
  - Material Design UI with responsive layout
  - OpenID Connect authentication integration
  - Real-time chat interface with message management
  - Chat creation, editing, and deletion
  - User profile and participant management
  - React Router for navigation with protected routes
  - React Query for server state management
  - Zustand for client state management
- **Development Port**: 51188 (HTTPS)

## Technology Stack

**Backend Services**:
- **.NET 8.0/9.0** with ASP.NET Core
- **OrchardCore CMS** for authentication infrastructure
- **Entity Framework Core** for data access
- **SQL Server 2022** for chat data storage
- **SQLite** for authentication data
- **JWT Bearer Authentication** with OpenID Connect
- **Docker** for containerization

**Frontend**:
- **React 19.1.0** with functional components and hooks
- **TypeScript 5.8.3** for type safety
- **Material-UI 7.2.0** for component library and design system
- **React Router 7.6.3** for client-side routing
- **React Query 5.81.5** for server state management
- **React OIDC Context 3.3.0** for authentication
- **Vite 7.0.0** for build tooling and development server

## Key Features

- **Authentication**: Enterprise-grade OpenID Connect with OrchardCore
- **Real-time Messaging**: Send, edit, and delete messages
- **Chat Management**: Create and manage Personal and Public chats
- **User Management**: Member profiles and chat participants
- **Responsive Design**: Mobile-first Material-UI interface
- **Scalable Architecture**: Microservices with independent deployment
- **Developer Experience**: Hot reload, TypeScript, and comprehensive tooling
- **Production Ready**: Docker deployment with SQL Server backend

## API Documentation

- **Chat API**: Available at `http://localhost:5004/swagger` when running
- **Authentication**: OpenID Connect discovery at `http://localhost:5000/.well-known/openid_configuration`

## Development

See individual project `CLAUDE.md` files for detailed development information:
- [MSChat.Auth/CLAUDE.md](MSChat.Auth/CLAUDE.md)
- [MSChat.Chat/CLAUDE.md](MSChat.Chat/CLAUDE.md)
- [MSChat.WebBFF/CLAUDE.md](MSChat.WebBFF/CLAUDE.md)
- [mschat.webclient/CLAUDE.md](mschat.webclient/CLAUDE.md)
