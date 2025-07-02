import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  List,
  ListItem,
  ListItemButton,
  Typography,
  Chip,
} from '@mui/material';
import { useChat } from '../context/ChatContext';

const ChatList: React.FC = () => {
  const { chats, selectedChatId, selectChat } = useChat();
  const navigate = useNavigate();

  const handleChatSelect = (chatId: string) => {
    selectChat(chatId);
    navigate('/chat');
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
          <ListItem key={chat.id} disablePadding>
            <ListItemButton
              onClick={() => handleChatSelect(chat.id)}
              selected={selectedChatId === chat.id}
              sx={{
                borderRadius: 2,
                mb: 0.5,
                '&.Mui-selected': {
                  backgroundColor: 'primary.main',
                  color: 'primary.contrastText',
                  '&:hover': {
                    backgroundColor: 'primary.dark',
                  },
                  '& .MuiTypography-root': {
                    color: 'inherit',
                  },
                },
                '&:hover': {
                  backgroundColor: 'action.hover',
                },
              }}
            >
              <Box sx={{ flex: 1, minWidth: 0 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Typography
                    variant="subtitle2"
                    sx={{
                      fontWeight: 600,
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap',
                      flexGrow: 1,
                    }}
                  >
                    {chat.title}
                  </Typography>
                  {chat.unreadCount && chat.unreadCount > 0 && (
                    <Chip
                      label={chat.unreadCount}
                      size="small"
                      sx={{
                        height: 20,
                        fontSize: '0.75rem',
                        minWidth: 20,
                        backgroundColor: selectedChatId === chat.id ? 'rgba(255,255,255,0.2)' : 'primary.main',
                        color: selectedChatId === chat.id ? 'inherit' : 'primary.contrastText',
                      }}
                    />
                  )}
                </Box>
                <Typography
                  variant="body2"
                  sx={{
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                    color: selectedChatId === chat.id ? 'rgba(255,255,255,0.8)' : 'text.secondary',
                    mt: 0.5,
                  }}
                >
                  {chat.lastMessage}
                </Typography>
                <Typography
                  variant="caption"
                  sx={{
                    color: selectedChatId === chat.id ? 'rgba(255,255,255,0.6)' : 'text.secondary',
                    mt: 0.5,
                    display: 'block',
                  }}
                >
                  {chat.lastMessageTime}
                </Typography>
              </Box>
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );
};

export default ChatList;