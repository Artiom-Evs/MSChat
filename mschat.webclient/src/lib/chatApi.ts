import axios, { type AxiosInstance } from "axios";
import type {
  ChatDto,
  CreateChatDto,
  UpdateChatDto,
  ChatParticipantDto,
  AddParticipantDto,
  UpdateParticipantRoleDto,
  MemberDto,
  MessageDto,
  CreateMessageDto,
  UpdateMessageDto,
} from "../types";
import { config } from "../config";

class ChatApiService {
  private _instance?: AxiosInstance;

  // lazy-load AxiosInstance object
  get instance() {
    if (!this._instance) {
      this._instance = this.createInstance();
    }

    return this._instance;
  }

  createInstance() {
    return axios.create({
      baseURL: `${config.chatApiUri}/api`,
      headers: {
        "Content-Type": "application/json",
      },
    });
  }

  setAuthToken(token: string | null) {
    if (token) {
      this.instance.defaults.headers.common[
        "Authorization"
      ] = `Bearer ${token}`;
    } else {
      delete this.instance.defaults.headers.common["Authorization"];
    }
  }

  getChats = (): Promise<ChatDto[]> =>
    this.instance.get<ChatDto[]>("v1/chats").then((response) => response.data);

  getChat = (chatId: number): Promise<ChatDto | null> =>
    this.instance
      .get<ChatDto | null>(`v1/chats/${chatId}`)
      .then((response) => response.data);

  createChat = (chat: CreateChatDto): Promise<ChatDto> =>
    this.instance
      .post<ChatDto>("v1/chats", chat)
      .then((response) => response.data);

  updateChat = (chatId: number, chat: UpdateChatDto): Promise<void> =>
    this.instance.put(`v1/chats/${chatId}`, chat).then(() => {});

  deleteChat = (chatId: number): Promise<void> =>
    this.instance.delete(`v1/chats/${chatId}`).then(() => {});

  // Participant management
  getChatParticipants = (chatId: number): Promise<ChatParticipantDto[]> =>
    this.instance
      .get<ChatParticipantDto[]>(`v1/chats/${chatId}/participants`)
      .then((response) => response.data);

  addParticipant = (
    chatId: number,
    participant: AddParticipantDto
  ): Promise<ChatParticipantDto> =>
    this.instance
      .post<ChatParticipantDto>(`v1/chats/${chatId}/participants`, participant)
      .then((response) => response.data);

  updateParticipantRole = (
    chatId: number,
    participantMemberId: string,
    roleUpdate: UpdateParticipantRoleDto
  ): Promise<void> =>
    this.instance
      .put(
        `v1/chats/${chatId}/participants/${participantMemberId}/role`,
        roleUpdate
      )
      .then(() => {});

  removeParticipant = (
    chatId: number,
    participantMemberId: string
  ): Promise<void> =>
    this.instance
      .delete(`v1/chats/${chatId}/participants/${participantMemberId}`)
      .then(() => {});

  joinChat = (chatId: number): Promise<void> =>
    this.instance.post(`v1/chats/${chatId}/participants/join`).then(() => {});

  leaveChat = (chatId: number): Promise<void> =>
    this.instance.post(`v1/chats/${chatId}/participants/leave`).then(() => {});

  // Members management
  getMembers = (search?: string): Promise<MemberDto[]> =>
    this.instance
      .get<MemberDto[]>("v1/members", {
        params: search ? { search } : undefined,
      })
      .then((response) => response.data);

  getMember = (memberId: string): Promise<MemberDto | null> =>
    this.instance
      .get<MemberDto | null>(`v1/members/${memberId}`)
      .then((response) => response.data);

  getCurrentMember = (): Promise<MemberDto | null> =>
    this.instance
      .get<MemberDto | null>("v1/members/me")
      .then((response) => response.data);

  // Messages management
  getMessages = (
    chatId: number,
    limit: number = 50,
    offset?: number
  ): Promise<MessageDto[]> =>
    this.instance
      .get<MessageDto[]>(`v1/chats/${chatId}/messages`, {
        params: { limit, offset },
      })
      .then((response) => response.data);

  getMessage = (
    chatId: number,
    messageId: number
  ): Promise<MessageDto | null> =>
    this.instance
      .get<MessageDto | null>(`v1/chats/${chatId}/messages/${messageId}`)
      .then((response) => response.data);

  createMessage = (
    chatId: number,
    message: CreateMessageDto
  ): Promise<MessageDto> =>
    this.instance
      .post<MessageDto>(`v1/chats/${chatId}/messages`, message)
      .then((response) => response.data);

  updateMessage = (
    chatId: number,
    messageId: number,
    message: UpdateMessageDto
  ): Promise<void> =>
    this.instance
      .put(`v1/chats/${chatId}/messages/${messageId}`, message)
      .then(() => {});

  deleteMessage = (chatId: number, messageId: number): Promise<void> =>
    this.instance
      .delete(`v1/chats/${chatId}/messages/${messageId}`)
      .then(() => {});
}

export const chatApi = new ChatApiService();
