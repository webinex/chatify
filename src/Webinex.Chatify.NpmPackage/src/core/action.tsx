import type { MutationState, QueryState, State } from './state';
import { Account, ChatListItem, Message, ReadEvent } from './models';
import { MessageState } from './useMessages';

function updateQuery<TData>(
  state: State,
  key: string,
  mutation: (queryState: QueryState<TData> | undefined) => QueryState<TData> | undefined,
) {
  state = {
    ...state,
    query: {
      ...state.query,
    },
  };

  const result = mutation(state.query[key]);
  if (result) {
    state.query[key] = result;
  }
  return state;
}

function updateMutation(
  state: State,
  key: string,
  mutation: (mutationState: MutationState | undefined) => MutationState | undefined,
) {
  state = {
    ...state,
    mutation: {
      ...state.mutation,
    },
  };

  const result = mutation(state.mutation[key]);
  if (result) {
    state.mutation[key] = result;
  }
  return state;
}

export const ACTIONS = {
  query_fetch: (state: State, { key }: { key: string }): State => {
    return updateQuery(state, key, (x) => ({ ...x, isFetching: true, uninitialized: false }));
  },

  query_resolve: (state: State, { key, data }: { key: string; data: any[] }): State => {
    return updateQuery(state, key, (x) => ({ ...x!, isFetching: false, error: undefined, data }));
  },

  query_reject: (state: State, { key, error }: { key: string; error: any }): State => {
    return updateQuery(state, key, (x) => ({ ...x!, isFetching: false, error }));
  },

  mutation_fetch: (state: State, { key }: { key: string }): State => {
    return updateMutation(state, key, (x) => ({ ...x, isFetching: true }));
  },

  mutation_resolve: (state: State, { key }: { key: string }): State => {
    return updateMutation(state, key, (x) => ({ ...x, isFetching: false, error: undefined }));
  },

  mutation_reject: (state: State, { key, error }: { key: string; error: any }): State => {
    return updateMutation(state, key, (x) => ({ ...x, isFetching: false, error }));
  },

  update: (state: State, { mutation }: { mutation: (state: State) => State }): State => {
    return mutation(state);
  },

  chat_open: (state: State, { chatId }: { chatId: string }): State => {
    return { ...state, ui: { ...state.ui, view: 'chat', chatId } };
  },

  new_chat_open: (state: State): State => {
    return { ...state, ui: { ...state.ui, view: 'new-chat', chatId: undefined } };
  },

  chat_settings_open: (state: State, { chatId }: { chatId: string }): State => {
    return { ...state, ui: { ...state.ui, view: 'chat-settings', chatId } };
  },

  message_received: (
    state: State,
    { message, requestId }: { message: Message; requestId: string },
  ): State => {
    if (state.query['messages.' + message.chatId]?.data !== undefined) {
      state = {
        ...state,
        query: {
          ...state.query,
          ['messages.' + message.chatId]: {
            ...state.query['messages.' + message.chatId],
            data: [{ type: 'message', ...message }, ...state.query['messages.' + message.chatId].data].filter(
              (x: MessageState) => x.type !== 'sending' || x.requestId !== requestId,
            ),
          },
        },
      };
    }

    if (state.query['chat-list'].data !== undefined) {
      state = {
        ...state,
        query: {
          ...state.query,
          'chat-list': {
            ...state.query['chat-list'],
            data: state.query['chat-list'].data.map((x: ChatListItem) =>
              x.id === message.chatId
                ? { ...x, message, unreadCount: message.read ? x.unreadCount : x.unreadCount + 1 }
                : x,
            ),
          },
        },
      };
    }

    return state;
  },

  chat_created: (state: State, { chat }: { chat: ChatListItem; requestId?: string }): State => {
    if (state.query['chat-list']?.data === undefined) {
      return state;
    }

    return {
      ...state,
      query: {
        ...state.query,
        'chat-list': {
          ...state.query['chat-list'],
          data: [chat, ...state.query['chat-list'].data!],
        },
      },
    };
  },

  read: (state: State, { id, timestamp }: { id: string; timestamp: number }): State => {
    return {
      ...state,
      read: {
        ...state.read,
        queued: [...state.read.queued, id],
        timestamp,
      },
    };
  },

  read_send: (state: State, { ids }: { ids: string[] }): State => {
    return {
      ...state,
      read: {
        ...state.read,
        queued: state.read.queued.filter((x) => !ids.includes(x)),
        sent: [...state.read.sent, ...ids],
      },
    };
  },

  read_reject: (state: State, { ids, timestamp }: { ids: string[]; timestamp: number }): State => {
    return {
      ...state,
      read: {
        ...state.read,
        queued: [...state.read.queued, ...ids],
        sent: state.read.sent.filter((x) => !ids.includes(x)),
        timestamp,
      },
    };
  },

  read_received: (state: State, { events }: { events: ReadEvent[] }): State => {
    const ids = events.map((x) => x.messageId);

    state = {
      ...state,
      query: {
        ...state.query,
      },
      read: {
        ...state.read,
        sent: state.read.sent.filter((x) => !ids.includes(x)),
      },
    };

    Object.keys(state.query)
      .filter((x) => x.startsWith('messages.'))
      .forEach((key) => {
        state.query[key] = { ...state.query[key] };
        const queryState: QueryState<MessageState[]> = state.query[key];

        queryState.data = queryState.data?.map((x) => {
          if (x.type === 'message' && ids.includes(x.id)) {
            return { ...x, read: true };
          }

          return x;
        });
      });

    state.query['chat-list'] = { ...state.query['chat-list'] };
    const chatListState: QueryState<ChatListItem[]> = state.query['chat-list'];
    chatListState.data = chatListState.data?.map((x) =>
      ids.includes(x.message.id) ? { ...x, message: { ...x.message, read: true } } : x,
    );

    chatListState.data = chatListState.data?.map((chat) => ({
      ...chat,
      unreadCount: chat.unreadCount - events.filter((x) => x.chatId === chat.id).length,
    }));

    return state;
  },

  member_added: (
    state: State,
    {
      chatId,
      chatName,
      account,
      me,
      message,
    }: { chatId: string; chatName: string; account: Account; me: string; message: Message },
  ): State => {
    state = {
      ...state,
      query: {
        ...state.query,
      },
    };

    if (state.query['chat.' + chatId]?.data !== undefined) {
      state.query['chat.' + chatId] = {
        ...state.query['chat.' + chatId],
        data: { ...state.query['chat.' + chatId].data },
      };
      state.query['chat.' + chatId].data.members = [...state.query['chat.' + chatId].data.members, account];
    }

    if (state.query['messages.' + chatId]?.data !== undefined) {
      state.query['messages.' + chatId] = {
        ...state.query['messages.' + chatId],
        data: [{ type: 'message', ...message }, ...state.query['messages.' + chatId].data],
      };
    }

    if (account.id !== me && state.query['chat-list'].data !== undefined) {
      state.query['chat-list'] = { ...state.query['chat-list'] };
      state.query['chat-list'].data = state.query['chat-list'].data.map((x: ChatListItem) =>
        x.id === chatId ? { ...x, message: { ...message } } : x,
      );
    }

    if (account.id === me && state.query['chat-list'].data !== undefined) {
      const listItem: ChatListItem = {
        id: chatId,
        name: chatName,
        message: { ...message },
        unreadCount: 0,
      };

      state.query['chat-list'] = {
        ...state.query['chat-list'],
        data: [...state.query['chat-list'].data, listItem],
      };
    }

    return state;
  },

  member_removed: (
    state: State,
    {
      chatId,
      accountId,
      deleteHistory,
      me,
    }: { chatId: string; accountId: string; deleteHistory: boolean; me: string },
  ): State => {
    state = {
      ...state,
      query: {
        ...state.query,
      },
    };

    if (state.query['chat.' + chatId]?.data !== undefined) {
      state.query['chat.' + chatId] = {
        ...state.query['chat.' + chatId],
        data: { ...state.query['chat.' + chatId].data },
      };
      state.query['chat.' + chatId].data.members = state.query['chat.' + chatId].data.members.filter(
        (x: Account) => x.id !== accountId,
      );
    }

    if (accountId === me && deleteHistory && state.query['chat-list']?.data) {
      state.query['chat-list'] = { ...state.query['chat-list'] };
      state.query['chat-list'].data = state.query['chat-list'].data.filter(
        (x: ChatListItem) => x.id !== chatId,
      );
    }

    if (accountId === me && deleteHistory && state.query['chat-list']?.data && state.ui.chatId === chatId) {
      state.ui = { ...state.ui, chatId: state.query['chat-list']?.data.at(0)?.id };
    }

    return state;
  },
};

export type Action = {
  [K in keyof typeof ACTIONS]: {
    type: K;
    data: Parameters<(typeof ACTIONS)[K]>[1];
  };
}[keyof typeof ACTIONS];
