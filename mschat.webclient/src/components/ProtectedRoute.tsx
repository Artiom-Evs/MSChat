import React from 'react';
import { Box, Typography } from '@mui/material';
import { useAuthStore } from '../stores/authStore';
import Layout from './Layout';
import LoginPrompt from './LoginPrompt';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuthStore();

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

  if (!isAuthenticated) {
    return <LoginPrompt />;
  }

  return <Layout>{children}</Layout>;
};

export default ProtectedRoute;