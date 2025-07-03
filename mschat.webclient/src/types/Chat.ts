export interface Chat {
  id: number;
  name: string;
  type: number;
  lastMessage?: string;
  lastMessageTime?: string;
  unreadCount?: number;
}
