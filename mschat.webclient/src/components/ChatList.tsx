import React, { useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  List,
  Typography,
} from '@mui/material';
import { useChat } from '../context/ChatContext';
import ChatListItem from './ChatListItem';
import { useAuth } from '../auth/AuthContext';

const ChatList: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const { chats, loading, updateChats } = useChat();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  useEffect(() => {
    if (!isAuthenticated) return;
    
    updateChats();
  }, [isAuthenticated]);
  
  const handleChatSelect = (chatId: number) => {
    navigate(`/chats/${chatId}`);
  };

  return (
    <Box sx={{ py: 1 }}>
      <Typography
        variant="subtitle2"
        sx={{
          px: 2,
          py: 1,
          fontWeight: 600,
          color: 'text.secondary',
          textTransform: 'uppercase',
          fontSize: '0.75rem',
          letterSpacing: '0.5px',
        }}
      >
        Chats
      </Typography>
      
      <List sx={{ px: 1 }}>
        {chats.map((chat) => (
          <ChatListItem
            key={chat.id}
            chat={chat}
            isSelected={id === chat.id.toString()}
            onSelect={handleChatSelect}
          />
        ))}
      </List>
    </Box>
  );
};

export default ChatList;