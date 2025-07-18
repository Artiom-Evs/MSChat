# MSChat.ChatAPI

Chat API service for MSChat that handles messaging, chat management, and participant operations.

## Overview

This service provides RESTful APIs and SignalR hubs for real-time chat functionality, along with gRPC endpoints for inter-service communication.

## Features

- RESTful API for chat operations (CRUD)
- Real-time messaging with SignalR
- gRPC service for inter-service communication
- Message queuing with RabbitMQ for notifications
- JWT authentication and authorization
- Entity Framework Core with SQL Server
- Redis backplane for SignalR scaling

## Technologies

- .NET 9.0
- ASP.NET Core Web API
- SignalR with Redis backplane
- Entity Framework Core
- SQL Server
- MediatR (CQRS pattern)
- JWT Bearer authentication
- gRPC
- MassTransit + RabbitMQ

## Configuration

Configure the following in `appsettings.json`:

- `ConnectionStrings:ChatDBConnection` - SQL Server connection
- `ApiAuth` - JWT authentication settings
- `Redis:ConnectionString` - Redis connection for SignalR
- `RabbitMQ` - Message queue settings
- `Cors:AllowedOrigins` - CORS configuration