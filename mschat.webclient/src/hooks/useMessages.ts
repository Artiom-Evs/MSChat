import { useQuery, useMutation, useQueryClient, useInfiniteQuery } from "@tanstack/react-query";
import { chatApi } from "../lib/chatApi";
import type { CreateMessageDto, UpdateMessageDto } from "../types";

export const useMessages = (chatId: number, limit: number = 50, offset?: number) => {
  return useQuery({
    queryKey: ["messages", chatId, limit, offset],
    queryFn: () => chatApi.getMessages(chatId, limit, offset),
    enabled: !!chatId,
  });
};

export const useInfiniteMessages = (chatId: number, limit: number = 50) => {
  return useInfiniteQuery({
    queryKey: ["messages", chatId, "infinite"],
    queryFn: ({ pageParam = 0 }) => chatApi.getMessages(chatId, limit, pageParam || undefined),
    initialPageParam: 0,
    getNextPageParam: (lastPage, allPages) => {
      // If the last page has fewer items than limit, we've reached the end
      if (lastPage.length < limit) {
        return undefined;
      }
      // Calculate offset for next page: total items fetched so far
      const totalItemsFetched = allPages.reduce((sum, page) => sum + page.length, 0);
      return totalItemsFetched;
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
         queryClient.getQueryData(["messages", chatId, 50, undefined]);
};