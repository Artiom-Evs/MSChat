# MSChat.WebBFF Project

This file provides guidance specific to the MSChat.WebBFF Backend for Frontend service.

## Project Overview

MSChat.WebBFF is a minimal Backend for Frontend (BFF) service built with ASP.NET Core 9.0 that primarily serves the React SPA and provides runtime configuration. The service has evolved from a full API gateway to a focused SPA host since chat functionality moved to the dedicated MSChat.Chat service.

## Technology Stack

- **.NET 9.0** with ASP.NET Core
- **Minimal APIs** - Lightweight API approach
- **SPA Proxy** - React development integration
- **OpenAPI/Swagger** - API documentation

## Development Commands

```bash
# Run the WebBFF service
dotnet run --project MSChat.WebBFF

# Build the project
dotnet build MSChat.WebBFF

# Run with Docker
docker compose up mschat-webbff
```

## Configuration

### Ports and URLs
- **Development**: HTTP 5002, HTTPS 5003
- **Docker**: HTTP 8080, HTTPS 8081
- **React Proxy**: HTTPS 51188 (via SpaProxy)

### SPA Integration
- **React Project**: `../mschat.webclient`
- **Development**: Uses SpaProxy for hot reload
- **Production**: Serves React build from wwwroot
- **Proxy Command**: `npm run dev`

## API Endpoints

### Current Endpoints
- **GET /app-config** - Runtime configuration for React application
  - Returns configuration object with service URLs and settings
  - Used by React app to discover backend services
  - Environment-specific configuration support

### API Architecture
- Uses minimal APIs with single configuration endpoint
- No controllers or complex business logic
- OpenAPI documentation available in development
- Focus on serving React SPA and providing config

## Middleware Pipeline

Current middleware configuration:
1. `UseDefaultFiles()` - Serves index.html as default
2. `MapStaticAssets()` - Optimized static file serving
3. `MapOpenApi()` - OpenAPI docs (development only)
4. `UseHttpsRedirection()` - HTTPS redirection
5. `UseCors()` - Cross-origin requests
6. `MapFallbackToFile("/index.html")` - SPA routing fallback

## Security Considerations

### CORS Configuration
CORS is configured to allow cross-origin requests for development:
- Allows requests from React development server
- Configured for local development scenarios
- Should be restricted for production deployment

### Current Security Status
- **Authentication**: Not required for this minimal BFF
- **Authorization**: No protected endpoints currently
- **JWT Integration**: Not implemented (chat API handles auth)
- **Security Headers**: Basic HTTPS redirection configured

## React Frontend Integration

### Development Mode
- SPA proxy integrates with React dev server (port 51188)
- Hot reload support for React development
- Configuration endpoint provides runtime settings
- Vite handles API proxying to backend services

### Production Mode
- React build output served from wwwroot as static files
- Client-side routing via fallback to index.html
- Single configuration endpoint for service discovery
- Optimized static file serving with caching

## Key Dependencies

- **Microsoft.AspNetCore.OpenApi** - API documentation
- **Microsoft.AspNetCore.SpaProxy** - React development integration
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** - Docker support

## Current Implementation Status

### Service Role
- **Primary Function**: Serve React SPA and provide runtime configuration
- **Architecture**: Minimal BFF with single configuration endpoint
- **Dependencies**: React build output and configuration management

### Production-Ready Features
- **SPA Hosting**: Complete React application serving with static file optimization
- **Runtime Config**: `/app-config` endpoint for service discovery
- **Development Integration**: SPA proxy for hot reload development
- **Static File Serving**: Optimized serving with appropriate caching headers
- **Client Routing**: Fallback to index.html for React Router support

### Configuration Management
- **WebClientOptions**: Strongly-typed configuration for frontend settings
- **Environment-Specific**: Different configs for development/production
- **Service URLs**: Provides backend service endpoints to React app

## Integration Points

### Current Architecture
- **MSChat.Auth**: React app authenticates directly with auth service
- **MSChat.Chat**: React app calls chat API directly (no proxying)
- **Frontend**: Serves complete React application with Material-UI

### Service Dependencies
- **Minimal Dependencies**: No direct integration with other services
- **Configuration Provider**: Supplies service URLs to React application
- **Static Asset Host**: Serves React build in production

## Development Workflow

### Local Development
1. `dotnet run --project MSChat.WebBFF` - Start BFF service
2. SPA proxy automatically starts React dev server
3. React app gets configuration from `/app-config`
4. Hot reload works through Vite development server

### Production Deployment
1. React build copied to `wwwroot` directory
2. Static files served with optimized caching
3. Configuration endpoint provides runtime settings
4. Client-side routing handled via SPA fallback