import React, { createContext, useContext, useState, ReactNode } from 'react';

interface Chat {
  id: string;
  title: string;
  lastMessage: string;
  lastMessageTime: string;
  unreadCount?: number;
}

interface ChatContextType {
  chats: Chat[];
  selectedChatId: string | null;
  selectChat: (chatId: string) => void;
  addChat: (chat: Chat) => void;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

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
  const [selectedChatId, setSelectedChatId] = useState<string | null>(null);
  
  // Mock chat data
  const [chats] = useState<Chat[]>([
    {
      id: '1',
      title: 'General Discussion',
      lastMessage: 'Hey everyone! How is everyone doing today?',
      lastMessageTime: '2 min ago',
      unreadCount: 3
    },
    {
      id: '2',
      title: 'Project Updates',
      lastMessage: 'The new feature is ready for testing',
      lastMessageTime: '1 hour ago',
      unreadCount: 1
    },
    {
      id: '3',
      title: 'Random Chat',
      lastMessage: 'Anyone up for lunch?',
      lastMessageTime: '3 hours ago'
    },
    {
      id: '4',
      title: 'Tech Talk',
      lastMessage: 'React 19 is amazing! The new features are...',
      lastMessageTime: '1 day ago'
    }
  ]);

  const selectChat = (chatId: string) => {
    setSelectedChatId(chatId);
  };

  const addChat = (chat: Chat) => {
    // Implementation for adding new chats
    console.log('Adding chat:', chat);
  };

  const value = {
    chats,
    selectedChatId,
    selectChat,
    addChat
  };

  return (
    <ChatContext.Provider value={value}>
      {children}
    </ChatContext.Provider>
  );
};