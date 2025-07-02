import React from 'react';
import { Box, Button, Typography, Paper } from '@mui/material';
import { useAuthStore } from '../stores/authStore';

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

export default LoginPrompt;