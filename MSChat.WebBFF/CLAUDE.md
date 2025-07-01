# MSChat.WebBFF Project

This file provides guidance specific to the MSChat.WebBFF Backend for Frontend service.

## Project Overview

MSChat.WebBFF is a Backend for Frontend (BFF) service built with ASP.NET Core 9.0 that serves as both an API gateway and static file server for the React frontend application.

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
docker-compose up mschat-webbff
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
- **GET /weatherforecast** - Sample weather forecast data
  - Returns 5-day weather forecast with random data
  - Named "GetWeatherForecast" for OpenAPI

### API Architecture
- Uses minimal APIs (`app.MapGet()`)
- No traditional controllers or services yet
- OpenAPI documentation available in development

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
**CRITICAL**: Current CORS setup is insecure for production:
```csharp
// This configuration is too permissive
options.AllowAnyOrigin()
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials(); // Problematic with AllowAnyOrigin
```

### Missing Security Features
- No authentication middleware
- No authorization policies
- No integration with MSChat.Auth
- No JWT token validation
- No rate limiting or security headers

## React Frontend Integration

### Development Mode
- SPA proxy integrates React dev server
- Vite serves frontend on port 51188
- API calls proxied through Vite configuration
- Hot reload and debugging support

### Production Mode
- React build output copied to wwwroot
- Static files served by ASP.NET Core
- Client-side routing via fallback to index.html
- No SPA proxy in production

## Key Dependencies

- **Microsoft.AspNetCore.OpenApi** - API documentation
- **Microsoft.AspNetCore.SpaProxy** - React development integration
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** - Docker support

## Development Notes

### Current State
- Template-level implementation with minimal business logic
- Only demo weather forecast endpoint
- No real chat functionality implemented
- Basic logging and error handling

### Recommended Improvements
1. Implement proper CORS policies for production
2. Integrate authentication with MSChat.Auth service
3. Add JWT token validation middleware
4. Implement real API endpoints for chat functionality
5. Add proper error handling and logging
6. Include health checks and monitoring
7. Add rate limiting and security headers

### Integration Points
- Should authenticate users via MSChat.Auth
- Will serve as API gateway for chat functionality
- Hosts React SPA in production deployments
- Provides backend services for frontend features