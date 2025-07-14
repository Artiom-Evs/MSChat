import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { chatApi } from "../lib/chatApi";
import type { AddParticipantDto, UpdateParticipantRoleDto } from "../types";

export const useParticipants = (chatId: number) => {
  return useQuery({
    queryKey: ["participants", chatId],
    queryFn: () => chatApi.getChatParticipants(chatId),
    enabled: !!chatId,
  });
};

export const useAddParticipant = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      participant,
    }: {
      chatId: number;
      participant: AddParticipantDto;
    }) => chatApi.addParticipant(chatId, participant),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["participants", variables.chatId],
      });
    },
  });
};

export const useUpdateParticipantRole = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      participantMemberId,
      roleUpdate,
    }: {
      chatId: number;
      participantMemberId: string;
      roleUpdate: UpdateParticipantRoleDto;
    }) =>
      chatApi.updateParticipantRole(chatId, participantMemberId, roleUpdate),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["participants", variables.chatId],
      });
    },
  });
};

export const useRemoveParticipant = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      chatId,
      participantMemberId,
    }: {
      chatId: number;
      participantMemberId: string;
    }) => chatApi.removeParticipant(chatId, participantMemberId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["participants", variables.chatId],
      });
    },
  });
};

export const useJoinChat = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: chatApi.joinChat,
    onSuccess: (_, chatId) => {
      queryClient.invalidateQueries({ queryKey: ["participants", chatId] });
      queryClient.invalidateQueries({ queryKey: ["chats"] });
      queryClient.invalidateQueries({ queryKey: ["chat", chatId] });
    },
  });
};

export const useLeaveChat = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: chatApi.leaveChat,
    onSuccess: (_, chatId) => {
      queryClient.invalidateQueries({ queryKey: ["participants", chatId] });
      queryClient.invalidateQueries({ queryKey: ["chats"] });
      queryClient.invalidateQueries({ queryKey: ["chat", chatId] });
    },
  });
};
