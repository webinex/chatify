import { Account, Chat, ChatListItem, Message, SendMessageRequest, File } from './models';

export type MessageState =
  | ({ type: 'message' } & Message)
  | { type: 'next' }
  | { type: 'sending'; text: string; files: File[]; requestId: string };

export interface UIState {
  view?: 'chat' | 'new-chat';
  showMembers: boolean;
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

export interface QueueState<TValue> {
  queued: TValue[];
  processing: TValue[];
  timestamp: number;
}

export interface State {
  ui: UIState;
  query: {
    messages: Record<string, QueryState<MessageState[]>>;
    chats: Record<string, QueryState<Chat>>;
    chatList: QueryState<ChatListItem[]>;
    accounts: QueryState<Account[]>;
  };
  mutation: {
    fetchNext: MutationState;
    removeMember: MutationState;
    addMember: MutationState;
    updateChatName: MutationState;
    addChat: MutationState;
    send: MutationState;
    updateAccount: MutationState;
  };
  queue: {
    read: QueueState<string>;
    send: QueueState<SendMessageRequest>;
  };
}

export const INITIAL_QUERY_STATE: QueryState<any> = {
  uninitialized: false,
  isFetching: true,
  data: undefined,
  error: undefined,
};

export const INITIAL_STATE: State = {
  ui: {
    showMembers: false,
  },
  query: {
    messages: {},
    chats: {},
    chatList: { uninitialized: true, isFetching: false, data: undefined, error: undefined },
    accounts: { uninitialized: true, isFetching: false, data: undefined, error: undefined },
  },
  mutation: {
    fetchNext: { isFetching: false, error: undefined },
    removeMember: { isFetching: false, error: undefined },
    addMember: { isFetching: false, error: undefined },
    updateChatName: { isFetching: false, error: undefined },
    addChat: { isFetching: false, error: undefined },
    send: { isFetching: false, error: undefined },
    updateAccount: { isFetching: false, error: undefined },
  },
  queue: {
    read: {
      queued: [],
      processing: [],
      timestamp: 0,
    },

    send: {
      queued: [],
      processing: [],
      timestamp: 0,
    },
  },
};
