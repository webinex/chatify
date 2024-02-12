import { useCallback } from 'react';
import { useClient } from '../ChatifyContext';
import { UpdateChatNameRequest } from './models';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useUpdateChatNameMutation() {
  const stateRef = useStateRef();
  useSelect((x) => x.mutation.updateChatName, []);
  const dispatch = useDispatch();
  const client = useClient();

  const invoke = useCallback((args: UpdateChatNameRequest) => {
    dispatch({ type: 'updateChatName_fetch', data: void 0 });
    client
      .updateChatName(args)
      .then(() => dispatch({ type: 'updateChatName_fulfilled', data: void 0 }))
      .catch((error) => dispatch({ type: 'updateChatName_rejected', data: { error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return [invoke, stateRef.current.mutation.updateChatName] as const;
}
