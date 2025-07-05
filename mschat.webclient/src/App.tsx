import { BrowserRouter } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from 'react-oidc-context';
import { queryClient } from './lib/queryClient';
import Router from './components/Router';
import { oidcSettings } from './auth/oidcSettings';

function App() {
  return (
    <AuthProvider {...oidcSettings}>
      <BrowserRouter>
        <QueryClientProvider client={queryClient}>
          <Router />
        </QueryClientProvider>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;