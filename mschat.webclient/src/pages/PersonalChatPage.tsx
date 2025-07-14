import React, { useEffect, useState, useCallback } from 'react';
import { Box, Typography, Paper, IconButton, CircularProgress, Chip } from '@mui/material';
import { Delete, Edit } from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { useChat, useDeleteChat } from '../hooks/useChats';
import DeleteChatDialog from '../components/DeleteChatDialog';
import MessageList from '../components/MessageList';
import MessageForm from '../components/MessageForm';
import { ChatType, type MessageDto } from '../types';
import { useCurrentMember } from '../hooks/useMembers';
import { useChatHub } from '../context';
import { useQueryClient } from '@tanstack/react-query';

const PersonalChatPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: selectedChat, isLoading } = useChat(chatId || 0);
  const { data: me } = useCurrentMember();
  const deleteChatMutation = useDeleteChat();
  const chatHub = useChatHub();
  const queryClient = useQueryClient();
  
  const handleNewMessage = useCallback((newMessage: MessageDto) => {
    console.log('SignalR: New message received in personal chat:', newMessage);
    if (newMessage.chatId === chatId) {
      console.log('SignalR: Updating cache for personal chat:', chatId);
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
    console.log('SignalR: Message updated in personal chat:', updatedMessage);
    if (updatedMessage.chatId === chatId) {
      console.log('SignalR: Updating message in cache for personal chat:', chatId);
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
    console.log('SignalR: Message deleted in personal chat:', messageId, 'from chat:', chatId);
    if (chatId) {
      console.log('SignalR: Removing message from cache for personal chat:', chatId);
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
    console.log('SignalR: Setting up event listeners for personal chat:', id);

    conn.on("NewMessageSent", handleNewMessage);
    conn.on("MessageUpdated", handleMessageUpdate);
    conn.on("MessageDeleted", handleMessageDelete);
    
    conn.invoke("SubscribeToChat", `${id}`).then(() => {
      console.log('SignalR: Successfully subscribed to personal chat:', id);
    }).catch(err => {
      console.error('SignalR: Failed to subscribe to personal chat:', id, err);
    });
    
    return () => {
      console.log('SignalR: Cleaning up event listeners for personal chat:', id);
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
          Loading conversation...
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
          Conversation Not Found
        </Typography>
        <Typography variant="body1" color="text.secondary">
          The requested conversation could not be found
        </Typography>
      </Box>
    );
  }

  // Validate that this is actually a personal chat
  if (selectedChat.type !== ChatType.Personal) {
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
          This is not a personal conversation
        </Typography>
      </Box>
    );
  }

  // Get the other participant's name as chat title
  const otherParticipant = selectedChat.participants?.find(p => me && p.memberId !== me.id);
  const chatTitle = otherParticipant?.memberName || selectedChat.name;

  // In personal chats, user is always in the chat (by definition)
  const isUserInChat = selectedChat.participants?.some(p => p.memberId === me?.id) || false;

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
              {chatTitle}
            </Typography>
            <Chip
              label="Personal"
              size="small"
              color="secondary"
            />
            {otherParticipant && (
              <Typography variant="body2" color="text.secondary">
                Direct message with {otherParticipant.memberName}
              </Typography>
            )}
          </Box>
          <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
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

export default PersonalChatPage;