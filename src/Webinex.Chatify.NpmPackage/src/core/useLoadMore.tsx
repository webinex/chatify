import { useClient } from '../ChatifyContext';
import { useMutation } from './useMutation';
import { MessageState } from './useMessages';
import { useStateRef } from './reducer';
import { QueryState } from './state';

export function useLoadMore({ chatId }: { chatId: string }) {
  const state = useStateRef();
  const client = useClient();

  return useMutation({
    key: 'more.' + chatId,
    queryFn: () =>
      client.messages({
        chatId,
        skip: state.current.query['messages.' + chatId].data.filter((x: MessageState) => x.type === 'message')
          .length,
        take: 15,
      }),

    onQueryStarted: async (_, { promise, updateState }) => {
      const result = await promise;
      updateState((state) => {
        state = { ...state };
        state.query = { ...state.query };
        state.query['messages.' + chatId] = { ...state.query['messages.' + chatId] };
        const value: QueryState<MessageState[]> = state.query['messages.' + chatId];
        value.data = value.data?.filter((x) => x.type !== 'more');
        value.data = [
          ...value.data!,
          ...result.map((x) => ({ type: 'message' as const, ...x, proceed: true })),
        ];

        if (result.length === 15) {
          value.data = [...value.data!, { type: 'more' }];
        }

        return state;
      });
    },
  });
}
