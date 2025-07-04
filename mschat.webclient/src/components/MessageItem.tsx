import React, { useState } from 'react';
import {
  Box,
  Typography,
  Avatar,
  IconButton,
  Menu,
  MenuItem,
  Chip,
  TextField,
  Button,
} from '@mui/material';
import { MoreVert, Edit, Delete, Save, Cancel } from '@mui/icons-material';
import type { MessageDto } from '../types';
import { useCurrentMember } from '../hooks/useMembers';
import { useUpdateMessage, useDeleteMessage } from '../hooks/useMessages';

interface MessageItemProps {
  message: MessageDto;
  chatId: number;
}

const MessageItem: React.FC<MessageItemProps> = ({ message, chatId }) => {
  const { data: me } = useCurrentMember();
  const updateMessage = useUpdateMessage();
  const deleteMessage = useDeleteMessage();
  
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [editText, setEditText] = useState(message.text);

  const isMyMessage = me?.id === message.senderId;
  const isDeleted = message.isDeleted;
  const isEdited = message.isEdited;

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleEdit = () => {
    setIsEditing(true);
    setEditText(message.text);
    handleMenuClose();
  };

  const handleSaveEdit = async () => {
    if (editText.trim() && editText !== message.text) {
      try {
        await updateMessage.mutateAsync({
          chatId,
          messageId: message.id,
          message: { text: editText.trim() },
        });
        setIsEditing(false);
      } catch (error) {
        console.error('Failed to update message:', error);
      }
    } else {
      setIsEditing(false);
    }
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setEditText(message.text);
  };

  const handleDelete = async () => {
    try {
      await deleteMessage.mutateAsync({
        chatId,
        messageId: message.id,
      });
      handleMenuClose();
    } catch (error) {
      console.error('Failed to delete message:', error);
    }
  };

  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInHours = (now.getTime() - date.getTime()) / (1000 * 60 * 60);

    if (diffInHours < 24) {
      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    } else if (diffInHours < 24 * 7) {
      return date.toLocaleDateString([], { weekday: 'short', hour: '2-digit', minute: '2-digit' });
    } else {
      return date.toLocaleDateString([], { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' });
    }
  };

  if (isDeleted) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: isMyMessage ? 'flex-end' : 'flex-start',
          mb: 0.5,
        }}
      >
        <Box
          sx={{
            maxWidth: '70%',
            p: 0.75,
            borderRadius: 1,
            backgroundColor: 'action.disabledBackground',
            opacity: 0.7,
          }}
        >
          <Typography variant="caption" color="text.secondary" fontStyle="italic">
            This message was deleted
          </Typography>
        </Box>
      </Box>
    );
  }

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: isMyMessage ? 'flex-end' : 'flex-start',
        mb: 0.5,
        alignItems: 'flex-start',
        gap: 0.5,
      }}
    >
      {!isMyMessage && (
        <Avatar
          src={message.senderPhotoUrl || undefined}
          sx={{ width: 24, height: 24, fontSize: '0.75rem' }}
        >
          {message.senderName.charAt(0).toUpperCase()}
        </Avatar>
      )}
      
      <Box
        sx={{
          maxWidth: '75%',
          display: 'flex',
          flexDirection: 'column',
          alignItems: isMyMessage ? 'flex-end' : 'flex-start',
        }}
      >
        {!isMyMessage && (
          <Typography variant="caption" color="text.secondary" sx={{ mb: 0.25, fontSize: '0.7rem' }}>
            {message.senderName}
          </Typography>
        )}
        
        <Box
          sx={{
            position: 'relative',
            p: 0.75,
            borderRadius: 1.5,
            backgroundColor: isMyMessage ? 'primary.main' : 'background.paper',
            color: isMyMessage ? 'primary.contrastText' : 'text.primary',
            border: isMyMessage ? 'none' : '1px solid',
            borderColor: 'divider',
            '&:hover .message-actions': {
              opacity: 1,
            },
          }}
        >
          {isEditing ? (
            <Box sx={{ minWidth: 200 }}>
              <TextField
                fullWidth
                multiline
                value={editText}
                onChange={(e) => setEditText(e.target.value)}
                variant="outlined"
                size="small"
                sx={{
                  '& .MuiOutlinedInput-root': {
                    backgroundColor: 'background.paper',
                  },
                }}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    handleSaveEdit();
                  } else if (e.key === 'Escape') {
                    handleCancelEdit();
                  }
                }}
              />
              <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 1, mt: 1 }}>
                <Button
                  size="small"
                  onClick={handleCancelEdit}
                  startIcon={<Cancel />}
                >
                  Cancel
                </Button>
                <Button
                  size="small"
                  variant="contained"
                  onClick={handleSaveEdit}
                  startIcon={<Save />}
                  disabled={updateMessage.isPending}
                >
                  Save
                </Button>
              </Box>
            </Box>
          ) : (
            <>
              <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', fontSize: '0.875rem', lineHeight: 1.3 }}>
                {message.text}
              </Typography>
              
              {isMyMessage && (
                <IconButton
                  className="message-actions"
                  size="small"
                  onClick={handleMenuOpen}
                  sx={{
                    position: 'absolute',
                    top: -6,
                    right: -6,
                    opacity: 0,
                    transition: 'opacity 0.2s',
                    backgroundColor: 'background.paper',
                    border: '1px solid',
                    borderColor: 'divider',
                    width: 20,
                    height: 20,
                    '&:hover': {
                      backgroundColor: 'action.hover',
                    },
                  }}
                >
                  <MoreVert sx={{ fontSize: 12 }} />
                </IconButton>
              )}
            </>
          )}
        </Box>
        
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mt: 0.25 }}>
          <Typography variant="caption" color="text.secondary" sx={{ fontSize: '0.65rem' }}>
            {formatTime(message.createdAt)}
          </Typography>
          {isEdited && (
            <Chip
              label="edited"
              size="small"
              variant="outlined"
              sx={{ height: 12, fontSize: '0.5rem', '& .MuiChip-label': { px: 0.5 } }}
            />
          )}
        </Box>
      </Box>

      {/* Message Actions Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
      >
        <MenuItem onClick={handleEdit}>
          <Edit fontSize="small" sx={{ mr: 1 }} />
          Edit
        </MenuItem>
        <MenuItem onClick={handleDelete} sx={{ color: 'error.main' }}>
          <Delete fontSize="small" sx={{ mr: 1 }} />
          Delete
        </MenuItem>
      </Menu>
    </Box>
  );
};

export default MessageItem;