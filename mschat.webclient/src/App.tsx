import { ChatProvider } from './context/ChatContext';
import Router from './components/Router';

function App() {
  return (
    <ChatProvider>
      <Router />
    </ChatProvider>
  );
}

export default App;