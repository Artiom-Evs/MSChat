import axios, { type AxiosInstance } from "axios";
import type {
  ChatDto,
  CreateChatDto,
  UpdateChatDto,
  ChatParticipantDto,
  AddParticipantDto,
  UpdateParticipantRoleDto,
  MemberDto,
} from "../types";

export const apiUri = import.meta.env.VITE_CHAT_API_URI;

if (!apiUri) {
  throw new Error(
    "VITE_CHAT_API_URI is not defined in the environment variables."
  );
}

export const chatApiInstance: AxiosInstance = axios.create({
  baseURL: `${apiUri}/api`,
  headers: {
    "Content-Type": "application/json",
  },
});

// Auth interceptor will be set up in the auth context
chatApiInstance.interceptors.request.use((config) => {
  // Token will be injected by auth context
  return config;
});

export const chatApi = {
  getChats: (): Promise<ChatDto[]> =>
    chatApiInstance
      .get<ChatDto[]>("v1/chats")
      .then((response) => response.data),

  getChat: (chatId: number): Promise<ChatDto | null> =>
    chatApiInstance
      .get<ChatDto | null>(`v1/chats/${chatId}`)
      .then((response) => response.data),

  createChat: (chat: CreateChatDto): Promise<ChatDto> =>
    chatApiInstance
      .post<ChatDto>("v1/chats", chat)
      .then((response) => response.data),

  updateChat: (chatId: number, chat: UpdateChatDto): Promise<void> =>
    chatApiInstance.put(`v1/chats/${chatId}`, chat).then(() => {}),

  deleteChat: (chatId: number): Promise<void> =>
    chatApiInstance.delete(`v1/chats/${chatId}`).then(() => {}),

  // Participant management
  getChatParticipants: (chatId: number): Promise<ChatParticipantDto[]> =>
    chatApiInstance
      .get<ChatParticipantDto[]>(`v1/chats/${chatId}/participants`)
      .then((response) => response.data),

  addParticipant: (
    chatId: number,
    participant: AddParticipantDto
  ): Promise<ChatParticipantDto> =>
    chatApiInstance
      .post<ChatParticipantDto>(`v1/chats/${chatId}/participants`, participant)
      .then((response) => response.data),

  updateParticipantRole: (
    chatId: number,
    participantMemberId: number,
    roleUpdate: UpdateParticipantRoleDto
  ): Promise<void> =>
    chatApiInstance
      .put(
        `v1/chats/${chatId}/participants/${participantMemberId}/role`,
        roleUpdate
      )
      .then(() => {}),

  removeParticipant: (
    chatId: number,
    participantMemberId: number
  ): Promise<void> =>
    chatApiInstance
      .delete(`v1/chats/${chatId}/participants/${participantMemberId}`)
      .then(() => {}),

  joinChat: (chatId: number): Promise<void> =>
    chatApiInstance.post(`v1/chats/${chatId}/participants/join`).then(() => {}),

  leaveChat: (chatId: number): Promise<void> =>
    chatApiInstance
      .post(`v1/chats/${chatId}/participants/leave`)
      .then(() => {}),

  // Members management
  getMembers: (search?: string): Promise<MemberDto[]> =>
    chatApiInstance
      .get<MemberDto[]>("v1/members", {
        params: search ? { search } : undefined,
      })
      .then((response) => response.data),

  getMember: (memberId: number): Promise<MemberDto | null> =>
    chatApiInstance
      .get<MemberDto | null>(`v1/members/${memberId}`)
      .then((response) => response.data),

  getCurrentMember: (): Promise<MemberDto | null> =>
    chatApiInstance
      .get<MemberDto | null>("v1/members/me")
      .then((response) => response.data),
};
