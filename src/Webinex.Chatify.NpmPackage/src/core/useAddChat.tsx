import { useClient } from '../ChatifyContext';
import { AddChatRequest } from './models';
import { useMutation } from './useMutation';

export function useAddChat() {
  const client = useClient();

  return useMutation({
    key: 'add-chat',
    queryFn: (args: AddChatRequest) => client.addChat(args),
  });
}
