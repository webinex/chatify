import { useClient } from '../ChatifyContext';
import { File, Message } from './models';
import { QueryState } from './state';
import { useQuery } from './useQuery';

export type MessageState =
  | ({ type: 'message' } & Message)
  | { type: 'more' }
  | { type: 'sending'; text: string; files: File[]; requestId: string };

export function useMessages(args: { chatId: string }): QueryState<MessageState[]> {
  const { chatId } = args;
  const client = useClient();

  return useQuery({
    key: 'messages.' + chatId,
    queryFn: () =>
      client.messages({ chatId, skip: 0, take: 15 }).then((x) => {
        const result = x.map((message) => ({ type: 'message' as const, ...message, proceed: true }));
        return x.length === 15 ? [...result, { type: 'more' as const }] : result;
      }),
  });
}
