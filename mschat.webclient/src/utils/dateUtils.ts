/**
 * Formats a date string to a short, human-readable format
 * - Today: Just time (e.g., "2:30 PM")
 * - This week: Day + time (e.g., "Mon 2:30")
 * - This year: Month + day (e.g., "Jan 15")
 * - Older: Month + day + year (e.g., "Jan 15, 23")
 */
export const formatShortTime = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();
  
  // Check if it's today
  const isToday = date.toDateString() === now.toDateString();
  if (isToday) {
    return date.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
  }
  
  // Check if it's this week
  const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
  if (date > oneWeekAgo) {
    return date.toLocaleDateString([], { weekday: 'short' }) + ' ' + 
           date.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
  }
  
  // Check if it's this year
  const isThisYear = date.getFullYear() === now.getFullYear();
  if (isThisYear) {
    return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
  }
  
  // Older than this year
  return date.toLocaleDateString([], { 
    month: 'short', 
    day: 'numeric', 
    year: '2-digit' 
  });
};

/**
 * Truncates text to a specified length and adds ellipsis if needed
 */
export const truncateText = (text: string, maxLength: number = 50): string => {
  if (text.length <= maxLength) {
    return text;
  }
  return text.substring(0, maxLength).trim() + '...';
};

/**
 * Gets a preview of the last message with sender info for group chats
 */
export const getMessagePreview = (
  message: { text: string; senderName: string },
  isPersonalChat: boolean,
  isOwnMessage: boolean,
  maxLength: number = 50
): string => {
  let preview = message.text;
  
  // For group chats, prefix with sender name unless it's your own message
  if (!isPersonalChat && !isOwnMessage) {
    preview = `${message.senderName}: ${message.text}`;
  } else if (!isPersonalChat && isOwnMessage) {
    preview = `You: ${message.text}`;
  }
  
  return truncateText(preview, maxLength);
};