# MSChat.WebClient Project

This file provides guidance specific to the MSChat.WebClient React frontend application.

## Project Overview

MSChat.WebClient is a modern React application built with TypeScript and Vite, serving as the frontend user interface for the MSChat system.

## Technology Stack

- **React 19.1.0** - Latest React with modern features
- **TypeScript 5.8.3** - Type safety and modern JavaScript
- **Vite 7.0.0** - Fast build tool and development server
- **ESLint** - Code quality and linting

## Development Commands

```bash
# Navigate to project directory
cd mschat.webclient

# Install dependencies
npm install

# Development server with hot reload
npm run dev

# Build for production
npm run build

# Lint code
npm run lint

# Preview production build
npm run preview
```

## Configuration

### Development Server
- **Port**: 51188 (configurable via `DEV_SERVER_PORT`)
- **HTTPS**: Enabled with auto-generated certificates
- **Certificate**: Managed via `dotnet dev-certs`
- **Proxy**: API requests to `/weatherforecast` proxied to backend

### Build Configuration
- **Target**: ES2022 for modern browsers
- **Output**: `dist/` directory
- **Path Alias**: `@` → `./src`
- **TypeScript**: Strict mode enabled

## Project Structure

```
src/
├── main.tsx          # Application entry point
├── App.tsx           # Main application component
├── App.css           # Component-specific styles
├── index.css         # Global styles and theme
└── vite-env.d.ts     # Vite TypeScript definitions
```

## Component Architecture

### Current Components
- **App.tsx** - Main weather forecast display component
- Uses functional components with React hooks
- TypeScript interfaces for type safety (Forecast interface)
- useState for local state management
- useEffect for data fetching

### Data Fetching Pattern
```typescript
// Current API integration pattern
useEffect(() => {
  fetch('/weatherforecast')
    .then(response => response.json())
    .then(data => setForecasts(data));
}, []);
```

## Styling Approach

### CSS Strategy
- Traditional CSS files with component-specific stylesheets
- Global styles in `index.css` with theme support
- Component styles in `App.css`

### Design System
- System fonts (system-ui, Avenir, Helvetica, Arial)
- CSS custom properties for theming
- Dark/light mode support via `prefers-color-scheme`
- Bootstrap-like table styling

## API Integration

### Current Integration
- **Endpoint**: `/weatherforecast`
- **Method**: Native fetch API
- **Error Handling**: Basic response.ok checking
- **Typing**: TypeScript interfaces for API responses

### Proxy Configuration
Development API requests are proxied through Vite:
```typescript
// In vite.config.ts
proxy: {
  '/weatherforecast': {
    target: 'https://localhost:7127',
    secure: true,
    changeOrigin: true
  }
}
```

## TypeScript Configuration

### Strict Settings
- Strict mode enabled
- Unused variable/parameter checking
- Modern JSX transform (react-jsx)
- ES2022 target with bundler resolution

### Build Configs
- `tsconfig.app.json` - Application TypeScript config
- `tsconfig.node.json` - Build tools TypeScript config

## Code Quality

### ESLint Configuration
- Modern flat config format
- JavaScript and TypeScript recommended rules
- React Hooks linting rules
- React Refresh rules for Vite HMR
- Browser environment with ES2020 globals

## Visual Studio Integration

### Project Files
- `.esproj` file for Visual Studio integration
- MSBuild integration with .NET solution
- Startup command: `npm run dev`

## Development Notes

### Current State
- Template-level implementation showing weather forecast
- Single component architecture
- Basic API integration pattern
- No routing or complex state management yet

### Recommended Improvements
1. Add React Router for navigation
2. Implement proper state management (Context API or Redux)
3. Add authentication integration with MSChat.Auth
4. Create reusable component library
5. Add proper error boundaries and loading states
6. Implement chat-specific UI components
7. Add comprehensive testing setup

### Integration Points
- Receives authentication tokens from MSChat.Auth
- Communicates with MSChat.WebBFF for API calls
- Will implement chat functionality UI
- Designed for production deployment via WebBFF static files

## Certificate Management

Development HTTPS certificates are automatically managed:
- Generated via `dotnet dev-certs https`
- Trusted for local development
- Integrated with Vite dev server configuration