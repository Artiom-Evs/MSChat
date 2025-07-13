import { WebStorageStateStore } from "oidc-client-ts";
import type { AuthProviderProps } from "react-oidc-context";
import { config } from "../config";

// lazy-load OIDC configuration
export const getOidcSettings = (): AuthProviderProps => ({
  authority: config.oidcServerUri,
  client_id: config.oidcClientId,
  redirect_uri: `${window.location.origin}/auth/callback`,
  post_logout_redirect_uri: `${window.location.origin}`,
  response_type: "code",
  scope: "openid profile email chatapi presenceapi",
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
});
