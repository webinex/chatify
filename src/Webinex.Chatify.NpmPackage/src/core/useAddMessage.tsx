import { useClient } from '../ChatifyContext';
import { SendMessageRequest } from './models';
import { QueryState } from './state';
import { MessageState } from './useMessages';
import { useMutation } from './useMutation';

export interface UseAddMessageArgs {
  chatId: string;
}

export function useAddMessage(args: UseAddMessageArgs) {
  const { chatId } = args;
  const client = useClient();

  return useMutation<unknown, Omit<SendMessageRequest, 'chatId'>>({
    key: 'add.' + chatId,
    queryFn: (args) => client.send({ chatId, ...args }),
    onQueryStarted: async (args, { updateState }) => {
      updateState((state) => {
        state = {
          ...state,
          query: {
            ...state.query,
            ['messages.' + chatId]: {
              ...state.query['messages.' + chatId],
            },
          },
        };

        const queryState: QueryState<MessageState[]> = state.query['messages.' + chatId];
        queryState.data = [
          { type: 'sending', text: args.text, requestId: args.requestId, files: args.files },
          ...queryState.data!,
        ];

        return state;
      });
    },
  });
}
