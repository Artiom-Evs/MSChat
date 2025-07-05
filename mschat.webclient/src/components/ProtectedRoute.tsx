import React from 'react';
import { Box, Typography } from '@mui/material';
import { useAuthToken } from '../hooks/useAuthToken';
import Layout from './Layout';
import LoginPrompt from './LoginPrompt';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const auth = useAuthToken();

  if (auth.isLoading) {
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

  if (!auth.isAuthenticated) {
    return <LoginPrompt />;
  }

  return <Layout>{children}</Layout>;
};

export default ProtectedRoute;