import { useCallback } from 'react';
import { useClient } from '../ChatifyContext';
import { AddMemberRequest } from './models';
import { useDispatch, useSelect, useStateRef } from './reducer';

export function useAddMemberMutation() {
  const client = useClient();
  const dispatch = useDispatch();
  const state = useStateRef();
  useSelect((x) => x.mutation.addMember, []);

  const invoke = useCallback(
    (args: AddMemberRequest) => {
      dispatch({ type: 'addMember_fetch', data: void 0 });
      client
        .addMember(args)
        .then(() => dispatch({ type: 'addMember_fulfilled', data: void 0 }))
        .catch((error) => dispatch({ type: 'addMember_rejected', data: { error } }));
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [dispatch],
  );

  return [invoke, state.current.mutation.addMember] as const;
}
