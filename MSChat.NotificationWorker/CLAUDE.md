# MSChat.NotificationWorker - CLAUDE.md

## Project Overview

MSChat.NotificationWorker is a background service that handles email notifications for unread chat messages in the MSChat application. It operates as a message consumer using MassTransit/RabbitMQ and integrates with other MSChat services via gRPC and HTTP APIs.

## Architecture

### Core Components

- **Message Consumer**: `ChatMessageSentConsumer` processes `ChatMessageSent` events from RabbitMQ
- **Notification Service**: `NotificationService` orchestrates the notification logic
- **Email Service**: `EmailService` handles SMTP email sending using MailKit
- **API Clients**: gRPC clients for Chat API and Presence API, HTTP client for Auth API
- **Authentication**: JWT-based machine-to-machine authentication with token caching

### Technology Stack

- **.NET 9.0** - Runtime framework
- **MassTransit + RabbitMQ** - Message bus for consuming events
- **gRPC** - Communication with Chat API and Presence API
- **HTTP Client** - Communication with Auth API
- **MailKit** - SMTP email sending
- **Duende.IdentityModel** - JWT token handling

## Key Services

### NotificationService (`Services/NotificationService.cs`)
Main business logic service that orchestrates the notification workflow:
- Retrieves chat memberships from Chat API via gRPC
- Checks user presence status via Presence API via gRPC
- Fetches user information from Auth API via HTTP
- Sends email notifications to offline users with unread messages
- Implements comprehensive error handling and logging

### EmailService (`Services/EmailService.cs`)
Handles email composition and sending using MailKit SMTP client:
- **Method**: `SendUnreadMessageNotificationAsync(UnreadMessageInfo messageInfo)`
- **Email Content**: Formatted unread message notifications
- **SMTP Support**: Full SSL/TLS support with authentication
- **Error Handling**: Proper connection management and cleanup

### AccessTokenProvider (`Services/AccessTokenProvider.cs`)
Manages JWT token acquisition for machine-to-machine authentication:
- **OAuth2 Flow**: Client credentials flow for service-to-service authentication
- **Token Caching**: In-memory caching with expiration handling
- **Scopes**: Requests "chatapi presenceapi" scopes for API access
- **Refresh Logic**: Automatic token refresh before expiration

### AuthAPIClient (`Services/AuthAPIClient.cs`)
HTTP client wrapper for Auth API communication:
- **Method**: `GetUserInfoAsync(string userId)` - Fetches user information
- **Authentication**: Bearer token authentication via AccessTokenProvider
- **Error Handling**: Graceful handling of API failures
- **JSON Serialization**: Automatic JSON deserialization

## Configuration Classes

### EmailSettings (`Configuration/EmailSettings.cs`)
Strongly-typed configuration for email settings:
- `SmtpServer` - SMTP server hostname
- `SmtpPort` - SMTP server port
- `Username` - SMTP authentication username
- `Password` - SMTP authentication password
- `FromEmail` - Sender email address
- `FromName` - Optional sender display name

### ServicesSettings (`Configuration/ServicesSettings.cs`)
Configuration for external service endpoints:
- `ChatApiUri` - Chat API gRPC endpoint
- `PresenceApiUri` - Presence API gRPC endpoint
- `AuthApiUri` - Auth API HTTP endpoint

### ConfigurationExtensions (`Configuration/ConfigurationExtensions.cs`)
Extension methods for configuration validation and binding.

## Message Consumer

### ChatMessageSentConsumer (`Consumers/ChatMessageSentConsumer.cs`)
MassTransit consumer that processes chat message events:
- **Event Type**: `ChatMessageSent` from MSChat.Shared.Events
- **Processing**: 5-second delay followed by notification processing
- **Error Handling**: Automatic retry and error logging
- **Dependency Injection**: Uses `INotificationService` for business logic

## Configuration

The service requires several configuration sections in `appsettings.json`:

### RabbitMQSettings
```json
{
  "RabbitMQSettings": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### ServicesSettings
```json
{
  "ServicesSettings": {
    "ChatApiUri": "http://localhost:5014",
    "PresenceApiUri": "http://localhost:5016",
    "AuthApiUri": "http://localhost:5000"
  }
}
```

### EmailSettings
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 465,
    "Username": "notifications@example.com",
    "Password": "app-password",
    "FromEmail": "notifications@example.com",
    "FromName": "MSChat Notifications"
  }
}
```

### M2MAuthSettings
```json
{
  "M2MAuthSettings": {
    "Authority": "http://localhost:5000",
    "ClientId": "notificationworker",
    "ClientSecret": "secret",
    "Scope": "chatapi presenceapi"
  }
}
```

## Message Flow

1. **Event Reception**: `ChatMessageSentConsumer` receives `ChatMessageSent` events from RabbitMQ
2. **Processing Delay**: 5-second delay (TODO: replace with persistent storage)
3. **Membership Retrieval**: Fetches chat memberships from Chat API via gRPC
4. **Presence Check**: Queries user presence status from Presence API via gRPC
5. **Notification Logic**: For each offline user with unread messages:
   - Fetches user info from Auth API via HTTP
   - Sends email notification via EmailService
   - Logs notification success/failure

## Service Integration

### Chat API Integration
- **Protocol**: gRPC
- **Service**: `ChatAPI.ChatAPIClient`
- **Methods**: `GetChatMembershipsAsync(GetChatMembershipsRequest)`
- **Authentication**: JWT Bearer token via gRPC metadata
- **Purpose**: Retrieve chat memberships for notification targeting

### Presence API Integration
- **Protocol**: gRPC
- **Service**: `PresenceAPI.PresenceAPIClient`
- **Methods**: `GetUsersStatusAsync(GetUsersStatusRequest)`
- **Authentication**: JWT Bearer token via gRPC metadata
- **Purpose**: Check user online status to avoid notifying online users

### Auth API Integration
- **Protocol**: HTTP
- **Methods**: `GET /api/users/{userId}/info`
- **Authentication**: JWT Bearer token via HTTP header
- **Purpose**: Fetch user email and display name for notifications

## Security Features

### Authentication & Authorization
- **OAuth2 Client Credentials**: Machine-to-machine authentication
- **JWT Token Caching**: Efficient token reuse with expiration handling
- **Secure Credential Storage**: Configuration-based credential management
- **gRPC Security**: Bearer token authentication for all gRPC calls

### Email Security
- **SMTP Authentication**: Username/password authentication
- **SSL/TLS Support**: Secure email transmission
- **Connection Management**: Proper connection cleanup and disposal

## Error Handling

### Notification Service
- **User Info Failures**: Graceful handling of missing user information
- **API Failures**: Comprehensive error logging and recovery
- **Email Failures**: SMTP error handling and connection management

### Consumer Resilience
- **MassTransit Retry**: Automatic retry for transient failures
- **Error Logging**: Comprehensive error tracking and diagnostics
- **Circuit Breaker**: Prevents cascading failures

## Docker Support

### Dockerfile
- **Base Image**: .NET 9.0 runtime
- **Multi-stage Build**: Optimized container size
- **Linux Containers**: Configured for Linux deployment

### Docker Compose Integration
- **Service Name**: `nworker`
- **Dependencies**: RabbitMQ, Auth API, Chat API, Presence API
- **Environment Variables**: Complete configuration via environment

## Development Notes

### Current Implementation Status
- **Event Processing**: Fully implemented with MassTransit
- **Multi-API Integration**: Complete gRPC and HTTP integration
- **Email Notifications**: Production-ready SMTP implementation
- **JWT Authentication**: Secure machine-to-machine authentication
- **Docker Support**: Full containerization support

### Current Limitations
- **Processing Delay**: Uses `Task.Delay(5 seconds)` instead of persistent storage
- **Error Recovery**: Basic error handling for missing user information
- **Batch Processing**: Processes notifications individually

### Architecture Decisions
- **Event-Driven**: Decoupled from chat operations via message queue
- **Multi-Protocol**: gRPC for internal APIs, HTTP for external APIs
- **Token Caching**: Efficient JWT token reuse with expiration
- **Service Isolation**: Independent service with focused responsibility

### Integration Points
- **Chat API**: gRPC communication for chat memberships
- **Presence API**: gRPC communication for user presence status
- **Auth API**: HTTP communication for user information
- **RabbitMQ**: Message consumption for chat events
- **SMTP Server**: Email delivery via configured SMTP

### Performance Considerations
- **Token Caching**: Reduces authentication overhead
- **Async Operations**: Non-blocking I/O for all external calls
- **Connection Pooling**: Efficient HTTP and gRPC connection management
- **Resource Cleanup**: Proper disposal of resources

## Build and Run

```bash
# Build the project
dotnet build

# Run the service
dotnet run --project MSChat.NotificationWorker

# Run with Docker
docker build -t mschat-notification-worker .
docker run mschat-notification-worker

# Run via Docker Compose
docker compose up nworker
```

## Dependencies

Key NuGet packages:
- `MassTransit` (8.5.1) - Message bus framework
- `MassTransit.RabbitMQ` (8.5.1) - RabbitMQ transport
- `Grpc.AspNetCore` (2.71.0) - gRPC client support
- `MailKit` (4.13.0) - Email sending
- `Duende.IdentityModel` (7.1.0) - JWT token handling

## Project References
- `MSChat.Shared` - Shared contracts, events, and configuration models

## Future Enhancements

### Immediate Improvements
- **Persistent Storage**: Replace Task.Delay with database-backed scheduling
- **Batch Processing**: Process multiple notifications in batches
- **Retry Logic**: Implement exponential backoff for failed notifications

### Advanced Features
- **Template Engine**: Rich email templates with HTML support
- **Notification Preferences**: User-configurable notification settings
- **Analytics**: Track notification delivery and engagement metrics
- **Multi-Channel**: Support for SMS, push notifications, etc.