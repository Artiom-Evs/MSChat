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
  participants: ChatParticipantDto[];
  createdAt: string;
  deletedAt?: string;
}

export interface CreateChatDto {
  name: string;
  type: ChatType;
  // used for personal chats creation
  otherMemberId?: number;
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

export interface MemberDto {
  id: number;
  name: string;
  photoUrl?: string;
  createdAt: string;
  deletedAt?: string;
}

export interface MessageDto {
  id: number;
  chatId: number;
  senderId: number;
  senderName: string;
  senderPhotoUrl?: string;
  text: string;
  createdAt: string;
  updatedAt?: string;
  deletedAt?: string;
  isDeleted: boolean;
  isEdited: boolean;
}

export interface CreateMessageDto {
  text: string;
}

export interface UpdateMessageDto {
  text: string;
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
