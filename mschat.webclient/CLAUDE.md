# MSChat.WebClient Project

This file provides guidance specific to the MSChat.WebClient React frontend application.

## Project Overview

MSChat.WebClient is a modern React application built with TypeScript and Vite, serving as the frontend user interface for the MSChat system.

## Technology Stack

- **React 19.1.0** - Latest React with modern features
- **TypeScript 5.8.3** - Type safety and modern JavaScript
- **Vite 7.0.0** - Fast build tool and development server
- **Material-UI 7.2.0** - React components and design system
- **React Router 7.6.3** - Client-side routing
- **React Query 5.81.5** - Server state management
- **React OIDC Context 3.3.0** - OpenID Connect authentication
- **Zustand 5.0.6** - Client state management
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
- **Proxy**: API requests proxied to MSChat.Auth and MSChat.Chat services

### Build Configuration
- **Target**: ES2022 for modern browsers
- **Output**: `dist/` directory
- **Path Alias**: `@` → `./src`
- **TypeScript**: Strict mode enabled

## Project Structure

```
src/
├── main.tsx              # Application entry point
├── App.tsx               # Main application component
├── index.css             # Global styles and theme
├── config.ts             # Runtime configuration
├── components/           # Reusable React components
│   ├── Layout.tsx        # Main layout wrapper
│   ├── Router.tsx        # Application routing
│   ├── ChatList.tsx      # Chat list display
│   ├── ChatListItem.tsx  # Individual chat item
│   ├── MessageList.tsx   # Message display list
│   ├── MessageItem.tsx   # Individual message
│   ├── MessageForm.tsx   # Message input form
│   ├── Sidebar.tsx       # Navigation sidebar
│   ├── UserProfile.tsx   # User profile display
│   └── index.ts          # Component exports
├── pages/                # Page components
│   ├── ChatPage.tsx      # Main chat interface
│   ├── CreateChatPage.tsx # Chat creation
│   ├── UpdateChatPage.tsx # Chat editing
│   ├── MemberPage.tsx    # Member management
│   ├── SearchPage.tsx    # Search functionality
│   └── index.ts          # Page exports
├── hooks/                # Custom React hooks
│   ├── useChats.ts       # Chat data management
│   ├── useMessages.ts    # Message operations
│   ├── useMembers.ts     # Member operations
│   ├── useParticipants.ts # Participant management
│   └── useAuthToken.ts   # Authentication token
├── lib/                  # Utility libraries
│   ├── chatApi.ts        # API client
│   └── queryClient.ts    # React Query setup
├── auth/                 # Authentication
│   └── oidcSettings.ts   # OIDC configuration
├── types/                # TypeScript definitions
│   ├── Chat.ts           # Chat-related types
│   └── index.ts          # Type exports
├── utils/                # Utility functions
│   └── dateUtils.ts      # Date formatting
└── vite-env.d.ts         # Vite TypeScript definitions
```

## Component Architecture

### Application Structure
- **Material-UI Design System**: Consistent component library with theming
- **Functional Components**: All components use React hooks pattern
- **TypeScript**: Strong typing throughout the application
- **Route-Based Pages**: React Router 7 for navigation
- **Protected Routes**: Authentication-gated access to chat features

### Key Components
- **Layout.tsx**: Main application shell with sidebar and content area
- **Router.tsx**: Route definitions with authentication guards
- **ChatList.tsx**: Displays user's chats with search and filtering
- **MessageList.tsx**: Real-time message display with pagination
- **MessageForm.tsx**: Message composition with send functionality
- **UserProfile.tsx**: User information and authentication status

### Data Management Pattern
```typescript
// React Query for server state
const { data: chats, isLoading } = useChats(searchTerm);

// Custom hooks for API operations
const createMessageMutation = useCreateMessage();
const { mutate: sendMessage } = createMessageMutation;

// Zustand for client state
const useStore = create((set) => ({
  selectedChatId: null,
  setSelectedChatId: (id) => set({ selectedChatId: id })
}));
```

## Styling Approach

### Material-UI Design System
- **Component Library**: Material-UI 7.2.0 with comprehensive component set
- **Theming**: Centralized theme configuration with Material Design principles
- **Responsive Design**: Built-in responsive breakpoints and grid system
- **Typography**: Material Design typography system
- **Icons**: Material Icons integrated throughout the application

### Custom Styling
- **Global Styles**: `index.css` for application-wide styles and CSS reset
- **CSS-in-JS**: Emotion styling engine with Material-UI
- **Theme Customization**: Custom color palette and component overrides
- **Responsive Layout**: Mobile-first design with responsive components

## API Integration

### Production API Integration
- **MSChat.Chat API**: Full REST API integration for chat functionality
- **MSChat.Auth**: OpenID Connect authentication flow
- **MSChat.WebBFF**: Runtime configuration endpoint
- **React Query**: Server state management with caching and synchronization
- **Custom Hooks**: Abstracted API operations (useChats, useMessages, etc.)

### Authentication Flow
- **OIDC Provider**: MSChat.Auth service integration
- **React OIDC Context**: Handles authentication state and token management
- **JWT Tokens**: Automatic token refresh and API authorization
- **Protected Routes**: Authentication-gated access to application features

### API Client Architecture
```typescript
// Centralized API client with authentication
const apiClient = axios.create({
  baseURL: config.chatApiUrl,
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

// Custom hooks for API operations
export const useChats = (searchTerm?: string) => {
  return useQuery({
    queryKey: ['chats', searchTerm],
    queryFn: () => chatApi.getChats(searchTerm)
  });
};
```

### Proxy Configuration
Development API requests are proxied through Vite:
```typescript
// In vite.config.ts
proxy: {
  '/api': {
    target: 'https://localhost:5004', // MSChat.Chat
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

## Current Implementation Status

### Production-Ready Features
- **Complete Chat Application**: Full-featured messaging interface
- **Authentication**: OIDC integration with MSChat.Auth
- **Real-time Interface**: Message sending, editing, and deletion
- **Chat Management**: Create, edit, and delete chats
- **User Management**: Member profiles and participant management
- **Responsive Design**: Material-UI components with mobile support
- **State Management**: React Query + Zustand for optimal performance

### Implemented Features
- **User Authentication**: Login/logout with OIDC flow
- **Chat List**: Search and filter user's chats
- **Messaging**: Send, edit, delete messages with real-time updates
- **Chat Types**: Support for Personal and Public chat types
- **Participants**: Join/leave chats, view member lists
- **Navigation**: React Router with protected routes
- **Error Handling**: User-friendly error messages and loading states

### Architecture Decisions
- **Material-UI**: Comprehensive design system for consistent UI
- **React Query**: Server state management with automatic caching
- **React OIDC Context**: Simplified authentication state management
- **Custom Hooks**: Abstracted API operations for reusability
- **TypeScript**: Strong typing throughout the application
- **Responsive Design**: Mobile-first approach with Material-UI breakpoints

### Integration Status
- **MSChat.Auth**: Full OIDC authentication integration
- **MSChat.Chat**: Complete REST API integration
- **MSChat.WebBFF**: Runtime configuration and static file serving
- **Docker**: Containerized deployment ready

## Development Workflow

### Local Development
1. `npm run dev` - Start HTTPS development server on port 51188
2. Hot reload enabled for rapid development
3. API requests proxied to backend services
4. Automatic TypeScript compilation and error checking

### Production Build
1. `npm run build` - Create optimized production build
2. Static assets generated in `dist/` directory
3. Build served by MSChat.WebBFF in production
4. TypeScript compilation and optimization

### Code Quality
- **ESLint**: Modern flat configuration with React and TypeScript rules
- **TypeScript**: Strict mode enabled for maximum type safety
- **Hot Reload**: Vite HMR for instant development feedback
- **Development Certificates**: HTTPS enabled with automatic certificate management