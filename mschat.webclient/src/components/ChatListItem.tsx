import React from 'react';
import {
  Box,
  ListItem,
  ListItemButton,
  Typography,
  Chip,
} from '@mui/material';

interface Chat {
  id: number;
  name: string;
  lastMessage?: string;
  lastMessageTime?: string;
  unreadCount?: number;
}

interface ChatListItemProps {
  chat: Chat;
  isSelected: boolean;
  onSelect: (chatId: number) => void;
}

const ChatListItem: React.FC<ChatListItemProps> = ({ chat, isSelected, onSelect }) => {
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
              {chat.name}
            </Typography>
            {chat.unreadCount && chat.unreadCount > 0 && (
              <Chip
                label={chat.unreadCount}
                size="small"
                sx={{
                  height: 20,
                  fontSize: '0.75rem',
                  minWidth: 20,
                  backgroundColor: isSelected ? 'rgba(255,255,255,0.2)' : 'primary.main',
                  color: isSelected ? 'inherit' : 'primary.contrastText',
                }}
              />
            )}
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