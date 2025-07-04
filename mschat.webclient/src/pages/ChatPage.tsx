import React, { useState } from 'react';
import { Box, Typography, Paper, IconButton, CircularProgress, Chip } from '@mui/material';
import { Delete, Edit } from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { useChat, useDeleteChat } from '../hooks/useChats';
import DeleteChatDialog from '../components/DeleteChatDialog';
import { ChatType } from '../types';
import { useCurrentMember } from '../hooks/useMembers';

const ChatPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: selectedChat, isLoading } = useChat(chatId || 0);
  const { data: me } = useCurrentMember();
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

  // get second member name as a chat title when chat is personal
  const title = selectedChat.type == ChatType.Personal
    ? selectedChat.participants?.find(m => me && m.memberId != me?.id)?.memberName ?? selectedChat.name
    : selectedChat.name;

  const getChatTypeLabel = (type: ChatType) => {
    return type === ChatType.Public ? 'Public' : 'Personal';
  };

  const getChatTypeColor = (type: ChatType) => {
    return type === ChatType.Public ? 'primary' : 'secondary';
  };

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
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              {title}
            </Typography>
            <Chip
              label={getChatTypeLabel(selectedChat.type)}
              size="small"
              color={getChatTypeColor(selectedChat.type)}
            />
          </Box>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <IconButton
              onClick={() => navigate(`/chats/${selectedChat.id}/edit`)}
              sx={{
                color: 'text.secondary',
                '&:hover': {
                  color: 'primary.main',
                },
              }}
            >
              <Edit fontSize="small" />
            </IconButton>
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
            {title}
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