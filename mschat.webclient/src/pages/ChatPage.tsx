import React from 'react';
import { Box, Typography, CircularProgress } from '@mui/material';
import { useParams } from 'react-router-dom';
import { useChat } from '../hooks/useChats';
import { ChatType } from '../types';
import PublicChatPage from './PublicChatPage';
import PersonalChatPage from './PersonalChatPage';

const ChatPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: selectedChat, isLoading } = useChat(chatId || 0);

  if (isLoading) {
    return (
      <Box 
        sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center', 
          height: '100%',
          flexDirection: 'column',
          gap: 2
        }}
      >
        <CircularProgress />
        <Typography variant="h6" color="text.secondary">
          Loading chat...
        </Typography>
      </Box>
    );
  }

  if (!selectedChat) {
    return (
      <Box 
        sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center', 
          height: '100%',
          flexDirection: 'column',
          gap: 2
        }}
      >
        <Typography variant="h5" color="text.secondary">
          Welcome to MSChat
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Select a chat from the sidebar to start messaging
        </Typography>
      </Box>
    );
  }

  // Route to specific chat component based on chat type
  if (selectedChat.type === ChatType.Public) {
    return <PublicChatPage />;
  }

  if (selectedChat.type === ChatType.Personal) {
    return <PersonalChatPage />;
  }

  // Fallback for unknown chat types
  return (
    <Box 
      sx={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'center', 
        height: '100%',
        flexDirection: 'column',
        gap: 2
      }}
    >
      <Typography variant="h5" color="error">
        Unknown Chat Type
      </Typography>
      <Typography variant="body1" color="text.secondary">
        This chat type is not supported
      </Typography>
    </Box>
  );
};

export default ChatPage;