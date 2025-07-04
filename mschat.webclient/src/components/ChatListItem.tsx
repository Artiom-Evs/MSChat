import React from 'react';
import {
  Box,
  ListItem,
  ListItemButton,
  Typography,
} from '@mui/material';
import { useCurrentMember } from '../hooks/useMembers';
import { ChatType, type ChatParticipantDto, type MessageDto } from '../types';
import { formatShortTime, getMessagePreview } from '../utils/dateUtils';

interface Chat {
  id: number;
  name: string;
  type: ChatType;
  participants: ChatParticipantDto[];
  lastMessage?: MessageDto;
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

  const isPersonalChat = chat.type === ChatType.Personal;
  const isOwnMessage = chat.lastMessage?.senderId === me?.id;
  
  const lastMessagePreview = chat.lastMessage 
    ? getMessagePreview(
        { text: chat.lastMessage.text, senderName: chat.lastMessage.senderName },
        isPersonalChat,
        isOwnMessage,
        45 // Shorter for better fit
      )
    : "No messages yet";

  const lastMessageTime = chat.lastMessage?.createdAt 
    ? formatShortTime(chat.lastMessage.createdAt)
    : "";
  
  return (
    <ListItem disablePadding>
      <ListItemButton
        onClick={() => onSelect(chat.id)}
        selected={isSelected}
        sx={{
          borderRadius: 2,
          mb: 0.5,
          py: 1.5,
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
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 0.5 }}>
            <Typography
              variant="subtitle2"
              sx={{
                fontWeight: 600,
                overflow: 'hidden',
                textOverflow: 'ellipsis',
                whiteSpace: 'nowrap',
                flexGrow: 1,
                pr: 1,
              }}
            >
              {title}
            </Typography>
            {lastMessageTime && (
              <Typography
                variant="caption"
                sx={{
                  color: isSelected ? 'rgba(255,255,255,0.7)' : 'text.secondary',
                  fontSize: '0.75rem',
                  flexShrink: 0,
                }}
              >
                {lastMessageTime}
              </Typography>
            )}
          </Box>
          <Typography
            variant="body2"
            sx={{
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
              color: isSelected ? 'rgba(255,255,255,0.8)' : 'text.secondary',
              fontSize: '0.875rem',
            }}
          >
            {lastMessagePreview}
          </Typography>
        </Box>
      </ListItemButton>
    </ListItem>
  );
};

export default ChatListItem;