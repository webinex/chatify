import { useCallback } from 'react';
import { useClient } from '../ChatifyContext';
import { SendMessageRequest } from './models';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useSendMutation() {
  const stateRef = useStateRef();
  useSelect((x) => x.mutation.send, []);
  const dispatch = useDispatch();
  const client = useClient();

  const invoke = useCallback((args: SendMessageRequest) => {
    const { chatId, requestId, files, text } = args;
    dispatch({ type: 'send_fetch', data: { chatId, requestId, files, text } });
    client
      .send(args)
      .then(() => dispatch({ type: 'send_fulfilled', data: { chatId, requestId } }))
      .catch((error) => dispatch({ type: 'send_rejected', data: { chatId, requestId, error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return [invoke, stateRef.current.mutation.send] as const;
}
