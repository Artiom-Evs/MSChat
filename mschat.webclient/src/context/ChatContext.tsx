import React, { createContext, useContext, useEffect, useRef, useState, type ReactNode } from 'react';
import type { Chat } from '../types';
import { useAuth } from '../auth/AuthContext';
import { ChatApiClient } from '../services';

interface ChatContextType {
  chats: Chat[];
  loading: boolean;
  selectedChatId: number | null;
  selectChat: (chatId: number) => void;
  updateChats: () => Promise<void>;
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
  const [loading, setLoading] = useState(false);
  const [selectedChatId, setSelectedChatId] = useState<number | null>(null);
  
  useEffect(() => {
    if (isAuthenticated && user) {
      chatApiRef.current.instance.defaults.headers[
        "Authorization"
      ] = `Bearer ${user.access_token}`;
    } else {
      delete chatApiRef.current.instance.defaults.headers["Authorization"];
    }
  }, [isAuthenticated, user]);

  const updateChats = async () => {
    if (loading) return;

    setLoading(true);

    chatApiRef.current.getChats().then(newChats => {
      setChats(newChats);
    }).catch(error => {
      console.error("Error while get chats.", error.message)
    }).finally(() => {
      setLoading(false);
    })
  };

  const selectChat = (chatId: number) => {
    setSelectedChatId(chatId);
  };

  const value = {
    chats,
    loading,
    selectedChatId,
    updateChats,
    selectChat,
  };

  return (
    <ChatContext.Provider value={value}>
      {children}
    </ChatContext.Provider>
  );
};