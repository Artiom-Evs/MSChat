import React from 'react';
import { 
  Box, 
  Typography, 
  Paper, 
  IconButton, 
  List, 
  ListItem, 
  ListItemAvatar, 
  ListItemText, 
  Avatar, 
  Chip, 
  CircularProgress,
  Divider
} from '@mui/material';
import { ArrowBack, Person } from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { useChat } from '../hooks/useChats';
import { useParticipants } from '../hooks/useParticipants';
import { ChatType, ChatRole } from '../types';
import { useCurrentMember } from '../hooks/useMembers';

const ChatParticipantsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  const chatId = id ? parseInt(id, 10) : null;
  const { data: selectedChat, isLoading: chatLoading } = useChat(chatId || 0);
  const { data: participants, isLoading: participantsLoading } = useParticipants(chatId || 0);
  const { data: me } = useCurrentMember();

  const isLoading = chatLoading || participantsLoading;

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
        <CircularProgress />
        <Typography variant="h6" color="text.secondary">
          Loading participants...
        </Typography>
      </Box>
    );
  }

  if (!selectedChat || !participants) {
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
          Chat not found
        </Typography>
        <Typography variant="body1" color="text.secondary">
          The chat or participants could not be loaded
        </Typography>
      </Box>
    );
  }

  const title = selectedChat.type === ChatType.Personal
    ? selectedChat.participants?.find(m => me && m.memberId !== me?.id)?.memberName ?? selectedChat.name
    : selectedChat.name;

  const getRoleColor = (role: ChatRole) => {
    switch (role) {
      case ChatRole.Owner:
        return 'primary';
      case ChatRole.Admin:
        return 'secondary';
      case ChatRole.Member:
        return 'default';
      default:
        return 'default';
    }
  };

  const getRoleLabel = (role: ChatRole) => {
    switch (role) {
      case ChatRole.Owner:
        return 'Owner';
      case ChatRole.Admin:
        return 'Admin';
      case ChatRole.Member:
        return 'Member';
      default:
        return 'Member';
    }
  };

  const handleMemberClick = (memberId: number) => {
    navigate(`/members/${memberId}`);
  };

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      {/* Header */}
      <Paper 
        elevation={1} 
        sx={{ 
          p: 2, 
          borderRadius: 0,
          borderBottom: '1px solid',
          borderColor: 'divider'
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <IconButton
            onClick={() => navigate(`/chats/${selectedChat.id}`)}
            sx={{
              color: 'text.secondary',
              '&:hover': {
                color: 'primary.main',
              },
            }}
          >
            <ArrowBack />
          </IconButton>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              {title}
            </Typography>
            <Chip
              label={`${participants.length} ${participants.length == 1 ? "member" : "members"}`}
              size="small"
              color="primary"
              variant="outlined"
            />
          </Box>
        </Box>
      </Paper>
      
      {/* Participants List */}
      <Box sx={{ flexGrow: 1, overflow: 'auto' }}>
        <List sx={{ width: '100%', bgcolor: 'background.paper' }}>
          {participants.map((participant, index) => (
            <React.Fragment key={participant.memberId}>
              <ListItem 
                alignItems="flex-start"
                button
                onClick={() => handleMemberClick(participant.memberId)}
                sx={{
                  cursor: 'pointer',
                  '&:hover': {
                    backgroundColor: 'action.hover',
                  },
                }}
              >
                <ListItemAvatar>
                  <Avatar 
                    src={participant.memberPhotoUrl || undefined}
                    sx={{ bgcolor: 'primary.main' }}
                  >
                    {participant.memberPhotoUrl ? undefined : <Person />}
                  </Avatar>
                </ListItemAvatar>
                <ListItemText
                  primary={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Typography variant="subtitle1" component="span">
                        {participant.memberName}
                      </Typography>
                      {participant.memberId === me?.id && (
                        <Chip 
                          label="You" 
                          size="small" 
                          color="info" 
                          variant="outlined"
                        />
                      )}
                    </Box>
                  }
                  secondary={
                    <Box sx={{ mt: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Chip
                        label={getRoleLabel(participant.roleInChat)}
                        size="small"
                        color={getRoleColor(participant.roleInChat)}
                        variant="filled"
                      />
                      <Typography variant="body2" color="text.secondary">
                        Joined {new Date(participant.joinedAt).toLocaleDateString()}
                      </Typography>
                    </Box>
                  }
                />
              </ListItem>
              {index < participants.length - 1 && <Divider variant="inset" component="li" />}
            </React.Fragment>
          ))}
        </List>
        
        {participants.length === 0 && (
          <Box 
            sx={{ 
              display: 'flex', 
              alignItems: 'center', 
              justifyContent: 'center', 
              height: '200px',
              flexDirection: 'column',
              gap: 2
            }}
          >
            <Person sx={{ fontSize: 48, color: 'text.secondary' }} />
            <Typography variant="h6" color="text.secondary">
              No participants found
            </Typography>
          </Box>
        )}
      </Box>
    </Box>
  );
};

export default ChatParticipantsPage;