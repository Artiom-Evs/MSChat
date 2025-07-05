import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { chatApi } from "../lib/chatApi";

export const useAuthToken = () => {
  const auth = useAuth();

  useEffect(() => {
    if (auth.isAuthenticated && auth.user?.access_token) {
      chatApi.setAuthToken(auth.user.access_token);
    } else {
      chatApi.setAuthToken(null);
    }
  }, [auth.isAuthenticated, auth.user?.access_token]);

  return auth;
};
