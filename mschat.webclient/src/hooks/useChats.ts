import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { chatApi } from "../lib/chatApi";

export const useChats = () => {
  return useQuery({
    queryKey: ["chats"],
    queryFn: chatApi.getChats,
  });
};

export const useChat = (chatId: number) => {
  return useQuery({
    queryKey: ["chat", chatId],
    queryFn: () => chatApi.getChat(chatId),
    enabled: !!chatId,
  });
};

export const useCreateChat = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: chatApi.createChat,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["chats"] });
    },
  });
};

export const useUpdateChat = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      chat,
    }: {
      chatId: number;
      chat: { name: string };
    }) => chatApi.updateChat(chatId, chat),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["chat", variables.chatId] });
      queryClient.invalidateQueries({ queryKey: ["chats"] });
    },
  });
};

export const useDeleteChat = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: chatApi.deleteChat,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["chats"] });
    },
  });
};
