import React from 'react';
import { Box, Typography, Paper } from '@mui/material';
import { useChat } from '../context/ChatContext';

const ChatPage: React.FC = () => {
  const { chats, selectedChatId } = useChat();
  
  const selectedChat = chats.find(chat => chat.id === selectedChatId);

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

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Paper 
        elevation={1} 
        sx={{ 
          p: 2, 
          borderRadius: 0,
          borderBottom: '1px solid',
          borderColor: 'divider'
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          {selectedChat.name}
        </Typography>
      </Paper>
      
      <Box sx={{ flexGrow: 1, p: 3 }}>
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
          <Typography variant="h6" color="text.secondary">
            Chat: {selectedChat.name}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Last message: {selectedChat.lastMessage}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {selectedChat.lastMessageTime}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
            Chat interface will be implemented here
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};

export default ChatPage;