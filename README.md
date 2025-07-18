# MSChat

Microservices-based chat application built with .NET, React, and TypeScript.

## Quick Start

```bash
# Start all services
docker compose up -d

# Access the application at:
# - Frontend: http://localhost:5002
# - Auth Server: http://localhost:5000
# - Chat API: http://localhost:5004
# - Presence API: http://localhost:5006
```

## Architecture

- **MSChat.Auth** - OpenID Connect authentication server (.NET 8.0)
- **MSChat.WebBFF** - Backend for Frontend serving React SPA (.NET 9.0)
- **MSChat.ChatAPI** - Chat API service (.NET 9.0)
- **MSChat.PresenceAPI** - User presence tracking service (.NET 9.0)
- **MSChat.NotificationWorker** - Background notification service (.NET 9.0)
- **MSChat.Shared** - Shared contracts and configurations (.NET 9.0)
- **mschat.webclient** - React frontend (TypeScript + Material-UI)

## Infrastructure

- **SQL Server** - Chat data storage
- **Redis** - Caching and presence data
- **RabbitMQ** - Message queuing for notifications

## Features

- OpenID Connect authentication
- Real-time messaging with SignalR
- Chat management and user presence
- Email notifications for unread messages
- Responsive Material-UI design

## Development

See individual project `CLAUDE.md` files for detailed development information.
