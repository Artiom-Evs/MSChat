import React, { useState } from 'react';
import { Box, Typography, Paper, IconButton, CircularProgress, Chip, Button } from '@mui/material';
import { Delete, Edit, ExitToApp, Login, People } from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { useChat, useDeleteChat } from '../hooks/useChats';
import { useJoinChat, useLeaveChat } from '../hooks/useParticipants';
import DeleteChatDialog from '../components/DeleteChatDialog';
import MessageList from '../components/MessageList';
import MessageForm from '../components/MessageForm';
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
  const joinChatMutation = useJoinChat();
  const leaveChatMutation = useLeaveChat();

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

  // Check if current user is in the chat
  const isUserInChat = selectedChat.participants?.some(p => p.memberId === me?.id) || false;
  const canJoinChat = selectedChat.type === ChatType.Public && !isUserInChat;
  const canLeaveChat = isUserInChat && selectedChat.type === ChatType.Public;

  const handleJoinChat = async () => {
    if (chatId) {
      await joinChatMutation.mutateAsync(chatId);
    }
  };

  const handleLeaveChat = async () => {
    if (chatId) {
      await leaveChatMutation.mutateAsync(chatId);
    }
  };

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
          <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
            {canJoinChat && (
              <Button
                variant="contained"
                size="small"
                startIcon={<Login />}
                onClick={handleJoinChat}
                disabled={joinChatMutation.isPending}
                sx={{ mr: 1 }}
              >
                {joinChatMutation.isPending ? 'Joining...' : 'Join Chat'}
              </Button>
            )}
            {canLeaveChat && (
              <Button
                variant="outlined"
                size="small"
                startIcon={<ExitToApp />}
                onClick={handleLeaveChat}
                disabled={leaveChatMutation.isPending}
                sx={{ mr: 1 }}
              >
                {leaveChatMutation.isPending ? 'Leaving...' : 'Leave Chat'}
              </Button>
            )}
            <IconButton
              onClick={() => navigate(`/chats/${selectedChat.id}/participants`)}
              sx={{
                color: 'text.secondary',
                '&:hover': {
                  color: 'primary.main',
                },
              }}
            >
              <People fontSize="small" />
            </IconButton>
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
      
      {/* Messages Area */}
      <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column' }}>
        <MessageList chatId={chatId!} />
      </Box>
      
      {/* Message Input */}
      <MessageForm 
        chatId={chatId!} 
        disabled={!isUserInChat}
      />
      
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