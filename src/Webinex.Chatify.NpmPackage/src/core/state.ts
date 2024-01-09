export interface UIState {
  view?: 'chat' | 'new-chat' | 'chat-settings';
  chatId?: string;
}

export interface QueryState<TData> {
  uninitialized: boolean;
  isFetching: boolean;
  error?: any;
  data?: TData;
}

export interface MutationState {
  isFetching: boolean;
  error?: any;
}

export interface State {
  ui: UIState;
  query: Record<string, QueryState<any>>;
  mutation: Record<string, MutationState>;
  read: {
    queued: string[];
    sent: string[];
    timestamp: number;
  };
}

export const INITIAL_STATE: State = {
  ui: {},
  query: {},
  mutation: {},
  read: {
    queued: [],
    sent: [],
    timestamp: 0,
  },
};
