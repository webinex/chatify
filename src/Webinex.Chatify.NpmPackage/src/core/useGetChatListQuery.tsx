import { useEffect, useMemo } from 'react';
import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useGetChatListQuery() {
  const stateRef = useStateRef();
  const client = useClient();
  useSelect((x) => x.query.chatList, []);
  const dispatch = useDispatch();

  const uninitialized = useMemo(() => {
    const uninitialized = stateRef.current.query.chatList.uninitialized !== false;
    uninitialized && dispatch({ type: 'chatList_fetch', data: void 0 });
    return uninitialized;

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (!uninitialized) {
      return;
    }

    client
      .chats()
      .then((chats) => dispatch({ type: 'chatList_fulfilled', data: { chats } }))
      .catch((error) => dispatch({ type: 'chatList_rejected', data: { error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return stateRef.current.query.chatList;
}
