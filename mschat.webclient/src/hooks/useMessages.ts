import { useQuery, useMutation, useQueryClient, useInfiniteQuery } from "@tanstack/react-query";
import { chatApi } from "../lib/chatApi";
import type { CreateMessageDto, UpdateMessageDto } from "../types";

export const useMessages = (chatId: number, page: number = 1, pageSize: number = 50) => {
  return useQuery({
    queryKey: ["messages", chatId, page, pageSize],
    queryFn: () => chatApi.getMessages(chatId, page, pageSize),
    enabled: !!chatId,
  });
};

export const useInfiniteMessages = (chatId: number, pageSize: number = 50) => {
  return useInfiniteQuery({
    queryKey: ["messages", chatId, "infinite"],
    queryFn: ({ pageParam = 1 }) => chatApi.getMessages(chatId, pageParam, pageSize),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) => {
      // If the last page has fewer items than pageSize, we've reached the end
      if (lastPage.length < pageSize) {
        return undefined;
      }
      return allPages.length + 1;
    },
    enabled: !!chatId,
  });
};

export const useMessage = (chatId: number, messageId: number) => {
  return useQuery({
    queryKey: ["message", chatId, messageId],
    queryFn: () => chatApi.getMessage(chatId, messageId),
    enabled: !!chatId && !!messageId,
  });
};

export const useCreateMessage = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      message,
    }: {
      chatId: number;
      message: CreateMessageDto;
    }) => chatApi.createMessage(chatId, message),
    onSuccess: (newMessage, variables) => {
      // Invalidate and refetch messages for this chat
      queryClient.invalidateQueries({
        queryKey: ["messages", variables.chatId],
      });
      
      // Optionally update the infinite query cache directly for better UX
      queryClient.setQueryData(
        ["messages", variables.chatId, "infinite"],
        (oldData: unknown) => {
          if (!oldData || typeof oldData !== 'object' || !('pages' in oldData)) {
            return oldData;
          }
          
          const data = oldData as { pages: Array<unknown[]>; pageParams: unknown[] };
          
          // Add the new message to the first page (most recent messages)
          const newPages = [...data.pages];
          if (newPages[0]) {
            newPages[0] = [newMessage, ...newPages[0]];
          }
          
          return {
            ...data,
            pages: newPages,
          };
        }
      );
    },
  });
};

export const useUpdateMessage = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      messageId,
      message,
    }: {
      chatId: number;
      messageId: number;
      message: UpdateMessageDto;
    }) => chatApi.updateMessage(chatId, messageId, message),
    onSuccess: (_, variables) => {
      // Invalidate messages queries for this chat
      queryClient.invalidateQueries({
        queryKey: ["messages", variables.chatId],
      });
      
      // Invalidate the specific message query
      queryClient.invalidateQueries({
        queryKey: ["message", variables.chatId, variables.messageId],
      });
    },
  });
};

export const useDeleteMessage = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      messageId,
    }: {
      chatId: number;
      messageId: number;
    }) => chatApi.deleteMessage(chatId, messageId),
    onSuccess: (_, variables) => {
      // Invalidate messages queries for this chat
      queryClient.invalidateQueries({
        queryKey: ["messages", variables.chatId],
      });
      
      // Remove the specific message from cache
      queryClient.removeQueries({
        queryKey: ["message", variables.chatId, variables.messageId],
      });
    },
  });
};

// Utility hook to get optimistic message count for a chat
export const useMessageCount = (chatId: number) => {
  const queryClient = useQueryClient();
  
  return queryClient.getQueryData(["messages", chatId, "infinite"]) || 
         queryClient.getQueryData(["messages", chatId, 1, 50]);
};