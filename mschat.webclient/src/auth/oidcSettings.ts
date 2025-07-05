import { WebStorageStateStore } from "oidc-client-ts";
import type { AuthProviderProps } from "react-oidc-context";

const env = import.meta.env;

if (!env.VITE_OIDC_SERVER_URI || !env.VITE_OIDC_CLIENT_ID) {
  throw new Error("OIDC configuration is missing in environment variables.");
}

export const oidcSettings: AuthProviderProps = {
  authority: env.VITE_OIDC_SERVER_URI,
  client_id: env.VITE_OIDC_CLIENT_ID,
  redirect_uri: `${window.location.origin}/auth/callback`,
  post_logout_redirect_uri: `${window.location.origin}`,
  response_type: "code",
  scope: "openid profile email chatapi",
  automaticSilentRenew: true,
  silent_redirect_uri: `${window.location.origin}/auth/silent-callback`,
  includeIdTokenInSilentRenew: true,
  loadUserInfo: true,
  monitorSession: false,
  revokeTokensOnSignout: true,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  onSigninCallback: () => {
    // Clear the callback URL from the browser history
    window.history.replaceState({}, document.title, window.location.origin);
  },
};
