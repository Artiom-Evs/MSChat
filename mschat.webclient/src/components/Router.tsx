import React, { useEffect, useState } from 'react';
import DefaultPage from '../pages/DefaultPage';
import ChatPage from '../pages/ChatPage';
import Layout from './Layout';
import { useAuthStore } from '../stores/authStore';
import AuthCallback from '../auth/AuthCallback';
import SilentCallback from '../auth/SilentCallback';
import { Box, Button, Typography, Paper } from '@mui/material';

const LoginPrompt: React.FC = () => {
  const login = useAuthStore((state) => state.login);

  const handleLogin = async () => {
    try {
      await login();
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'background.default',
        p: 2,
      }}
    >
      <Paper
        elevation={3}
        sx={{
          p: 4,
          width: '100%',
          maxWidth: 400,
          borderRadius: 3,
          textAlign: 'center'
        }}
      >
        <Typography variant="h4" component="h1" gutterBottom sx={{ fontWeight: 700 }}>
          Welcome to MSChat
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Please sign in to continue
        </Typography>
        <Button
          onClick={handleLogin}
          fullWidth
          variant="contained"
          sx={{
            py: 1.5,
            fontSize: '1rem',
            textTransform: 'none',
            borderRadius: 2,
          }}
        >
          Sign In
        </Button>
      </Paper>
    </Box>
  );
};

const Router: React.FC = () => {
  const { isAuthenticated, isLoading, initAuth } = useAuthStore();
  const [currentRoute, setCurrentRoute] = useState(window.location.pathname);

  useEffect(() => {
    initAuth();
  }, [initAuth]);

  useEffect(() => {
    const handleLocationChange = () => {
      setCurrentRoute(window.location.pathname);
    };

    window.addEventListener('popstate', handleLocationChange);
    return () => window.removeEventListener('popstate', handleLocationChange);
  }, []);

  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh'
        }}
      >
        <Typography variant="h6">Loading...</Typography>
      </Box>
    );
  }

  const renderPage = () => {
    switch (currentRoute) {
      case '/auth/callback':
        return <AuthCallback />;
      case '/auth/silent-callback':
        return <SilentCallback />;
      case '/chat':
        return isAuthenticated ? <ChatPage /> : <LoginPrompt />;
      case '/':
      case '':
      default:
        return isAuthenticated ? <DefaultPage /> : <LoginPrompt />;
    }
  };

  // Callback pages don't use the layout
  if (currentRoute === '/auth/callback' || currentRoute === '/auth/silent-callback') {
    return renderPage();
  }

  // Unauthenticated pages don't use the layout
  if (!isAuthenticated) {
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