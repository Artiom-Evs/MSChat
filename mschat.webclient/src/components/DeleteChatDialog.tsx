import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  CircularProgress,
  Alert,
} from '@mui/material';
import { useDeleteChat } from '../hooks/useChats';
import type { ChatDto } from '../types';

interface DeleteChatDialogProps {
  open: boolean;
  onClose: () => void;
  chat: ChatDto;
}

const DeleteChatDialog: React.FC<DeleteChatDialogProps> = ({ open, onClose, chat }) => {
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const navigate = useNavigate();
  const deleteChatMutation = useDeleteChat();

  const handleDeleteChat = async () => {
    setDeleteError(null);
    
    try {
      await deleteChatMutation.mutateAsync(chat.id);
      onClose();
      navigate('/chats');
    } catch (err) {
      setDeleteError('Failed to delete chat. Please try again.');
    }
  };

  const handleClose = () => {
    if (!deleteChatMutation.isPending) {
      setDeleteError(null);
      onClose();
    }
  };

  return (
    <Dialog open={open} onClose={handleClose}>
      <DialogTitle>Delete Chat</DialogTitle>
      <DialogContent>
        <Typography>
          Are you sure you want to delete "{chat.name}"? This action cannot be undone.
        </Typography>
        {deleteError && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {deleteError}
          </Alert>
        )}
      </DialogContent>
      <DialogActions>
        <Button 
          onClick={handleClose}
          disabled={deleteChatMutation.isPending}
        >
          Cancel
        </Button>
        <Button 
          onClick={handleDeleteChat}
          color="error"
          disabled={deleteChatMutation.isPending}
          startIcon={deleteChatMutation.isPending ? <CircularProgress size={16} /> : null}
        >
          {deleteChatMutation.isPending ? 'Deleting...' : 'Delete'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteChatDialog;