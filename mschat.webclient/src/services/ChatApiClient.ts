import axios, { type AxiosInstance } from "axios";
import type { Chat } from "../types";

const apiUri = import.meta.env.VITE_CHAT_API_URI;

if (!apiUri) {
  throw new Error(
    "VITE_CHAT_API_URI is not defined in the environment variables."
  );
}

export class ChatApiClient {
  instance: AxiosInstance;

  constructor() {
    this.instance = axios.create({
      baseURL: `${apiUri}/api`,
      headers: {
        "Content-Type": "application/json",
      },
    });
  }

  getChats(): Promise<Chat[]> {
    return this.instance
      .get<Chat[]>(`v1/chats`)
      .then((response) => response.data);
  }

  getChat(chatId: number): Promise<Chat | null> {
    return this.instance
      .get<Chat | null>(`v1/chats/${chatId}`)
      .then((response) => response.data);
  }

  createChat(chat: Omit<Chat, "id">): Promise<Chat> {
    return this.instance
      .post<Chat>(`v1/chats`, chat)
      .then((response) => response.data);
  }

  updateChat(chat: Chat): Promise<Chat> {
    return this.instance
      .put<Chat>(`v1/chats/${chat.id}`, chat)
      .then((response) => response.data);
  }

  deleteChat(chatId: number): Promise<void> {
    return this.instance.delete(`v1/chats/${chatId}`).then(() => {});
  }
}
