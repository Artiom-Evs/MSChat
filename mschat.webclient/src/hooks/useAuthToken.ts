import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import { setAuthToken } from '../lib/chatApi';

export const useAuthToken = () => {
  const auth = useAuth();

  useEffect(() => {
    if (auth.isAuthenticated && auth.user?.access_token) {
      setAuthToken(auth.user.access_token);
    } else {
      setAuthToken(null);
    }
  }, [auth.isAuthenticated, auth.user?.access_token]);

  return auth;
};