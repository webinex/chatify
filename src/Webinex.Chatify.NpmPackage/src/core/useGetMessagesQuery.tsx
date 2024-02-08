import { useEffect, useMemo } from 'react';
import { useClient } from '../ChatifyContext';
import { MessageState, QueryState } from './state';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useGetMessagesQuery(args: { chatId: string }): QueryState<MessageState[]> {
  const { chatId } = args;
  const stateRef = useStateRef();
  const dispatch = useDispatch();
  useSelect((x) => x.query.messages[chatId], [chatId]);
  const client = useClient();

  const uninitialized = useMemo(() => {
    const uninitialized = stateRef.current.query.messages[chatId]?.uninitialized !== false;
    uninitialized && dispatch({ type: 'messages_fetch', data: { chatId } });
    return uninitialized;
  }, [chatId, dispatch, stateRef]);

  useEffect(() => {
    if (!uninitialized) {
      return;
    }

    client.messages({ chatId, skip: 0, take: 16 }).then((messages) =>
      dispatch({
        type: 'messages_fulfilled',
        data: { chatId, messages: messages.slice(0, 15), hasMore: messages.length === 16 },
      }),
    );

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatId]);

  return stateRef.current.query.messages[chatId];
}
