import React, { useEffect, useState } from 'react';
import LoginPage from '../pages/LoginPage';
import RegisterPage from '../pages/RegisterPage';
import DefaultPage from '../pages/DefaultPage';
import ChatPage from '../pages/ChatPage';
import Layout from './Layout';
import { useAuth } from '../context/AuthContext';

const Router: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [currentRoute, setCurrentRoute] = useState(window.location.hash.slice(1) || '/');

  useEffect(() => {
    const handleHashChange = () => {
      setCurrentRoute(window.location.hash.slice(1) || '/');
    };

    window.addEventListener('hashchange', handleHashChange);
    return () => window.removeEventListener('hashchange', handleHashChange);
  }, []);

  // If not authenticated and trying to access protected routes, redirect to login
  if (!isAuthenticated && currentRoute !== '/login' && currentRoute !== '/register') {
    if (currentRoute !== '/login') {
      window.location.hash = '#/login';
      return null;
    }
  }

  // If authenticated and trying to access auth pages, redirect to home
  if (isAuthenticated && (currentRoute === '/login' || currentRoute === '/register')) {
    window.location.hash = '#/';
    return null;
  }

  const renderPage = () => {
    switch (currentRoute) {
      case '/login':
        return <LoginPage />;
      case '/register':
        return <RegisterPage />;
      case '/chat':
        return <ChatPage />;
      case '/':
      case '':
      default:
        return <DefaultPage />;
    }
  };

  // Auth pages don't use the layout
  if (currentRoute === '/login' || currentRoute === '/register') {
    return renderPage();
  }

  // Protected pages use the layout
  return (
    <Layout>
      {renderPage()}
    </Layout>
  );
};

export default Router;