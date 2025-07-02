import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import { UserManager, type User as OidcUser } from 'oidc-client-ts';
import { oidcConfig } from '../auth/oidcConfig';

interface User {
  id: string;
  name: string;
  email: string;
  avatar?: string;
  roles?: string[];
}

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  userManager: UserManager;
  initAuth: () => Promise<void>;
  login: () => Promise<void>;
  logout: () => Promise<void>;
  setUser: (oidcUser: OidcUser | null) => void;
  setLoading: (loading: boolean) => void;
}

const userManager = new UserManager(oidcConfig);

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      isAuthenticated: false,
      isLoading: true,
      userManager,

      initAuth: async () => {
        try {
          set({ isLoading: true });
          const oidcUser = await userManager.getUser();
          get().setUser(oidcUser);
        } catch (error) {
          console.error('Error initializing auth:', error);
          get().setUser(null);
        } finally {
          set({ isLoading: false });
        }
      },

      login: async () => {
        try {
          await userManager.signinRedirect();
        } catch (error) {
          console.error('Login error:', error);
          throw error;
        }
      },

      logout: async () => {
        try {
          await userManager.signoutRedirect();
        } catch (error) {
          console.error('Logout error:', error);
          set({
            user: null,
            isAuthenticated: false
          });
        }
      },

      setUser: (oidcUser: OidcUser | null) => {
        if (oidcUser && !oidcUser.expired) {
          const user: User = {
            id: oidcUser.profile.sub || '',
            name: oidcUser.profile.name || oidcUser.profile.preferred_username || '',
            email: oidcUser.profile.email || '',
            avatar: oidcUser.profile.picture,
            roles: oidcUser.profile.role ? 
              (Array.isArray(oidcUser.profile.role) ? oidcUser.profile.role : [oidcUser.profile.role]) : 
              undefined
          };
          
          set({
            user,
            isAuthenticated: true
          });
        } else {
          set({
            user: null,
            isAuthenticated: false
          });
        }
      },

      setLoading: (loading: boolean) => {
        set({ isLoading: loading });
      }
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => localStorage),
      partialize: (state) => ({
        user: state.user,
        isAuthenticated: state.isAuthenticated
      })
    }
  )
);