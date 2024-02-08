import { useEffect, useMemo } from 'react';
import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useGetAccountsQuery() {
  const client = useClient();
  const stateRef = useStateRef();
  useSelect((x) => x.query.accounts, []);
  const dispatch = useDispatch();

  const uninitialized = useMemo(() => {
    const uninitialized = stateRef.current.query.accounts.uninitialized !== false;
    uninitialized && dispatch({ type: 'accounts_fetch', data: void 0 });
    return uninitialized;

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (!uninitialized) {
      return;
    }

    client
      .accounts()
      .then((accounts) => dispatch({ type: 'accounts_fulfilled', data: { accounts } }))
      .catch((error) => dispatch({ type: 'accounts_rejected', data: { error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [uninitialized]);

  return stateRef.current.query.accounts;
}
