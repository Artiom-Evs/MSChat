import React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  List,
  Typography,
  IconButton,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { useChats } from '../hooks/useChats';
import ChatListItem from './ChatListItem';

const ChatList: React.FC = () => {
  const { data: chats = [] } = useChats();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  
  const handleChatSelect = (chatId: number) => {
    navigate(`/chats/${chatId}`);
  };

  return (
    <Box sx={{ py: 1 }}>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          px: 2,
          py: 1,
        }}
      >
        <Typography
          variant="subtitle2"
          sx={{
            fontWeight: 600,
            color: 'text.secondary',
            textTransform: 'uppercase',
            fontSize: '0.75rem',
            letterSpacing: '0.5px',
          }}
        >
          Chats
        </Typography>
        <IconButton
          size="small"
          onClick={() => navigate('/chats/create')}
          sx={{
            color: 'text.secondary',
            '&:hover': {
              color: 'primary.main',
            },
          }}
        >
          <Add fontSize="small" />
        </IconButton>
      </Box>
      
      <List sx={{ px: 1 }}>
        {chats.filter(c => c.isInChat).map((chat) => (
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