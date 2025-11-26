import { DEFAULT_MESSAGE_LIST_TAKE, __baseApi, __settings } from './baseApi';
import { useSelector } from 'react-redux';
import {
  Account,
  AddChatMemberRequest,
  Chat,
  ChatListItem,
  ChatMessage,
  RemoveChatMemberRequest,
  SendChatMessageRequest,
  SendThreadMessageRequest,
  Thread,
  ThreadMessage,
  ThreadWatchListItem,
  UpdateAccountRequest,
  calcThreadListUnreadCount,
  chatId,
  parseThreadMessageId,
} from '../types';
import { unsubscribe } from '../client';
import { Debounce, DebounceValue } from '../debounce';
import { useMemo } from 'react';

export interface ChatMessageListResult {
  messages: ChatMessage[];
  hasMore: boolean;
}

export interface ThreadMessageList {
  messages: ThreadMessage[];
  hasMore: boolean;
}

const THREAD_MESSAGE_READ_QUEUE = new Debounce<string>({
  callback: (id) => __settings.client.readThreadMessage(id),
  compare: (a, b) => -a.localeCompare(b),
  groupBy: (id) => parseThreadMessageId(id)[0],
  timeout: 500,
});

const CHAT_MESSAGE_READ_QUEUE = new Debounce<string>({
  callback: (id) => __settings.client.readChatMessage({ id }),
  compare: (a, b) => -a.localeCompare(b),
  groupBy: (id) => chatId(id),
  timeout: 500,
});

const __chatifyApi = __baseApi.injectEndpoints({
  endpoints: (builder) => ({
    // ====================================================
    // ==============     ACCOUNT    ======================
    // ====================================================

    getAccountList: builder.query<Account[], { ids?: string[] }>({
      queryFn: async (args) => ({ data: await __settings.client.accounts(args) }),
      providesTags: ['account'],
    }),

    updateAccount: builder.mutation<void, UpdateAccountRequest>({
      queryFn: async (request) => {
        await __settings.client.updateAccount(request);
        return { data: null! };
      },
      invalidatesTags: ['account'],
    }),

    // ====================================================
    // ================     CHAT    =======================
    // ====================================================

    getChat: builder.query<Chat, { id: string }>({
      queryFn: async ({ id }) => ({ data: await __settings.client.chat(id) }),
      providesTags: ['chat', 'account'],
      onCacheEntryAdded: async ({ id }, { cacheEntryRemoved, cacheDataLoaded, updateCachedData }) => {
        await cacheDataLoaded;

        unsubscribe(
          __settings.client.subscribe(
            'chatify://chat-member-added',
            ([chatId, _, account, message, , read]) =>
              chatId === id &&
              updateCachedData((chat) => {
                chat.members.push(account);
                chat.lastReadMessageId = read ? message.id : chat.lastReadMessageId;
              }),
          ),
          __settings.client.subscribe(
            'chatify://chat-member-removed',
            ([chatId, accountId, , message, read]) =>
              id === chatId &&
              updateCachedData((chat) => {
                chat.members = chat.members.filter((x) => x.id !== accountId);
                chat.lastReadMessageId = read ? message.id : chat.lastReadMessageId;
              }),
          ),
          __settings.client.subscribe(
            'chatify://chat-name-changed',
            ([chatId, newName, message, read]) =>
              id === chatId &&
              updateCachedData((chat) => {
                chat.name = newName;
                chat.lastReadMessageId = read ? message.id : chat.lastReadMessageId;
              }),
          ),
          __settings.client.subscribe(
            'chatify://chat-new-message',
            ([message]) =>
              id === message.chatId &&
              updateCachedData((chat) => {
                chat.lastReadMessageId =
                  message.sentBy.id === __settings.me() ? message.id : chat.lastReadMessageId;
              }),
          ),
          __settings.client.subscribe(
            'chatify://chat-message-read',
            ([events]) =>
              events.find((x) => x.chatId === id) &&
              updateCachedData((chat) => {
                const event = events.find((x) => x.chatId === id)!;
                chat.lastReadMessageId = event.newLastReadMessageId;
              }),
          ),
        ).when(cacheEntryRemoved);
      },
    }),

    addChat: builder.mutation<string, { name: string; members: string[] }>({
      queryFn: async ({ name, members }) => {
        const chatId = await __settings.client.addChat({
          name,
          members,
        });
        return { data: chatId };
      },
    }),

    updateChatName: builder.mutation<void, { id: string; name: string }>({
      queryFn: async ({ id, name }) => {
        await __settings.client.updateChatName({ id, name });
        return { data: null! };
      },
    }),

    // ====================================================
    // =============     CHAT LIST    =====================
    // ====================================================

    getChatList: builder.query<ChatListItem[], void>({
      queryFn: async () => ({ data: await __settings.client.chats() }),
      providesTags: ['chat', 'account'],
      onCacheEntryAdded: async (_, { cacheEntryRemoved, cacheDataLoaded, updateCachedData }) => {
        await cacheDataLoaded;

        unsubscribe(
          __settings.client.subscribe('chatify://chat-created', ([chat]) =>
            updateCachedData((chatList) => {
              chatList.push(chat);
            }),
          ),

          __settings.client.subscribe('chatify://chat-new-message', ([message]) =>
            updateCachedData((chatList) => {
              const chat = chatList.find((x) => x.id === message.chatId);

              if (!chat) {
                return;
              }

              chat.message = message;

              if (message.sentBy.id === __settings.me()) {
                chat.lastReadMessageId = message.id;
                chat.totalUnreadCount = 0;
              }

              if (message.sentBy.id !== __settings.me()) {
                chat.totalUnreadCount++;
              }
            }),
          ),
          __settings.client.subscribe('chatify://chat-message-read', ([events]) =>
            updateCachedData((chatList) => {
              for (const event of events) {
                const chat = chatList.find((x) => x.id === event.chatId);

                if (!chat) {
                  return;
                }

                chat.lastReadMessageId = event.newLastReadMessageId;
                chat.totalUnreadCount = chat.totalUnreadCount - event.readCount;
              }
            }),
          ),
          __settings.client.subscribe(
            'chatify://chat-member-added',
            ([chatId, chatName, account, message, , read]) =>
              updateCachedData((chatList) => {
                const chat = chatList.find((x) => x.id === chatId);

                if (account.id === __settings.me() && !chat) {
                  chatList.push({
                    id: chatId,
                    name: chatName,
                    message,
                    totalUnreadCount: read ? 0 : 1,
                    active: true,
                    lastReadMessageId: read ? message.id : null,
                  });
                }

                if (!chat) {
                  return;
                }

                chat.message = message;
                chat.active = true;

                if (read) {
                  chat.totalUnreadCount = 0;
                  chat.lastReadMessageId = message.id;
                } else {
                  chat.totalUnreadCount++;
                }
              }),
          ),
          __settings.client.subscribe(
            'chatify://chat-member-removed',
            ([chatId, accountId, deleteHistory, message, read]) =>
              updateCachedData((chatList) => {
                if (accountId === __settings.me() && deleteHistory) {
                  return chatList.filter((x) => x.id !== chatId);
                }

                const chat = chatList.find((x) => x.id === chatId);

                if (!chat) {
                  return;
                }

                chat.message = message;

                if (accountId === __settings.me()) {
                  chat.active = false;
                }

                if (!read) {
                  chat.totalUnreadCount++;
                } else {
                  chat.totalUnreadCount = 0;
                  chat.lastReadMessageId = message.id;
                }
              }),
          ),
          __settings.client.subscribe('chatify://chat-name-changed', ([chatId, newName, message, read]) =>
            updateCachedData((chatList) => {
              const chat = chatList.find((x) => x.id === chatId);
              if (!chat) return;

              chat.name = newName;
              chat.message = message;

              if (!read) {
                chat.totalUnreadCount++;
              } else {
                chat.totalUnreadCount = 0;
                chat.lastReadMessageId = message.id;
              }
            }),
          ),
        ).when(cacheEntryRemoved);
      },
    }),

    // ====================================================
    // ============     CHAT MEMBER    ====================
    // ====================================================

    removeChatMember: builder.mutation<void, RemoveChatMemberRequest>({
      queryFn: async (args) => {
        await __settings.client.removeChatMember(args);
        return { data: null! };
      },
    }),

    addChatMember: builder.mutation<void, AddChatMemberRequest>({
      queryFn: async (args) => {
        await __settings.client.addChatMember(args);
        return { data: null! };
      },
    }),

    // ====================================================
    // ============     CHAT MESSAGE    ===================
    // ====================================================

    getChatMessageList: builder.query<ChatMessageListResult, { chatId: string }>({
      queryFn: async ({ chatId }) => {
        const messages = await __settings.client.chatMessages({
          chatId,
          skip: 0,
          take: DEFAULT_MESSAGE_LIST_TAKE + 1,
        });
        return {
          data: {
            messages: messages.slice(0, DEFAULT_MESSAGE_LIST_TAKE),
            hasMore: messages.length === DEFAULT_MESSAGE_LIST_TAKE + 1,
          },
        };
      },
      providesTags: ['message', 'chat', 'account'],
      onCacheEntryAdded: async ({ chatId }, { dispatch, cacheEntryRemoved, cacheDataLoaded }) => {
        await cacheDataLoaded;

        unsubscribe(
          __settings.client.subscribe('chatify://chat-new-message', ([message]) => {
            message.chatId === chatId &&
              dispatch(
                chatifyApi.util.updateQueryData('getChatMessageList', { chatId }, (draft) => {
                  draft.messages.unshift(message);
                }),
              );
          }),

          __settings.client.subscribe('chatify://chat-member-added', ([eventChatId, , , message]) => {
            eventChatId === chatId &&
              dispatch(
                chatifyApi.util.updateQueryData('getChatMessageList', { chatId }, (draft) => {
                  draft.messages.unshift(message);
                }),
              );
          }),

          __settings.client.subscribe('chatify://chat-member-removed', ([eventChatId, , , message]) => {
            eventChatId === chatId &&
              dispatch(
                chatifyApi.util.updateQueryData('getChatMessageList', { chatId }, (draft) => {
                  draft.messages.unshift(message);
                }),
              );
          }),

          __settings.client.subscribe('chatify://chat-name-changed', ([eventChatId, , message]) => {
            eventChatId === chatId &&
              dispatch(
                chatifyApi.util.updateQueryData('getChatMessageList', { chatId }, (draft) => {
                  draft.messages.unshift(message);
                }),
              );
          }),
        ).when(cacheEntryRemoved);
      },
    }),

    fetchNextChatMessageList: builder.mutation<void, { chatId: string }>({
      queryFn: async ({ chatId }, { getState }) => {
        const current = chatifyApi.endpoints.getChatMessageList.select({ chatId })(getState() as any);
        const messages = await __settings.client.chatMessages({
          chatId,
          skip: current.data!.messages.length,
          take: DEFAULT_MESSAGE_LIST_TAKE + 1,
        });
        return {
          data: null!,
          meta: {
            messages: messages.slice(0, DEFAULT_MESSAGE_LIST_TAKE),
            hasMore: messages.length === DEFAULT_MESSAGE_LIST_TAKE + 1,
          },
        };
      },
      onQueryStarted: async ({ chatId }, { dispatch, queryFulfilled }) => {
        const result = await queryFulfilled;
        const meta: ChatMessageListResult = result.meta! as any;

        dispatch(
          chatifyApi.util.updateQueryData('getChatMessageList', { chatId }, (draft) => {
            draft.hasMore = meta.hasMore;
            draft.messages.push(...meta!.messages);
          }),
        );
      },
    }),

    sendChatMessage: builder.mutation<void, SendChatMessageRequest>({
      queryFn: async ({ chatId, text, files }) => {
        await __settings.client.sendChatMessage({
          chatId,
          text,
          files,
        });

        return { data: null! };
      },
    }),

    readChatMessage: builder.mutation<void, { id: string }>({
      queryFn: async () => {
        return { data: null! };
      },
      onQueryStarted: async ({ id }, { dispatch }) => {
        CHAT_MESSAGE_READ_QUEUE.push(id);
        dispatch(chatifyApi.endpoints.getChatMessageReadQueue.initiate(undefined, { forceRefetch: true }));
      },
    }),

    getChatMessageReadQueue: builder.query<DebounceValue<string>[], void>({
      queryFn: async () => ({ data: CHAT_MESSAGE_READ_QUEUE.units }),
      onCacheEntryAdded: async (_, { dispatch, cacheEntryRemoved }) => {
        unsubscribe(
          CHAT_MESSAGE_READ_QUEUE.subscribe((value) => {
            dispatch(
              chatifyApi.util.updateQueryData('getChatMessageReadQueue', undefined, () => value.units),
            );
          }),
        ).when(cacheEntryRemoved);
      },
    }),

    // ====================================================
    // ===============     THREAD    ======================s
    // ====================================================
    getWatchThreadList: builder.query<ThreadWatchListItem[], void>({
      queryFn: async () => {
        const threads = await __settings.client.watchThreads();
        return { data: threads };
      },
      providesTags: ['account', 'thread'],
      onCacheEntryAdded: async (_, { cacheEntryRemoved, cacheDataLoaded, updateCachedData }) => {
        await cacheDataLoaded;

        unsubscribe(
          __settings.client.subscribe(
            'chatify://thread-created',
            ([thread, watch]) =>
              watch &&
              updateCachedData((threads) => {
                threads.push(thread);
              }),
          ),
          __settings.client.subscribe('chatify://thread-message-new', ([threadId, message, readForId]) =>
            updateCachedData((threads) => {
              const thread = threads.find((x) => x.id === threadId);
              if (!thread) return;

              thread.lastMessage = message;

              if (readForId === __settings.me()) {
                thread.lastReadMessageId = message.id;
              }
            }),
          ),
          __settings.client.subscribe('chatify://thread-message-read', ([threadId, messageId]) =>
            updateCachedData((threads) => {
              const thread = threads.find((x) => x.id === threadId);
              if (!thread) return;

              thread.lastReadMessageId = messageId;
            }),
          ),
          __settings.client.subscribe('chatify://thread-watch-added', ([thread]) =>
            updateCachedData((threads) => {
              threads.push(thread);
            }),
          ),
          __settings.client.subscribe('chatify://thread-watch-removed', ([id]) =>
            updateCachedData((threads) => {
              const index = threads.findIndex((x) => x.id === id);
              if (index !== -1) threads.splice(index, 1);
            }),
          ),
          __settings.client.subscribe('chatify://thread-updated', ([threadId, threadName]) =>
            updateCachedData((threads) => {
              const thread = threads.find((x) => x.id === threadId);
              if (!thread) return;

              thread.name = threadName;
            }),
          ),
        ).when(cacheEntryRemoved);
      },
    }),

    getThread: builder.query<Thread, { id: string }>({
      queryFn: async ({ id }) => {
        const threads = await __settings.client.thread(id);
        return { data: threads };
      },
      providesTags: ['account', 'thread'],
      onCacheEntryAdded: async ({ id }, { cacheEntryRemoved, cacheDataLoaded, updateCachedData }) => {
        await cacheDataLoaded;

        unsubscribe(
          __settings.client.subscribe(
            'chatify://thread-message-read',
            ([threadId, messageId]) =>
              id === threadId &&
              updateCachedData((t) => {
                t.lastReadMessageId = messageId;
              }),
          ),
          __settings.client.subscribe(
            'chatify://thread-message-new',
            ([threadId, message, readForId]) =>
              id === threadId &&
              updateCachedData((thread) => {
                thread.lastMessageId = message.id;
                thread.lastReadMessageId =
                  readForId === __settings.me() ? message.id : thread.lastReadMessageId;
              }),
          ),
          __settings.client.subscribe(
            'chatify://thread-watch-added',
            ([threadListItem]) =>
              id === threadListItem.id &&
              updateCachedData((thread) => {
                thread.watch = true;
                thread.lastMessageId = threadListItem.lastMessage?.id ?? null;
                thread.lastReadMessageId = threadListItem.lastReadMessageId ?? null;
              }),
          ),
          __settings.client.subscribe(
            'chatify://thread-watch-removed',
            ([threadId]) =>
              id === threadId &&
              updateCachedData((thread) => {
                thread.watch = false;
              }),
          ),
          __settings.client.subscribe(
            'chatify://thread-updated',
            ([threadId, threadName]) =>
              id === threadId &&
              updateCachedData((thread) => {
                thread.name = threadName;
              }),
          ),
        ).when(cacheEntryRemoved);
      },
    }),

    watchThread: builder.mutation<void, { id: string; watch: boolean }>({
      queryFn: async ({ id, watch }) => {
        await __settings.client.watchThread(id, watch);
        return { data: null! };
      },
    }),

    // ====================================================
    // ============     THREAD MESSAGE    =================
    // ====================================================

    getThreadMessageList: builder.query<ThreadMessageList, { threadId: string }>({
      queryFn: async (args) => {
        const requestArgs = {
          skip: 0,
          take: DEFAULT_MESSAGE_LIST_TAKE + 1,
          ...args,
        };
        const messages = await __settings.client.threadMessages(requestArgs);
        return {
          data: {
            messages: messages.slice(0, DEFAULT_MESSAGE_LIST_TAKE),
            hasMore: messages.length === DEFAULT_MESSAGE_LIST_TAKE + 1,
          },
        };
      },
      providesTags: ['account', 'thread', 'thread-message'],
      onCacheEntryAdded: async ({ threadId }, { cacheEntryRemoved, cacheDataLoaded, updateCachedData }) => {
        await cacheDataLoaded;

        unsubscribe(
          __settings.client.subscribe('chatify://thread-message-new', ([_threadId, message]) => {
            threadId === _threadId &&
              updateCachedData((messageList) => {
                !messageList.messages.some((x) => x.id === message.id) &&
                  messageList.messages.unshift(message);
              });
          }),
        ).when(cacheEntryRemoved);
      },
    }),

    fetchNextThreadMessageList: builder.mutation<void, { threadId: string }>({
      queryFn: async ({ threadId }, { getState }) => {
        const current = chatifyApi.endpoints.getThreadMessageList.select({ threadId })(getState() as any);
        const messages = await __settings.client.threadMessages({
          threadId,
          skip: current.data!.messages.length,
          take: DEFAULT_MESSAGE_LIST_TAKE + 1,
        });
        return {
          data: null!,
          meta: {
            messages: messages.slice(0, DEFAULT_MESSAGE_LIST_TAKE),
            hasMore: messages.length === DEFAULT_MESSAGE_LIST_TAKE + 1,
          },
        };
      },
      onQueryStarted: async ({ threadId }, { dispatch, queryFulfilled }) => {
        const result = await queryFulfilled;
        const meta: ThreadMessageList = result.meta! as any;

        dispatch(
          chatifyApi.util.updateQueryData('getThreadMessageList', { threadId }, (draft) => {
            draft.hasMore = meta.hasMore;
            draft.messages.push(...meta!.messages);
          }),
        );
      },
    }),

    sendThreadMessage: builder.mutation<string, SendThreadMessageRequest>({
      queryFn: async (args) => {
        const id = await __settings.client.sendThreadMessage(args);
        return { data: id };
      },
    }),

    readThreadMessage: builder.mutation<void, { id: string }>({
      queryFn: async () => {
        return { data: null! };
      },
      onQueryStarted: async ({ id }, { dispatch }) => {
        THREAD_MESSAGE_READ_QUEUE.push(id);
        dispatch(chatifyApi.endpoints.getThreadMessageReadQueue.initiate(undefined, { forceRefetch: true }));
      },
    }),

    getThreadMessageReadQueue: builder.query<DebounceValue<string>[], void>({
      queryFn: async () => ({ data: THREAD_MESSAGE_READ_QUEUE.units }),
      onCacheEntryAdded: async (_, { dispatch, cacheEntryRemoved }) => {
        unsubscribe(
          THREAD_MESSAGE_READ_QUEUE.subscribe((value) => {
            dispatch(
              chatifyApi.util.updateQueryData('getThreadMessageReadQueue', undefined, () => value.units),
            );
          }),
        ).when(cacheEntryRemoved);
      },
    }),
  }),
});

export function useChatListItem(id: string | null) {
  return useSelector((state: any) =>
    id != null
      ? __chatifyApi.endpoints.getChatList
          .select()(state)
          .data?.find((x) => x.id === id)
      : null,
  );
}

export function useUnreadThreadMessageCount() {
  const { data: threads, ...state } = chatifyApi.useGetWatchThreadListQuery();
  const count = useMemo(() => (threads != null ? calcThreadListUnreadCount(threads) : null), [threads]);
  return [count, state] as const;
}

export const chatifyApi = Object.assign(__chatifyApi, {
  settings: __settings,
  useChatListItem,
  useUnreadThreadMessageCount,
});
