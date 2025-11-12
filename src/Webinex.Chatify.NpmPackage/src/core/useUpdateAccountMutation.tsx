import { useCallback } from 'react';
import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect, useStateRef } from './reducer';
import { UpdateAccountRequest } from './models';

export function useUpdateAccountMutation() {
  const stateRef = useStateRef();
  useSelect((x) => x.mutation.updateAccount, []);
  const dispatch = useDispatch();
  const client = useClient();

  const invoke = useCallback((args: UpdateAccountRequest) => {
    dispatch({ type: 'updateAccount_fetch', data: void 0 });
    client
      .updateAccount(args)
      .then(() => dispatch({ type: 'updateAccount_fulfilled', data: void 0 }))
      .catch((error) => dispatch({ type: 'updateAccount_rejected', data: { error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return [invoke, stateRef.current.mutation.updateAccount] as const;
}
