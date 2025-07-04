import React, { useEffect, useRef, useState } from 'react';
import {
  Box,
  CircularProgress,
  Typography,
  Button,
  Fade,
  IconButton,
} from '@mui/material';
import { KeyboardArrowDown, Refresh } from '@mui/icons-material';
import { useInfiniteMessages } from '../hooks/useMessages';
import MessageItem from './MessageItem';

interface MessageListProps {
  chatId: number;
}

const MessageList: React.FC<MessageListProps> = ({ chatId }) => {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
    isError,
    refetch,
  } = useInfiniteMessages(chatId);

  const messagesEndRef = useRef<HTMLDivElement>(null);
  const scrollContainerRef = useRef<HTMLDivElement>(null);
  const [showScrollToBottom, setShowScrollToBottom] = useState(false);
  const [isUserScrolling, setIsUserScrolling] = useState(false);

  // Flatten all pages into a single array of messages
  const messages = data?.pages.flat() || [];

  // Auto-scroll to bottom for new messages (only if user hasn't scrolled up)
  useEffect(() => {
    if (!isUserScrolling && messages.length > 0) {
      messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages.length, isUserScrolling]);

  // Handle scroll events to detect if user is scrolling
  const handleScroll = () => {
    const container = scrollContainerRef.current;
    if (!container) return;

    const { scrollTop, scrollHeight, clientHeight } = container;
    const isAtBottom = scrollHeight - scrollTop - clientHeight < 100;

    setShowScrollToBottom(!isAtBottom);
    setIsUserScrolling(!isAtBottom);

    // Load more messages when scrolling to top
    if (scrollTop === 0 && hasNextPage && !isFetchingNextPage) {
      const currentScrollHeight = scrollHeight;
      fetchNextPage().then(() => {
        // Maintain scroll position after loading more messages
        setTimeout(() => {
          if (container) {
            container.scrollTop = container.scrollHeight - currentScrollHeight;
          }
        }, 100);
      });
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    setIsUserScrolling(false);
  };

  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100%',
          flexDirection: 'column',
          gap: 2,
        }}
      >
        <CircularProgress />
        <Typography variant="body2" color="text.secondary">
          Loading messages...
        </Typography>
      </Box>
    );
  }

  if (isError) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100%',
          flexDirection: 'column',
          gap: 2,
        }}
      >
        <Typography variant="h6" color="error">
          Failed to load messages
        </Typography>
        <Button
          variant="outlined"
          startIcon={<Refresh />}
          onClick={() => refetch()}
        >
          Try Again
        </Button>
      </Box>
    );
  }

  if (messages.length === 0) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100%',
          flexDirection: 'column',
          gap: 2,
        }}
      >
        <Typography variant="h6" color="text.secondary">
          No messages yet
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Start the conversation by sending a message
        </Typography>
      </Box>
    );
  }

  return (
    <Box
      sx={{
        position: 'relative',
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
      }}
    >
      {/* Messages Container */}
      <Box
        ref={scrollContainerRef}
        onScroll={handleScroll}
        sx={{
          flex: 1,
          overflowY: 'auto',
          p: 1,
          display: 'flex',
          flexDirection: 'column',
          '&::-webkit-scrollbar': {
            width: '6px',
          },
          '&::-webkit-scrollbar-track': {
            background: 'transparent',
          },
          '&::-webkit-scrollbar-thumb': {
            background: 'rgba(0,0,0,0.2)',
            borderRadius: '3px',
          },
          '&::-webkit-scrollbar-thumb:hover': {
            background: 'rgba(0,0,0,0.3)',
          },
        }}
      >
        {/* Load More Messages Indicator */}
        {isFetchingNextPage && (
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'center',
              py: 1,
            }}
          >
            <CircularProgress size={20} />
          </Box>
        )}

        {/* End of Messages Indicator */}
        {!hasNextPage && messages.length > 0 && (
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'center',
              py: 1,
            }}
          >
            <Typography variant="caption" color="text.secondary" sx={{ fontSize: '0.7rem' }}>
              Beginning of conversation
            </Typography>
          </Box>
        )}

        {/* Messages */}
        {messages.map((message) => (
          <MessageItem
            key={message.id}
            message={message}
            chatId={chatId}
          />
        ))}

        {/* Scroll anchor */}
        <div ref={messagesEndRef} />
      </Box>

      {/* Scroll to Bottom Button */}
      <Fade in={showScrollToBottom}>
        <IconButton
          onClick={scrollToBottom}
          sx={{
            position: 'absolute',
            bottom: 8,
            right: 8,
            width: 32,
            height: 32,
            backgroundColor: 'primary.main',
            color: 'primary.contrastText',
            boxShadow: 1,
            '&:hover': {
              backgroundColor: 'primary.dark',
            },
          }}
        >
          <KeyboardArrowDown fontSize="small" />
        </IconButton>
      </Fade>
    </Box>
  );
};

export default MessageList;