import React, { useEffect, useState, useCallback } from 'react';
import { Box, Typography, Paper, IconButton, CircularProgress, Chip, Button } from '@mui/material';
import { Delete, Edit, ExitToApp, Login, People } from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { useChat, useDeleteChat } from '../hooks/useChats';
import { useJoinChat, useLeaveChat } from '../hooks/useParticipants';
import DeleteChatDialog from '../components/DeleteChatDialog';
import MessageList from '../components/MessageList';
import MessageForm from '../components/MessageForm';
import { ChatType, type MessageDto } from '../types';
import { useCurrentMember } from '../hooks/useMembers';
import { useChatHub } from '../context';
import { useQueryClient } from '@tanstack/react-query';

const PublicChatPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: selectedChat, isLoading } = useChat(chatId || 0);
  const { data: me } = useCurrentMember();
  const deleteChatMutation = useDeleteChat();
  const joinChatMutation = useJoinChat();
  const leaveChatMutation = useLeaveChat();
  const chatHub = useChatHub();
  const queryClient = useQueryClient();
  
  const handleNewMessage = useCallback((newMessage: MessageDto) => {
    console.log('SignalR: New message received:', newMessage);
    if (newMessage.chatId === chatId) {
      console.log('SignalR: Updating cache for chat:', chatId);
      queryClient.setQueryData(
        ["messages", chatId, "infinite"],
        (oldData: unknown) => {
          console.log('SignalR: Current cache data:', oldData);
          if (!oldData || typeof oldData !== 'object' || !('pages' in oldData)) {
            return {
              pages: [[newMessage]],
              pageParams: [1],
            };
          }
          
          const data = oldData as { pages: Array<unknown[]>; pageParams: unknown[] };
          
          const newPages = [...data.pages];
          if (newPages.length === 0) {
            newPages.push([newMessage]);
          } else {
            newPages[0] = [newMessage, ...newPages[0]];
          }
          
          return {
            ...data,
            pages: newPages,
          };
        }
      );
      
      queryClient.invalidateQueries({
        queryKey: ["messages", chatId],
      });
    }
  }, [chatId, queryClient]);

  const handleMessageUpdate = useCallback((updatedMessage: MessageDto) => {
    console.log('SignalR: Message updated:', updatedMessage);
    if (updatedMessage.chatId === chatId) {
      console.log('SignalR: Updating message in cache for chat:', chatId);
      queryClient.setQueryData(
        ["messages", chatId, "infinite"],
        (oldData: unknown) => {
          if (!oldData || typeof oldData !== 'object' || !('pages' in oldData)) {
            return oldData;
          }
          
          const data = oldData as { pages: Array<unknown[]>; pageParams: unknown[] };
          
          const newPages = data.pages.map(page => 
            page.map(msg => 
              (msg as MessageDto).id === updatedMessage.id ? updatedMessage : msg
            )
          );
          
          return {
            ...data,
            pages: newPages,
          };
        }
      );
      
      queryClient.invalidateQueries({
        queryKey: ["messages", chatId],
      });
    }
  }, [chatId, queryClient]);

  const handleMessageDelete = useCallback((messageId: number) => {
    console.log('SignalR: Message deleted:', messageId, 'from chat:', chatId);
    if (chatId) {
      console.log('SignalR: Removing message from cache for chat:', chatId);
      queryClient.setQueryData(
        ["messages", chatId, "infinite"],
        (oldData: unknown) => {
          if (!oldData || typeof oldData !== 'object' || !('pages' in oldData)) {
            return oldData;
          }
          
          const data = oldData as { pages: Array<unknown[]>; pageParams: unknown[] };
          
          const newPages = data.pages.map(page => 
            page.filter(msg => (msg as MessageDto).id !== messageId)
          );
          
          return {
            ...data,
            pages: newPages,
          };
        }
      );
      
      queryClient.invalidateQueries({
        queryKey: ["messages", chatId],
      });
      
      queryClient.removeQueries({
        queryKey: ["message", chatId, messageId],
      });
    }
  }, [chatId, queryClient]);

  useEffect(() => {
    if (!chatHub.connection || !id) return;

    const conn = chatHub.connection;
    console.log('SignalR: Setting up event listeners for public chat:', id);

    conn.on("NewMessageSent", handleNewMessage);
    conn.on("MessageUpdated", handleMessageUpdate);
    conn.on("MessageDeleted", handleMessageDelete);
    
    conn.invoke("SubscribeToChat", `${id}`).then(() => {
      console.log('SignalR: Successfully subscribed to public chat:', id);
    }).catch(err => {
      console.error('SignalR: Failed to subscribe to public chat:', id, err);
    });
    
    return () => {
      console.log('SignalR: Cleaning up event listeners for public chat:', id);
      conn.off("NewMessageSent", handleNewMessage);
      conn.off("MessageUpdated", handleMessageUpdate);
      conn.off("MessageDeleted", handleMessageDelete);
      conn.invoke("UnsubscribeFromChat", id);
    }
  }, [chatHub.connection, id, handleNewMessage, handleMessageUpdate, handleMessageDelete]);

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
          Loading public chat...
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
          Public Chat Not Found
        </Typography>
        <Typography variant="body1" color="text.secondary">
          The requested public chat could not be found
        </Typography>
      </Box>
    );
  }

  // Validate that this is actually a public chat
  if (selectedChat.type !== ChatType.Public) {
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
          Invalid Chat Type
        </Typography>
        <Typography variant="body1" color="text.secondary">
          This is not a public chat
        </Typography>
      </Box>
    );
  }

  const isUserInChat = selectedChat.participants?.some(p => p.memberId === me?.id) || false;
  const canJoinChat = !isUserInChat;
  const canLeaveChat = isUserInChat;

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
              {selectedChat.name}
            </Typography>
            <Chip
              label="Public"
              size="small"
              color="primary"
            />
            <Typography variant="body2" color="text.secondary">
              {selectedChat.participants?.length || 0} members
            </Typography>
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
            {isUserInChat && (
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
            )}
            {isUserInChat && (
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
            )}
          </Box>
        </Box>
      </Paper>
      
      <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column' }}>
        <MessageList chatId={chatId!} />
      </Box>
      
      <MessageForm 
        chatId={chatId!} 
        disabled={!isUserInChat}
      />
      
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

export default PublicChatPage;