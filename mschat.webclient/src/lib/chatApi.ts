import axios, { type AxiosInstance } from "axios";
import type { ChatDto, CreateChatDto, UpdateChatDto } from "../types";

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
    chatApiInstance.get<ChatDto[]>("v1/chats").then((response) => response.data),

  getChat: (chatId: number): Promise<ChatDto | null> =>
    chatApiInstance
      .get<ChatDto | null>(`v1/chats/${chatId}`)
      .then((response) => response.data),

  createChat: (chat: CreateChatDto): Promise<ChatDto> =>
    chatApiInstance
      .post<ChatDto>("v1/chats", chat)
      .then((response) => response.data),

  updateChat: (chatId: number, chat: UpdateChatDto): Promise<ChatDto> =>
    chatApiInstance
      .put<ChatDto>(`v1/chats/${chatId}`, chat)
      .then((response) => response.data),

  deleteChat: (chatId: number): Promise<void> =>
    chatApiInstance.delete(`v1/chats/${chatId}`).then(() => {}),
};
