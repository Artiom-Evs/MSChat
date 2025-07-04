import React from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Avatar,
  Button,
  Chip,
  CircularProgress,
  Alert,
  Stack,
} from '@mui/material';
import MessageIcon from '@mui/icons-material/Message';
import PersonIcon from '@mui/icons-material/Person';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import { useParams, useNavigate } from 'react-router-dom';
import { useCurrentMember, useMember } from '../hooks/useMembers';
import { useCreateChat} from '../hooks/useChats';
import { ChatType } from '../types';

const MemberPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const memberId = id ? parseInt(id, 10) : 0;
  
  const { data: member, isLoading, error } = useMember(memberId);
  const { data: me } = useCurrentMember();
  const createPersonalChatMutation = useCreateChat();

  const handleSendMessage = async () => {
    if (!member) return;

    try {
      const chat = await createPersonalChatMutation.mutateAsync({
        name: "Personal chat",
        type: ChatType.Personal,
        otherMemberId: member.id,
      });
      navigate(`/chats/${chat.id}`);
    } catch (error) {
      console.error('Failed to create or get personal chat:', error);
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        Error loading member information. Please try again.
      </Alert>
    );
  }

  if (!member) {
    return (
      <Alert severity="warning" sx={{ mb: 2 }}>
        Member not found.
      </Alert>
    );
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  return (
    <Box sx={{ maxWidth: 600, mx: 'auto' }}>
      <Card sx={{ mb: 3 }}>
        <CardContent sx={{ p: 4 }}>
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 4 }}>
            <Avatar
              src={member.photoUrl || undefined}
              sx={{
                width: 120,
                height: 120,
                mb: 2,
                fontSize: '3rem',
              }}
            >
              {!member.photoUrl && <PersonIcon sx={{ fontSize: '3rem' }} />}
            </Avatar>
            
            <Typography variant="h4" component="h1" gutterBottom textAlign="center">
              {member.name}
            </Typography>

            {member.deletedAt && (
              <Chip
                label="Inactive"
                color="error"
                size="small"
                sx={{ mb: 2 }}
              />
            )}
          </Box>

          <Stack spacing={3}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <CalendarTodayIcon color="action" />
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Member since
                </Typography>
                <Typography variant="body1">
                  {formatDate(member.createdAt)}
                </Typography>
              </Box>
            </Box>

            {member.deletedAt && (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <CalendarTodayIcon color="action" />
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    Account deactivated
                  </Typography>
                  <Typography variant="body1">
                    {formatDate(member.deletedAt)}
                  </Typography>
                </Box>
              </Box>
            )}
          </Stack>
        </CardContent>
      </Card>

      {!member.deletedAt && member.id != me?.id && (
        <Card>
          <CardContent sx={{ p: 4 }}>
            <Typography variant="h6" gutterBottom>
              Actions
            </Typography>
            
            <Button
              variant="contained"
              startIcon={<MessageIcon />}
              onClick={handleSendMessage}
              disabled={createPersonalChatMutation.isPending}
              fullWidth
              size="large"
              sx={{ mt: 2 }}
            >
              {createPersonalChatMutation.isPending ? (
                <>
                  <CircularProgress size={20} sx={{ mr: 1 }} />
                  Creating chat...
                </>
              ) : (
                'Send Message'
              )}
            </Button>

            <Typography variant="body2" color="text.secondary" sx={{ mt: 2, textAlign: 'center' }}>
              Start a personal conversation with {member.name}
            </Typography>

            {createPersonalChatMutation.isError && (
              <Alert severity="error" sx={{ mt: 2 }}>
                Failed to create chat. Please try again.
              </Alert>
            )}
          </CardContent>
        </Card>
      )}
    </Box>
  );
};

export default MemberPage;