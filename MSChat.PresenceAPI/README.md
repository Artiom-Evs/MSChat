# MSChat.PresenceAPI

User presence tracking service for MSChat that manages online/offline status using Redis and SignalR.

## Overview

This service provides real-time user presence tracking, allowing applications to know when users are online or offline. It uses Redis for fast presence data storage and SignalR for real-time status updates.

## Features

- Real-time presence tracking with SignalR
- Redis-based presence data storage with TTL
- gRPC service for inter-service communication
- JWT authentication and authorization
- Automatic online/offline status management
- User status subscription system

## Technologies

- .NET 9.0
- ASP.NET Core Web API
- SignalR with Redis backplane
- Redis for presence data storage
- gRPC for inter-service communication
- JWT Bearer authentication

## SignalR Hub

- **Hub**: `//_hubs/presence`
- **Methods**: `GetUserStatus`, `SubscribeToUserStatus`, `UnsubscribeFromUserStatus`, `BroadcastOnlineStatus`
- **Events**: `PresenceStatusChanged`
- **Auto-tracking**: Automatically sets online/offline on connect/disconnect

## gRPC Service

- **Service**: `PresenceAPI`
- **Methods**: `GetUsersStatus` - Get status for multiple users
- **Port**: 5016 (development), 8082 (Docker)

## Configuration

Configure the following in `appsettings.json`:

- `ApiAuth` - JWT authentication settings
- `Redis:ConnectionString` - Redis connection for presence data
- `Cors:AllowedOrigins` - CORS configuration