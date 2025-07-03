import { BrowserRouter } from 'react-router-dom';
import { ChatProvider } from './context/ChatContext';
import Router from './components/Router';
import { AuthProvider } from './auth/AuthContext';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ChatProvider>
          <Router />
        </ChatProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;