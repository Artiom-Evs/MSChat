import { HashRouter } from 'react-router-dom';
import { ChatProvider } from './context/ChatContext';
import Router from './components/Router';

function App() {
  return (
    <HashRouter>
      <ChatProvider>
        <Router />
      </ChatProvider>
    </HashRouter>
  );
}

export default App;