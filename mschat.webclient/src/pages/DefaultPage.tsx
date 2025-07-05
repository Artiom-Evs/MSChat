import React from 'react';
import { 
  Box, 
  Typography, 
  Button,
  Card,
  CardContent,
} from '@mui/material';
import { Chat, Group, Search, Add } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

const DefaultPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        height: '100%',
        textAlign: 'center',
        px: 3,
      }}
    >
      <Box sx={{ mb: 6 }}>
        <Chat sx={{ fontSize: 80, color: 'primary.main', mb: 2 }} />
        <Typography variant="h3" component="h1" gutterBottom sx={{ fontWeight: 600 }}>
          Welcome to MSChat
        </Typography>
        <Typography variant="h6" color="text.secondary" sx={{ mb: 4, maxWidth: 500 }}>
          Connect with your team, share ideas, and collaborate seamlessly. Start by selecting a chat from the sidebar or create a new one.
        </Typography>
      </Box>

      <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', justifyContent: 'center', mb: 4 }}>
        <Button
          variant="contained"
          size="large"
          startIcon={<Add />}
          onClick={() => navigate('/chats/create')}
          sx={{ minWidth: 160 }}
        >
          Create Chat
        </Button>
        <Button
          variant="outlined"
          size="large"
          startIcon={<Search />}
          onClick={() => navigate('/search')}
          sx={{ minWidth: 160 }}
        >
          Search Chats
        </Button>
      </Box>

      <Box sx={{ display: 'flex', gap: 3, maxWidth: 800, width: '100%' }}>
        <Card sx={{ flex: 1 }}>
          <CardContent sx={{ textAlign: 'center', py: 3 }}>
            <Group sx={{ fontSize: 40, color: 'primary.main', mb: 2 }} />
            <Typography variant="h6" gutterBottom>
              Public Chats
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Join public conversations and connect with the entire team
            </Typography>
          </CardContent>
        </Card>

        <Card sx={{ flex: 1 }}>
          <CardContent sx={{ textAlign: 'center', py: 3 }}>
            <Chat sx={{ fontSize: 40, color: 'secondary.main', mb: 2 }} />
            <Typography variant="h6" gutterBottom>
              Personal Chats
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Have private conversations with individual team members
            </Typography>
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
};

export default DefaultPage;