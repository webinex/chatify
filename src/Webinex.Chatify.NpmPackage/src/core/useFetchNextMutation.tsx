import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect, useStateRef } from './reducer';
import { useCallback } from 'react';

export function useFetchNextMutation({ chatId }: { chatId: string }) {
  const state = useStateRef();
  useSelect((x) => x.mutation.fetchNext, []);
  const dispatch = useDispatch();
  const client = useClient();

  const fetchNext = useCallback(() => {
    const skip = state.current.query.messages[chatId].data?.filter((x) => x.type === 'message').length;
    dispatch({ type: 'fetchNext_fetch', data: { chatId } });
    client
      .messages({ chatId, skip, take: 16 })
      .then((messages) =>
        dispatch({
          type: 'fetchNext_fulfilled',
          data: { chatId, messages: messages.slice(0, 15), hasMore: messages.length === 16 },
        }),
      )
      .catch((error) => dispatch({ type: 'fetchNext_rejected', data: { chatId, error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatId]);

  return [fetchNext, state.current.mutation.fetchNext] as const;
}
