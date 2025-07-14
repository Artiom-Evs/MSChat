import { useQuery } from "@tanstack/react-query";
import { chatApi } from "../lib/chatApi";

export const useMembers = (search?: string) => {
  return useQuery({
    queryKey: ["members", search],
    queryFn: () => chatApi.getMembers(search),
  });
};

export const useMember = (memberId: string) => {
  return useQuery({
    queryKey: ["member", memberId],
    queryFn: () => chatApi.getMember(memberId),
    enabled: !!memberId,
  });
};

export const useCurrentMember = () => {
  return useQuery({
    queryKey: ["member", "me"],
    queryFn: chatApi.getCurrentMember,
  });
};
