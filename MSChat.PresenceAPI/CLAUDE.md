# MSChat.PresenceAPI Project

This file provides guidance specific to the MSChat.PresenceAPI service.

## Project Overview

MSChat.PresenceAPI is a specialized microservice built with ASP.NET Core that provides real-time user presence tracking capabilities. It manages online/offline status using Redis for fast data storage and SignalR for real-time communication, with gRPC endpoints for inter-service communication.

## Technology Stack

- **.NET 9.0** with ASP.NET Core
- **SignalR** - Real-time communication with Redis backplane
- **Redis** - Fast presence data storage with TTL
- **gRPC** - Inter-service communication
- **JWT Authentication** - Token-based security
- **Docker** - Containerization support

## Development Commands

```bash
# Run the Presence API service
dotnet run --project MSChat.PresenceAPI

# Build the project
dotnet build MSChat.PresenceAPI

# Run with Docker
docker compose up presenceapi
```

## Configuration

### Authentication
- **JWT Tokens**: Validates tokens issued by MSChat.Auth
- **Scopes**: Custom scope-based authorization with `ScopeRequirement`
- **Bearer Authentication**: Standard Authorization header format

### Ports and URLs
- **Development**: HTTP 5006, HTTPS 5007, gRPC 5016
- **Docker**: HTTP 8080, HTTPS 8081, gRPC 8082

### Infrastructure
- **Redis**: Used for presence data storage with TTL expiration
- **SignalR Redis Backplane**: Enables scaling across multiple instances

## SignalR Hub

### PresenceHub (`/_hubs/presence`)
- **Interface**: `IPresenceHubClient`
- **Authentication**: JWT Bearer token required
- **Auto-tracking**: Automatically handles online/offline on connect/disconnect

#### Hub Methods
- `GetUserStatus(string userId)` - Get current user status
- `SubscribeToUserStatus(string userId)` - Subscribe to user status changes
- `UnsubscribeFromUserStatus(string userId)` - Unsubscribe from user status
- `BroadcastOnlineStatus()` - Manually broadcast online status

#### Client Events
- `PresenceStatusChanged(string userId, string newStatus)` - Status change notification

#### Connection Lifecycle
- **OnConnectedAsync**: Sets user status to "online", broadcasts to subscribers
- **OnDisconnectedAsync**: Sets user status to "offline", broadcasts to subscribers

### Redis Backplane
- Enables SignalR scaling across multiple instances
- Configured via `AddStackExchangeRedis(connectionString)`
- Groups are managed for user status subscriptions

## gRPC Services

### PresenceAPIService
- **Proto Contract**: `MSChat.Shared.Contracts.PresenceAPI`
- **Authorization**: JWT Bearer token required
- **Usage**: Called by MSChat.NotificationWorker to check user presence

#### Methods
- `GetUsersStatus(GetUsersStatusRequest)` - Returns status for multiple users
  - **Request**: List of user IDs
  - **Response**: List of `UserStatus` objects with userId and status

## Services

### UserPresenceService
- **Interface**: `IUserPresenceService`
- **Implementation**: Redis-based presence storage

#### Methods
- `GetPresenceStatusAsync(string userId)` - Get user presence status
- `SetPresenceStatusAsync(string userId, string status)` - Set user presence status

#### Redis Storage
- **Key Pattern**: `user:{userId}:status`
- **TTL**: 10 seconds (auto-expires to offline)
- **Values**: "online" or "offline"

## Presence Status Constants

### PresenceStatus
- `Online = "online"` - User is connected and active
- `offline = "offline"` - User is disconnected or inactive

## Architecture Patterns

### Service Pattern
- Clean separation of concerns with `IUserPresenceService`
- Redis operations abstracted behind service interface

### Hub Pattern
- SignalR hub manages real-time connections
- Automatic lifecycle management (connect/disconnect)
- Group-based subscriptions for targeted notifications

### Authorization Pattern
- JWT-based authentication for all endpoints
- Custom scope requirements for API access
- Secure gRPC service endpoints

## Redis Integration

### Connection Management
- **IConnectionMultiplexer**: Singleton Redis connection
- **IDatabase**: Redis database operations
- **Connection String**: Configured via `RedisSettings`

### Data Storage Strategy
- **TTL-based expiration**: 10-second timeout for presence data
- **Automatic cleanup**: Redis handles expired keys
- **Fast retrieval**: String operations for optimal performance

### Key Management
- **Naming Convention**: `user:{userId}:status`
- **Collision Prevention**: User ID-based namespacing
- **Consistency**: Centralized key generation

## SignalR Groups

### User Status Subscriptions
- **Group Pattern**: `user:{userId}` for each user
- **Subscription Model**: Clients subscribe to specific users
- **Broadcast Targeting**: Only subscribers receive updates

### Connection Management
- **User Identification**: `Context.UserIdentifier` from JWT
- **Group Membership**: Dynamic add/remove based on subscriptions
- **Lifecycle Events**: Automatic status broadcasting

## Security Features

### Authentication & Authorization
- JWT token validation from MSChat.Auth
- Custom scope-based authorization with `ScopeRequirement`
- `ScopeHandler` for policy-based authorization
- SignalR hub authentication
- gRPC service authorization

### API Security
- HTTPS redirection enabled
- CORS configuration for cross-origin requests
- Secure Redis connections
- JWT token validation on all endpoints

## Integration Points

### MSChat.NotificationWorker
- **Purpose**: Check user presence before sending notifications
- **Method**: `GetUsersStatus` gRPC call
- **Logic**: Skip notifications for online users

### MSChat.Auth
- **Purpose**: JWT token validation and user identification
- **Integration**: Bearer token authentication
- **Claims**: User ID extraction from JWT

### Frontend Applications
- **Purpose**: Real-time presence updates
- **Integration**: SignalR hub connections
- **Features**: Subscribe to user status changes

## Performance Considerations

### Redis Optimizations
- **Connection Pooling**: Singleton connection multiplexer
- **TTL Strategy**: Automatic expiration prevents memory leaks
- **String Operations**: Fastest Redis data type for simple values

### SignalR Optimizations
- **Redis Backplane**: Horizontal scaling support
- **Group Targeting**: Efficient message broadcasting
- **Connection Lifecycle**: Automatic resource cleanup

### Memory Management
- **Stateless Service**: No in-memory state storage
- **Redis Dependency**: All state stored in Redis
- **Garbage Collection**: Minimal object allocation

## Monitoring and Observability

### Logging
- **ILogger**: Built-in ASP.NET Core logging
- **Connection Events**: SignalR connection lifecycle
- **Redis Operations**: Presence data changes

### Health Checks
- **Redis Connectivity**: Monitor Redis connection health
- **SignalR Hub**: Monitor active connections
- **gRPC Service**: Service availability checks

## Development Notes

### Current Implementation Status
- **SignalR Hub**: Fully implemented with real-time updates
- **Redis Storage**: Complete with TTL-based expiration
- **gRPC Service**: Ready for inter-service communication
- **JWT Authentication**: Integrated with MSChat.Auth
- **Docker Support**: Containerized deployment

### Architecture Decisions
- **Redis-First**: All presence data stored in Redis
- **TTL-Based**: Automatic offline detection via expiration
- **Real-Time**: SignalR for immediate status updates
- **Microservice**: Independent service with focused responsibility

### Future Enhancements
- **Presence History**: Track user activity patterns
- **Custom Statuses**: Support for away, busy, etc.
- **Presence Analytics**: Usage metrics and insights
- **Batch Operations**: Bulk presence updates

## Error Handling

### Redis Failures
- **Connection Resilience**: Automatic reconnection
- **Fallback Strategy**: Default to offline status
- **Error Logging**: Comprehensive error tracking

### SignalR Failures
- **Connection Drops**: Automatic status cleanup
- **Group Management**: Resilient group operations
- **Client Reconnection**: Graceful reconnection handling

### gRPC Failures
- **Service Unavailable**: Proper error responses
- **Authentication Failures**: Clear error messages
- **Request Validation**: Input validation and sanitization