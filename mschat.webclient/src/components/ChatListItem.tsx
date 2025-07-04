import React from 'react';
import {
  Box,
  ListItem,
  ListItemButton,
  Typography,
} from '@mui/material';
import { useCurrentMember } from '../hooks/useMembers';
import { ChatType, type ChatParticipantDto } from '../types';

interface Chat {
  id: number;
  name: string;
  type: ChatType;
  participants: ChatParticipantDto[];
  lastMessage?: string;
  lastMessageTime?: string;
}

interface ChatListItemProps {
  chat: Chat;
  isSelected: boolean;
  onSelect: (chatId: number) => void;
}

const ChatListItem: React.FC<ChatListItemProps> = ({ chat, isSelected, onSelect }) => {
  const { data: me } = useCurrentMember();

  const title = chat.type == ChatType.Personal
      ? chat.participants?.find(m => me && m.memberId != me?.id)?.memberName ?? chat.name
      : chat.name;
  
  
  return (
    <ListItem disablePadding>
      <ListItemButton
        onClick={() => onSelect(chat.id)}
        selected={isSelected}
        sx={{
          borderRadius: 2,
          mb: 0.5,
          '&.Mui-selected': {
            backgroundColor: 'primary.main',
            color: 'primary.contrastText',
            '&:hover': {
              backgroundColor: 'primary.dark',
            },
            '& .MuiTypography-root': {
              color: 'inherit',
            },
          },
          '&:hover': {
            backgroundColor: 'action.hover',
          },
        }}
      >
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Typography
              variant="subtitle2"
              sx={{
                fontWeight: 600,
                overflow: 'hidden',
                textOverflow: 'ellipsis',
                whiteSpace: 'nowrap',
                flexGrow: 1,
              }}
            >
              {title}
            </Typography>
          </Box>
          <Typography
            variant="body2"
            sx={{
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
              color: isSelected ? 'rgba(255,255,255,0.8)' : 'text.secondary',
              mt: 0.5,
            }}
          >
            {chat.lastMessage ?? "-"}
          </Typography>
          <Typography
            variant="caption"
            sx={{
              color: isSelected ? 'rgba(255,255,255,0.6)' : 'text.secondary',
              mt: 0.5,
              display: 'block',
            }}
          >
            {chat.lastMessageTime ?? "-"}
          </Typography>
        </Box>
      </ListItemButton>
    </ListItem>
  );
};

export default ChatListItem;