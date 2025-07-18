# MSChat.ChatAPI Project

This file provides guidance specific to the MSChat.ChatAPI service.

## Project Overview

MSChat.ChatAPI is a comprehensive API service built with ASP.NET Core that provides chat functionality including chat room management, messaging, and participant handling. It includes RESTful APIs, SignalR hubs for real-time communication, and gRPC services for inter-service communication.

## Technology Stack

- **.NET 9.0** with ASP.NET Core
- **Entity Framework Core** - Data access and ORM
- **SQL Server** - Database storage
- **SignalR** - Real-time communication with Redis backplane
- **gRPC** - Inter-service communication
- **MediatR** - CQRS pattern implementation
- **MassTransit + RabbitMQ** - Message queuing for notifications
- **JWT Authentication** - Token-based security
- **OpenAPI/Swagger** - API documentation
- **Docker** - Containerization support

## Development Commands

```bash
# Run the Chat API service
dotnet run --project MSChat.ChatAPI

# Build the project
dotnet build MSChat.ChatAPI

# Run with Docker
docker compose up chatapi

# Database migrations
dotnet ef database update --project MSChat.ChatAPI
dotnet ef migrations add <MigrationName> --project MSChat.ChatAPI
```

## Configuration

### Database Configuration
- **Development**: SQL Server with connection string in appsettings
- **Production**: SQL Server container via docker-compose
- **Entity Framework**: Code-first approach with migrations
- **Schema**: Separate tables for chats, messages, members, and memberships

### Authentication
- **JWT Tokens**: Validates tokens issued by MSChat.Auth
- **Scopes**: Custom scope-based authorization with `ScopeRequirement`
- **Bearer Authentication**: Standard Authorization header format

### Ports and URLs
- **Development**: HTTP 5004, HTTPS 5005, gRPC 5014
- **Docker**: HTTP 8080, HTTPS 8081, gRPC 8082
- **Swagger UI**: Available at `/swagger` endpoint

### Infrastructure
- **Redis**: Used for SignalR backplane scaling
- **RabbitMQ**: Message queuing for background notifications
- **SQL Server**: Primary data storage

## API Endpoints

### Chat Management
- **GET /api/chats** - List all chats
- **POST /api/chats** - Create new chat
- **GET /api/chats/{id}** - Get chat by ID
- **PUT /api/chats/{id}** - Update chat
- **DELETE /api/chats/{id}** - Delete chat

### Message Management
- **GET /api/messages** - List messages (with filtering)
- **POST /api/messages** - Send new message
- **GET /api/messages/{id}** - Get message by ID
- **PUT /api/messages/{id}** - Update message
- **DELETE /api/messages/{id}** - Delete message

### Member Management
- **GET /api/members** - List chat members
- **POST /api/members** - Add member
- **GET /api/members/{id}** - Get member by ID
- **PUT /api/members/{id}** - Update member
- **DELETE /api/members/{id}** - Remove member

### Participant Management
- **GET /api/chatparticipants** - List chat participants
- **POST /api/chatparticipants** - Add participant to chat
- **GET /api/chatparticipants/{id}** - Get participant by ID
- **PUT /api/chatparticipants/{id}** - Update participant
- **DELETE /api/chatparticipants/{id}** - Remove participant

## SignalR Hub

### ChatHub (`//_hubs/chat`)
- **Interface**: `IChatHubClient`
- **Methods**: 
  - `SubscribeToChat(string chatId)` - Join chat group
  - `UnsubscribeFromChat(string chatId)` - Leave chat group
- **Client Events**:
  - `NewMessageSent(MessageDto newMessage)` - New message notification
  - `MessageUpdated(MessageDto updatedMessage)` - Message update notification
  - `MessageDeleted(long messageId)` - Message deletion notification

### Redis Backplane
- Enables SignalR scaling across multiple instances
- Configured via `AddStackExchangeRedis(connectionString)`

## gRPC Services

### ChatAPIService
- **Proto Contract**: `MSChat.Shared.Contracts.ChatAPI`
- **Methods**:
  - `GetChatMemberships(GetChatMembershipsRequest)` - Returns chat memberships for notifications
- **Authorization**: JWT Bearer token required
- **Usage**: Called by MSChat.NotificationWorker for email notifications

## Data Models

### Core Entities

#### Chat
- `Id` (long) - Primary key
- `Name` (string) - Chat name/title
- `Description` (string) - Chat description
- `ChatType` (enum) - Personal/Public chat type
- `CreatedAt` (DateTime) - Creation timestamp
- `IsActive` (bool) - Active status

#### Message
- `Id` (long) - Primary key
- `IdInChat` (long) - Sequential ID within chat
- `ChatId` (long) - Foreign key to Chat
- `SenderId` (string) - User ID of sender
- `Content` (string) - Message content
- `SentAt` (DateTime) - Timestamp
- `UpdatedAt` (DateTime?) - Last update timestamp
- `IsDeleted` (bool) - Soft delete flag

#### ChatMember
- `Id` (long) - Primary key
- `UserId` (string) - User identifier
- `DisplayName` (string) - Display name
- `Email` (string) - Email address
- `JoinedAt` (DateTime) - Join timestamp
- `IsActive` (bool) - Active status

#### ChatMembership
- `Id` (long) - Primary key
- `ChatId` (long) - Foreign key to Chat
- `MemberId` (long) - Foreign key to ChatMember
- `JoinedAt` (DateTime) - Join timestamp
- `Role` (string) - Member role in chat
- `LastReadMessageId` (long?) - Last read message for notifications
- `IsActive` (bool) - Active status

### DTOs
- Comprehensive set of DTOs for all operations
- Located in `Models/DTOs/` folder
- Includes Create, Update, and response DTOs

## CQRS Pattern with MediatR

### Commands
- `SendMessageCommand` - Send new message
- `UpdateMessageCommand` - Update existing message
- `DeleteMessageCommand` - Delete message

### Queries
- `GetMessagesQuery` - Retrieve messages with filtering
- `GetMessageQuery` - Get single message
- `GetChatMembershipsQuery` - Get memberships for gRPC

### Handlers
- Separate handlers for each command/query
- Located in `Handlers/` folder
- Async operations with proper error handling

## Security Features

### Authentication & Authorization
- JWT token validation from MSChat.Auth
- Custom scope-based authorization with `ScopeRequirement`
- `ScopeHandler` for policy-based authorization
- Bearer token authentication scheme
- SignalR hub authorization

### API Security
- HTTPS redirection enabled
- CORS configuration for cross-origin requests
- Input validation and model binding
- SQL injection protection via Entity Framework

## Database Schema

### Migrations
- `20250702170134_Initial` - Initial database schema
- `20250703011557_AddUserIdFieldToChatMember` - Added UserId field
- `20250703025939_AddManyToManyForChatMemberLink` - Many-to-many relationship
- `20250708023551_AddIdInChatFieldToMessages` - Sequential message IDs
- `20250708053253_RenameChatMembershipsTableAndAddLastReadedMessageIdField` - Notification support

### Entity Relationships
- Chat → Messages (One-to-Many)
- Chat → ChatMemberships (One-to-Many)
- ChatMember → ChatMemberships (One-to-Many)
- Many-to-Many: Chat ↔ ChatMember (via ChatMembership)

## Service Layer

### Business Logic Services
- **ChatsService** - Chat management operations
- **MessagesService** - Message handling and storage
- **MembersService** - Member management
- **ChatParticipantsService** - Participant relationship management
- **ChatMessageIdService** - Sequential message ID generation
- **MemberRegistrationService** - Auto-registration of users

### Service Interfaces
- `IChatsService` - Chat operations contract
- `IMessagesService` - Message operations contract
- `IMembersService` - Member operations contract
- `IChatParticipantsService` - Participant operations contract
- `IChatMessageIdService` - Message ID service contract
- `IMemberRegistrationService` - Member registration contract

## Middleware

### MemberRegistrationMiddleware
- Automatically registers users from JWT claims
- Ensures user exists in ChatMember table
- Runs on every authenticated request

## Message Queuing

### RabbitMQ Integration
- **MassTransit** for message publishing
- **ChatMessageSent** events published for notifications
- Consumed by MSChat.NotificationWorker

## Current Implementation Status

### Fully Implemented Features
- Complete CRUD operations for all entities
- JWT authentication integration with custom scopes
- SignalR real-time messaging with Redis backplane
- gRPC service for inter-service communication
- MediatR CQRS pattern implementation
- Message queuing for background notifications
- Database migrations and schema
- Swagger API documentation
- Docker containerization support
- Sequential message IDs within chats
- Soft delete for messages
- Member auto-registration

### Integration Points
- **MSChat.Auth**: Validates JWT tokens for authentication
- **MSChat.NotificationWorker**: Consumes chat message events, calls gRPC endpoints
- **MSChat.PresenceAPI**: Independent service for user presence
- **MSChat.WebBFF**: Proxies API requests to this service
- **Frontend**: React app consumes REST APIs and SignalR hub

## Architecture Decisions

### Design Patterns
- **Service Pattern**: Separate service classes for business logic
- **Repository Pattern**: Entity Framework DbContext as repository
- **CQRS Pattern**: MediatR for command/query separation
- **Hub Pattern**: SignalR for real-time communication
- **Middleware Pattern**: Custom middleware for user registration

### Authorization Strategy
- Custom scope-based authorization instead of role-based
- Centralized authorization policies
- JWT token validation with custom requirements

### Data Strategy
- Separate entities for flexibility and normalization
- DTOs for API contract stability
- Soft deletes for audit trails
- Sequential message IDs for better UX

### Integration Strategy
- RESTful APIs for external clients
- gRPC for internal service communication
- SignalR for real-time updates
- Message queuing for decoupled background processing

## Development Notes

### Testing Strategy
- Unit tests for services and handlers
- Integration tests for controllers and hubs
- gRPC service testing

### Performance Considerations
- Redis backplane for SignalR scaling
- Entity Framework query optimization
- Async/await throughout
- Proper connection string configuration

### Monitoring and Logging
- Structured logging with built-in ILogger
- Health checks for dependencies
- Performance metrics collection