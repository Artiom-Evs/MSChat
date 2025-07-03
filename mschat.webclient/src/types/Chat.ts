export type ChatType = (typeof ChatType)[keyof typeof ChatType];
export const ChatType = {
  Public: 1,
  Personal: 2,
} as const;

export type ChatRole = (typeof ChatRole)[keyof typeof ChatRole];
export const ChatRole = {
  Owner: 1,
  Member: 2,
};

export interface ChatDto {
  id: number;
  name: string;
  type: ChatType;
  isInChat: boolean;
  createdAt: string;
  deletedAt?: string;
}

export interface CreateChatDto {
  name: string;
  type: ChatType;
}

export interface UpdateChatDto {
  name: string;
}

export interface ChatParticipantDto {
  chatId: number;
  memberId: number;
  memberName: string;
  memberPhotoUrl?: string;
  roleInChat: ChatRole;
  joinedAt: string;
}

export interface AddParticipantDto {
  memberId: number;
  roleInChat?: ChatRole;
}

export interface UpdateParticipantRoleDto {
  roleInChat: ChatRole;
}

// Legacy interface for backward compatibility
export interface Chat {
  id: number;
  name: string;
  type: number;
  lastMessage?: string;
  lastMessageTime?: string;
  unreadCount?: number;
}
