import { useCallback } from 'react';
import { useClient } from '../ChatifyContext';
import { AddChatRequest } from './models';
import { useDispatch, useStateRef } from './reducer';

export function useAddChatMutation() {
  const client = useClient();
  const dispatch = useDispatch();
  const stateRef = useStateRef();

  const invoke = useCallback((args: AddChatRequest) => {
    dispatch({ type: 'addChat_fetch', data: void 0 });
    client
      .addChat(args)
      .then(() => dispatch({ type: 'addChat_fulfilled', data: void 0 }))
      .catch((error) => dispatch({ type: 'addChat_rejected', data: { error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return [invoke, stateRef.current.mutation.addChat] as const;
}
