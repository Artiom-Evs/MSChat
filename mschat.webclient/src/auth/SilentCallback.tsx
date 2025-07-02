import React, { useEffect } from 'react';
import { useAuthStore } from '../stores/authStore';
import { Typography } from '@mui/material';

const SilentCallback: React.FC = () => {
  const { userManager } = useAuthStore();

  useEffect(() => {
    const handleSilentCallback = async () => {
      try {
        await userManager.signinSilentCallback();
      } catch (error) {
        console.error('Silent callback error:', error);
      }
    };

    handleSilentCallback();
  }, [userManager]);

  return <Typography>Processing silent authentication...</Typography>;
};

export default SilentCallback;