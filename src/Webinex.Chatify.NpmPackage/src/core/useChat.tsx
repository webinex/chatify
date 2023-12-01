import { useClient } from '../ChatifyContext';
import { useQuery } from './useQuery';

export function useChat(args: { chatId: string }) {
  const { chatId } = args;
  const client = useClient();

  return useQuery({
    key: 'chat.' + chatId,
    queryFn: () => client.chat(chatId),
  });
}
