import React, { useState } from 'react';
import { Box, Typography, Paper, IconButton, CircularProgress } from '@mui/material';
import { Delete } from '@mui/icons-material';
import { useParams } from 'react-router-dom';
import { useChat, useDeleteChat } from '../hooks/useChats';
import DeleteChatDialog from '../components/DeleteChatDialog';

const ChatPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: selectedChat, isLoading } = useChat(chatId || 0);
  const deleteChatMutation = useDeleteChat();

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
        <Typography variant="h6" color="text.secondary">
          Loading...
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
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            {selectedChat.name}
          </Typography>
          <IconButton
            onClick={() => setDeleteDialogOpen(true)}
            disabled={deleteChatMutation.isPending}
            sx={{
              color: 'text.secondary',
              '&:hover': {
                color: 'error.main',
              },
            }}
          >
            {deleteChatMutation.isPending ? (
              <CircularProgress size={20} />
            ) : (
              <Delete fontSize="small" />
            )}
          </IconButton>
        </Box>
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
            Last message: {""}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {""}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
            Chat interface will be implemented here
          </Typography>
        </Box>
      </Box>
      
      {/* Delete Confirmation Dialog */}
      {selectedChat && (
        <DeleteChatDialog
          open={deleteDialogOpen}
          onClose={() => setDeleteDialogOpen(false)}
          chat={selectedChat}
        />
      )}
    </Box>
  );
};

export default ChatPage;