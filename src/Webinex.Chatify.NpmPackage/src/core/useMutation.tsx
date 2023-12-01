import { useCallback } from 'react';
import { useDispatch, useSelect, useStateRef } from './reducer';
import { MutationState, State } from './state';

export interface MutationApi<TResult> {
  promise: Promise<TResult>;
  state: State;
  updateState: (mutation: (state: State) => State) => void;
}

export interface UseMutationArgs<TResult = unknown, TArgs = void> {
  key: string;
  queryFn: (args: TArgs) => Promise<TResult>;
  onQueryStarted?: (args: TArgs, api: MutationApi<TResult>) => void;
}

const INITIAL_MUTATION_STATE: MutationState = { isFetching: false, error: undefined };

export function useMutation<TResult = unknown, TArgs = void>(args: UseMutationArgs<TResult, TArgs>) {
  const { key, queryFn, onQueryStarted } = args;
  const dispatch = useDispatch();
  const state = useStateRef();
  const mutationState = useSelect((x) => x.mutation[args.key], []);

  const invoke = useCallback((args: TArgs) => {
    dispatch({ type: 'mutation_fetch', data: { key } });
    const promise = queryFn(args);

    promise
      .then(() => {
        dispatch({ type: 'mutation_resolve', data: { key } });
      })
      .catch((error) => dispatch({ type: 'mutation_reject', data: { key, error } }));

    onQueryStarted &&
      onQueryStarted(args, {
        promise,
        state: state.current,
        updateState: (mutation: (state: State) => State) => {
          dispatch({ type: 'update', data: { mutation } });
        },
      });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return [invoke, mutationState ?? INITIAL_MUTATION_STATE] as const;
}
