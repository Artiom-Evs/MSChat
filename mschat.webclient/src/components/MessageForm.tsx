import React, { useState, useRef } from 'react';
import {
  Box,
  TextField,
  IconButton,
  Paper,
  Typography,
  CircularProgress,
} from '@mui/material';
import { Send, AttachFile } from '@mui/icons-material';
import { useCreateMessage } from '../hooks/useMessages';

interface MessageFormProps {
  chatId: number;
  disabled?: boolean;
}

const MessageForm: React.FC<MessageFormProps> = ({ chatId, disabled = false }) => {
  const [message, setMessage] = useState('');
  const [isMultiline, setIsMultiline] = useState(false);
  const textFieldRef = useRef<HTMLTextAreaElement>(null);
  const createMessage = useCreateMessage();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const trimmedMessage = message.trim();
    if (!trimmedMessage || disabled) return;

    try {
      await createMessage.mutateAsync({
        chatId,
        message: { text: trimmedMessage },
      });
      setMessage('');
      setIsMultiline(false);
      
      // Reset textarea height
      if (textFieldRef.current) {
        textFieldRef.current.style.height = 'auto';
      }
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSubmit(e);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const value = e.target.value;
    setMessage(value);
    
    // Auto-resize textarea
    const textarea = e.target;
    textarea.style.height = 'auto';
    const newHeight = Math.min(textarea.scrollHeight, 120); // Max 5 lines
    textarea.style.height = `${newHeight}px`;
    
    // Check if multiline
    setIsMultiline(value.includes('\n') || textarea.scrollHeight > 56);
  };

  const canSend = message.trim().length > 0 && !createMessage.isPending && !disabled;

  return (
    <Paper
      elevation={0}
      sx={{
        p: 1,
        borderRadius: 0,
        borderTop: '1px solid',
        borderColor: 'divider',
        backgroundColor: 'background.paper',
      }}
    >
      <Box component="form" onSubmit={handleSubmit}>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'flex-end',
            gap: 0.5,
          }}
        >
          {/* File attachment button (placeholder for future implementation) */}
          <IconButton
            size="small"
            disabled
            sx={{
              mb: 0.25,
              opacity: 0.3,
              width: 32,
              height: 32,
            }}
          >
            <AttachFile fontSize="small" />
          </IconButton>

          {/* Message input */}
          <TextField
            inputRef={textFieldRef}
            fullWidth
            multiline
            maxRows={3}
            value={message}
            onChange={handleInputChange}
            onKeyDown={handleKeyDown}
            placeholder={disabled ? "You cannot send messages to this chat" : "Type a message..."}
            disabled={disabled}
            variant="outlined"
            size="small"
            sx={{
              '& .MuiOutlinedInput-root': {
                borderRadius: 2,
                backgroundColor: disabled ? 'action.disabledBackground' : 'background.default',
                minHeight: 32,
                '& fieldset': {
                  borderColor: 'divider',
                },
                '&:hover fieldset': {
                  borderColor: 'primary.main',
                },
                '&.Mui-focused fieldset': {
                  borderColor: 'primary.main',
                  borderWidth: 1,
                },
              },
              '& .MuiInputBase-input': {
                padding: '8px 12px',
                fontSize: '0.875rem',
                lineHeight: '1.4',
                resize: 'none',
                '&::placeholder': {
                  color: 'text.secondary',
                  opacity: 0.7,
                },
              },
            }}
          />

          {/* Send button */}
          <IconButton
            type="submit"
            disabled={!canSend}
            sx={{
              mb: 0.25,
              width: 32,
              height: 32,
              backgroundColor: canSend ? 'primary.main' : 'action.disabledBackground',
              color: canSend ? 'primary.contrastText' : 'text.disabled',
              '&:hover': {
                backgroundColor: canSend ? 'primary.dark' : 'action.disabledBackground',
              },
              '&:disabled': {
                backgroundColor: 'action.disabledBackground',
                color: 'text.disabled',
              },
            }}
          >
            {createMessage.isPending ? (
              <CircularProgress size={16} color="inherit" />
            ) : (
              <Send sx={{ fontSize: 16 }} />
            )}
          </IconButton>
        </Box>

        {/* Helper text */}
        {isMultiline && (
          <Typography
            variant="caption"
            color="text.secondary"
            sx={{
              display: 'block',
              mt: 0.5,
              ml: 4, // Account for attachment button width
              fontSize: '0.7rem',
            }}
          >
            Enter to send, Shift+Enter for new line
          </Typography>
        )}

        {/* Error message */}
        {createMessage.isError && (
          <Typography
            variant="caption"
            color="error"
            sx={{
              display: 'block',
              mt: 0.5,
              ml: 4,
              fontSize: '0.7rem',
            }}
          >
            Failed to send message. Please try again.
          </Typography>
        )}
      </Box>
    </Paper>
  );
};

export default MessageForm;