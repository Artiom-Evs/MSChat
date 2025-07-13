import { BrowserRouter } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from 'react-oidc-context';
import { queryClient } from './lib/queryClient';
import Router from './components/Router';
import { getOidcSettings } from './auth/oidcSettings';
import { ChatHubProvider, PresenceHubProvider } from './context';
import { config } from './config';

function App() {
  const chatHubUrl = `${config.chatApiUri}/_hubs/chat`;
  const presenceHubUrl = `${config.presenceApiUri}/_hubs/presence`;

  return (
    <AuthProvider {...getOidcSettings()}>
      <BrowserRouter>
        <QueryClientProvider client={queryClient}>
          <ChatHubProvider hubUrl={chatHubUrl}>
            <PresenceHubProvider hubUrl={presenceHubUrl}>
              <Router />
            </PresenceHubProvider>
          </ChatHubProvider>
        </QueryClientProvider>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
