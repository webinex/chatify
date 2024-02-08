import { useEffect, useMemo } from 'react';
import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useGetChatQuery(args: { chatId: string }) {
  const { chatId } = args;
  const client = useClient();
  const stateRef = useStateRef();
  useSelect((x) => x.query.chats[chatId], [chatId]);
  const dispatch = useDispatch();

  const uninitialized = useMemo(() => {
    const uninitialized = stateRef.current.query.chats[chatId]?.uninitialized !== false;
    uninitialized && dispatch({ type: 'chat_fetch', data: { chatId } });
    return uninitialized;

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatId]);

  useEffect(() => {
    if (!uninitialized) {
      return;
    }

    client
      .chat(chatId)
      .then((chat) => dispatch({ type: 'chat_fulfilled', data: { chat } }))
      .catch((error) => dispatch({ type: 'chat_rejected', data: { chatId, error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [uninitialized, chatId]);

  return stateRef.current.query.chats[chatId];
}
