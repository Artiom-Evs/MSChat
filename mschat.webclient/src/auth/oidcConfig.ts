import type { UserManagerSettings } from "oidc-client-ts";

const env = import.meta.env;

if (!env.VITE_OIDC_SERVER_URI || !env.VITE_OIDC_CLIENT_ID) {
  throw new Error("OIDC configuration is missing in environment variables.");
}

export const oidcConfig: UserManagerSettings = {
  authority: env.VITE_OIDC_SERVER_URI,
  client_id: env.VITE_OIDC_CLIENT_ID,
  redirect_uri: `${window.location.origin}/auth/callback`,
  post_logout_redirect_uri: `${window.location.origin}/`,
  response_type: "code",
  scope: "openid profile email chatapi.user",
  automaticSilentRenew: true,
  silent_redirect_uri: `${window.location.origin}/auth/silent-callback`,
  includeIdTokenInSilentRenew: true,
  loadUserInfo: true,
  monitorSession: false,
  revokeTokensOnSignout: true,
};
