import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { Box, Typography, Button } from '@mui/material';

const AuthCallback: React.FC = () => {
  const { userManager, setUser } = useAuthStore();
  const navigate = useNavigate();
  const [isProcessing, setIsProcessing] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const handleCallback = async () => {
      try {
        const user = await userManager.signinRedirectCallback();
        
        if (user) {
          setUser(user);
          navigate('/', { replace: true });
        } else {
          setError('Authentication failed: No user returned');
        }
      } catch (err) {
        console.error('Authentication callback error:', err);
        setError(err instanceof Error ? err.message : 'Authentication failed');
      } finally {
        setIsProcessing(false);
      }
    };

    handleCallback();
  }, [userManager, setUser, navigate]);

  if (isProcessing) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh',
          flexDirection: 'column'
        }}
      >
        <Typography variant="h5" gutterBottom>
          Processing authentication...
        </Typography>
        <Typography variant="body1">
          Please wait while we complete your login.
        </Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh',
          flexDirection: 'column'
        }}
      >
        <Typography variant="h5" gutterBottom>
          Authentication Error
        </Typography>
        <Typography variant="body1" sx={{ mb: 2 }}>
          {error}
        </Typography>
        <Button variant="contained" onClick={() => navigate('/')}>
          Return to Home
        </Button>
      </Box>
    );
  }

  return null;
};

export default AuthCallback;