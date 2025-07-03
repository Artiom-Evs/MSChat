import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
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
import { useCreateChat } from '../hooks/useChats';
import { ChatType } from '../types';

const CreateChatPage: React.FC = () => {
  const [name, setName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  
  const createChatMutation = useCreateChat();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!name.trim()) {
      setError('Chat name is required');
      return;
    }

    try {
      const newChat = await createChatMutation.mutateAsync({
        name: name.trim(),
        type: ChatType.Public,
      });
      
      navigate(`/chats/${newChat.id}`);
    } catch (err) {
      setError('Failed to create chat. Please try again.');
    }
  };

  const handleCancel = () => {
    navigate('/chats');
  };

  return (
    <Container maxWidth="xs" sx={{ py:4 }}>
      <Card>
        <CardContent>
          <Typography variant="h5" component="h1" gutterBottom>
            Create New Public Chat
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
              disabled={createChatMutation.isPending}
              inputProps={{ maxLength: 100 }}
            />
            
            <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
              <Button
                type="submit"
                variant="contained"
                disabled={createChatMutation.isPending}
                startIcon={createChatMutation.isPending ? <CircularProgress size={20} /> : null}
              >
                {createChatMutation.isPending ? 'Creating...' : 'Create Chat'}
              </Button>
              <Button
                variant="outlined"
                onClick={handleCancel}
                disabled={createChatMutation.isPending}
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

export default CreateChatPage;