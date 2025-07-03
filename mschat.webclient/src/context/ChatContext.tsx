import React, { createContext, useContext, useEffect, useRef, useState, type ReactNode } from 'react';
import type { Chat } from '../types';
import { useAuth } from '../auth/AuthContext';
import { ChatApiClient } from '../services';

interface ChatContextType {
  chats: Chat[];
  error: string | null;
  loading: boolean;
  updateChats: () => void;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

// eslint-disable-next-line react-refresh/only-export-components
export const useChat = () => {
  const context = useContext(ChatContext);
  if (context === undefined) {
    throw new Error('useChat must be used within a ChatProvider');
  }
  return context;
};

interface ChatProviderProps {
  children: ReactNode;
}

export const ChatProvider: React.FC<ChatProviderProps> = ({ children }) => {
  const { user, isAuthenticated } = useAuth();
  const chatApiRef = useRef(new ChatApiClient());
  
  const [chats, setChats] = useState<Chat[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  
  useEffect(() => {
    if (isAuthenticated && user) {
      chatApiRef.current.instance.defaults.headers[
        "Authorization"
      ] = `Bearer ${user.access_token}`;
    } else {
      delete chatApiRef.current.instance.defaults.headers["Authorization"];
    }
  }, [isAuthenticated, user]);

  const updateChats = () => {
    if (loading) return;

    setLoading(true);
    setError(null);

    chatApiRef.current.getChats().then(newChats => {
      setChats(newChats);
    }).catch(error => {
      console.error("Error while get chats.", error.message);
      setError(error.message);
    }).finally(() => {
      setLoading(false);
    })
  };

  const value = {
    chats,
    error,
    loading,
    updateChats,
  };

  return (
    <ChatContext.Provider value={value}>
      {children}
    </ChatContext.Provider>
  );
};