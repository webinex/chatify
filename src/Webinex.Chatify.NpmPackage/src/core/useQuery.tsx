import { useEffect } from 'react';
import { useDispatch, useSelect, useStateRef } from './reducer';
import { QueryState, State } from './state';

interface UseQueryArgs<TData> {
  key: string;
  queryFn: () => Promise<TData>;
}

const INITIAL_QUERY_STATE: QueryState<any> = {
  uninitialized: false,
  isFetching: true,
  data: undefined,
  error: undefined,
};

function select<TData>(state: State, key: string): QueryState<TData> | undefined {
  return (state.query as any)[key];
}

function useQueryState<TData>(key: string): QueryState<TData> | undefined {
  return useSelect((x) => select<TData>(x, key), []);
}

export function useQuery<TData>(args: UseQueryArgs<TData>): QueryState<TData> {
  const { key, queryFn } = args;
  const state = useStateRef();
  const dispatch = useDispatch();
  const queryState = useQueryState<TData>(args.key);

  useEffect(() => {
    if (select(state.current, key)) {
      return;
    }

    dispatch({ type: 'query_fetch', data: { key } });
    queryFn()
      .then((data) => dispatch({ type: 'query_resolve', data: { key, data: data as any } }))
      .catch((error) => dispatch({ type: 'query_reject', data: { key, error } }));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return queryState ?? INITIAL_QUERY_STATE;
}
