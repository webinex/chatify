import { useCallback } from 'react';
import { useClient } from '../ChatifyContext';
import { RemoveMemberRequest } from './models';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useRemoveMemberMutation() {
  const stateRef = useStateRef();
  useSelect((x) => x.mutation.removeMember, []);
  const dispatch = useDispatch();
  const client = useClient();

  const invoke = useCallback((args: RemoveMemberRequest) => {
    dispatch({ type: 'removeMember_fetch', data: void 0 });
    client
      .removeMember(args)
      .then(() => dispatch({ type: 'removeMember_fulfilled', data: void 0 }))
      .catch((error) => dispatch({ type: 'removeMember_rejected', data: { error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return [invoke, stateRef.current.mutation.removeMember] as const;
}
