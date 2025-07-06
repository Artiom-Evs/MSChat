# MSChat.Chat Project

This file provides guidance specific to the MSChat.Chat API service.

## Project Overview

MSChat.Chat is a RESTful API service built with ASP.NET Core that provides chat functionality including chat room management, messaging, and participant handling for the MSChat application ecosystem.

## Technology Stack

- **.NET 9.0** with ASP.NET Core
- **Entity Framework Core** - Data access and ORM
- **SQL Server** - Database storage
- **JWT Authentication** - Token-based security
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization support

## Development Commands

```bash
# Run the Chat API service
dotnet run --project MSChat.Chat

# Build the project
dotnet build MSChat.Chat

# Run with Docker
docker compose up mschat-chat

# Database migrations
dotnet ef database update --project MSChat.Chat
dotnet ef migrations add <MigrationName> --project MSChat.Chat
```

## Configuration

### Database Configuration
- **Development**: SQL Server with connection string in appsettings
- **Production**: SQL Server container via docker-compose
- **Entity Framework**: Code-first approach with migrations
- **Schema**: Separate tables for chats, messages, members, and participant links

### Authentication
- **JWT Tokens**: Validates tokens issued by MSChat.Auth
- **Scopes**: Custom scope-based authorization
- **Bearer Authentication**: Standard Authorization header format

### Ports and URLs
- **Development**: HTTP 5004, HTTPS 5005
- **Docker**: HTTP 8080, HTTPS 8081
- **Swagger UI**: Available at `/swagger` endpoint

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

## Data Models

### Core Entities

#### Chat
- `Id` (int) - Primary key
- `Name` (string) - Chat name/title
- `Description` (string) - Chat description
- `CreatedAt` (DateTime) - Creation timestamp
- `IsActive` (bool) - Active status

#### Message
- `Id` (int) - Primary key
- `ChatId` (int) - Foreign key to Chat
- `SenderId` (string) - User ID of sender
- `Content` (string) - Message content
- `MessageType` (string) - Type of message
- `SentAt` (DateTime) - Timestamp
- `IsDeleted` (bool) - Soft delete flag

#### ChatMember
- `Id` (int) - Primary key
- `UserId` (string) - User identifier
- `DisplayName` (string) - Display name
- `Email` (string) - Email address
- `JoinedAt` (DateTime) - Join timestamp
- `IsActive` (bool) - Active status

#### ChatMemberLink
- `Id` (int) - Primary key
- `ChatId` (int) - Foreign key to Chat
- `ChatMemberId` (int) - Foreign key to ChatMember
- `JoinedAt` (DateTime) - Join timestamp
- `Role` (string) - Member role in chat
- `IsActive` (bool) - Active status

## Security Features

### Authentication & Authorization
- JWT token validation from MSChat.Auth
- Custom scope-based authorization with `ScopeRequirement`
- `ScopeHandler` for policy-based authorization
- Bearer token authentication scheme

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

### Entity Relationships
- Chat → Messages (One-to-Many)
- Chat → ChatMemberLinks (One-to-Many)
- ChatMember → ChatMemberLinks (One-to-Many)
- Many-to-Many: Chat ↔ ChatMember (via ChatMemberLink)

## Service Layer

### Business Logic Services
- **ChatsService** - Chat management operations
- **MessagesService** - Message handling and storage
- **MembersService** - Member management
- **ChatParticipantsService** - Participant relationship management

### Service Interfaces
- `IChatsService` - Chat operations contract
- `IMessagesService` - Message operations contract
- `IMembersService` - Member operations contract
- `IChatParticipantsService` - Participant operations contract

## Development Notes

### Current Implementation Status
- Complete CRUD operations for all entities
- JWT authentication integration
- Database migrations and schema
- Swagger API documentation
- Docker containerization support

### Integration Points
- **MSChat.Auth**: Validates JWT tokens for authentication
- **MSChat.WebBFF**: Will proxy API requests to this service
- **Frontend**: React app will consume these API endpoints

## Memory and Context

### Current Status
- **Service Role**: Chat functionality API providing messaging and participant management
- **Active Features**: CRUD operations, JWT authentication, database storage, API documentation
- **Database**: SQL Server with Entity Framework Core, migrations applied

### Recent Development
- Implemented complete chat management API
- Added JWT authentication with custom scope authorization
- Created database schema with proper relationships
- Set up service layer with business logic separation
- Configured Swagger documentation for API testing

### Architecture Decisions
- **Service Pattern**: Separate service classes for business logic
- **Repository Pattern**: Entity Framework DbContext as repository
- **Authorization**: Custom scope-based authorization instead of role-based
- **Data Models**: Separate entities for flexibility and normalization

### API Design Patterns
- **RESTful Design**: Standard HTTP methods and status codes
- **Async/Await**: All service operations are asynchronous
- **Dependency Injection**: Services registered in DI container
- **Separation of Concerns**: Controllers, services, and data access layers

### Integration Status
- **MSChat.Auth**: JWT token validation working
- **Database**: Migrations applied, schema ready
- **MSChat.WebBFF**: Not yet integrated for API proxying
- **Frontend**: Not yet connected to API endpoints
