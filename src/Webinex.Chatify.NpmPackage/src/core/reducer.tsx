import React, { PropsWithChildren, useCallback, useContext, useEffect, useRef, useState } from 'react';
import { ACTIONS, Action } from './action';
import { INITIAL_STATE, State } from './state';
import { shallowEqual } from 'shallow-equal';

function reducer(state: State, action: Action) {
  const result = ACTIONS[action.type](state, action.data as any);
  console.debug('[Chatify]: ', action, result);
  return result;
}

export type Reducer = ReturnType<typeof useReducer>;

type Subscriber = (state: State) => void;

export function useReducer() {
  const state = useRef(INITIAL_STATE);
  const subscribers = useRef<Subscriber[]>([]);

  const dispatch = useCallback((action: Action) => {
    const newState = reducer(state.current, action);
    if (state.current === newState) {
      return;
    }

    state.current = newState;
    subscribers.current.forEach((s) => s(newState));
  }, []);

  const subscribe = useCallback((subscriber: Subscriber) => {
    subscribers.current.push(subscriber);
    return () => {
      subscribers.current = subscribers.current.filter((x) => x !== subscriber);
    };
  }, []);

  return [state, dispatch, subscribe] as const;
}

const ReducerContext = React.createContext<Reducer>(null!);
export function ReducerProvider({ children }: PropsWithChildren<{}>) {
  const reducer = useReducer();
  return <ReducerContext.Provider value={reducer}>{children}</ReducerContext.Provider>;
}

export function useDispatch() {
  return useContext(ReducerContext)[1];
}

export function useStateRef() {
  const [reducerState] = useContext(ReducerContext);
  return reducerState;
}

export function useSelect<TResult>(
  select: (state: State) => TResult,

  // TODO: make deps work
  deps: any[],
): TResult {
  const [reducerState, , subscribe] = useContext(ReducerContext);
  const prev = useRef(select(reducerState.current));
  const [state, setState] = useState(prev.current);

  useEffect(() => {
    const value = select(reducerState.current);
    if (!shallowEqual<any>(prev.current, value)) {
      prev.current = value;
      setState(value);
    }

    return subscribe((state: State) => {
      const newValue = select(state);
      if (shallowEqual<any>(newValue, prev.current)) {
        return;
      }

      prev.current = newValue;
      setState(newValue);
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return state;
}
