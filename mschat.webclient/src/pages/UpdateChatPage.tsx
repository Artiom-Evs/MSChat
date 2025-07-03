import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Button,
  Card,
  CardContent,
  Container,
  TextField,
  Typography,
  CircularProgress,
  Alert,
} from '@mui/material';
import { useChat, useUpdateChat } from '../hooks/useChats';

const UpdateChatPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [name, setName] = useState('');
  const [error, setError] = useState<string | null>(null);
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: chat, isLoading: isLoadingChat } = useChat(chatId || 0);
  const updateChatMutation = useUpdateChat();

  useEffect(() => {
    if (chat) {
      setName(chat.name);
    }
  }, [chat]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!name.trim()) {
      setError('Chat name is required');
      return;
    }

    if (!chatId) {
      setError('Invalid chat ID');
      return;
    }

    try {
      await updateChatMutation.mutateAsync({
        chatId,
        chat: {
          name: name.trim(),
        },
      });
      
      navigate(`/chats/${chatId}`);
    } catch (err) {
      setError('Failed to update chat. Please try again.');
    }
  };

  const handleCancel = () => {
    navigate(`/chats/${chatId}`);
  };

  if (isLoadingChat) {
    return (
      <Container maxWidth="sm" sx={{ py: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '200px' }}>
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  if (!chat) {
    return (
      <Container maxWidth="sm" sx={{ py: 4 }}>
        <Alert severity="error">
          Chat not found
        </Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="xs" sx={{ py: 4 }}>
      <Card>
        <CardContent>
          <Typography variant="h5" component="h1" gutterBottom>
            Edit Chat
          </Typography>
          
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          
          <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
            <TextField
              fullWidth
              label="Chat Name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              margin="normal"
              required
              disabled={updateChatMutation.isPending}
              inputProps={{ maxLength: 100 }}
            />
            
            <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
              <Button
                type="submit"
                variant="contained"
                disabled={updateChatMutation.isPending}
                startIcon={updateChatMutation.isPending ? <CircularProgress size={20} /> : null}
              >
                {updateChatMutation.isPending ? 'Updating...' : 'Update Chat'}
              </Button>
              <Button
                variant="outlined"
                onClick={handleCancel}
                disabled={updateChatMutation.isPending}
              >
                Cancel
              </Button>
            </Box>
          </Box>
        </CardContent>
      </Card>
    </Container>
  );
};

export default UpdateChatPage;