import axios, { type AxiosInstance } from "axios";
import type { Chat } from "../types";

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
  getChats: (): Promise<Chat[]> =>
    chatApiInstance.get<Chat[]>("v1/chats").then((response) => response.data),

  getChat: (chatId: number): Promise<Chat | null> =>
    chatApiInstance
      .get<Chat | null>(`v1/chats/${chatId}`)
      .then((response) => response.data),

  createChat: (chat: Omit<Chat, "id">): Promise<Chat> =>
    chatApiInstance
      .post<Chat>("v1/chats", chat)
      .then((response) => response.data),

  updateChat: (chat: Chat): Promise<Chat> =>
    chatApiInstance
      .put<Chat>(`v1/chats/${chat.id}`, chat)
      .then((response) => response.data),

  deleteChat: (chatId: number): Promise<void> =>
    chatApiInstance.delete(`v1/chats/${chatId}`).then(() => {}),
};
