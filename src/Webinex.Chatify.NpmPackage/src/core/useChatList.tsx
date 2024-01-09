import { useClient } from '../ChatifyContext';
import { useQuery } from './useQuery';

export function useChatList() {
  const client = useClient();
  return useQuery({
    key: 'chat-list',
    queryFn: () => client.chats(),
  });
}
