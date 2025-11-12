import type { State } from './state';
import { Account, Chat, ChatListItem, Message, ReadEvent, File } from './models';

export const ACTIONS = {
  update: (state: State, { mutation }: { mutation: (state: State) => State }) => {
    mutation(state);
  },

  messages_fetch: (state: State, { chatId }: { chatId: string }) => {
    state.query.messages[chatId] = {
      ...state.query.messages[chatId],
      uninitialized: false,
      isFetching: true,
    };
  },

  messages_fulfilled: (
    state: State,
    { chatId, messages, hasMore }: { chatId: string; messages: Message[]; hasMore: boolean },
  ) => {
    const qState = state.query.messages[chatId];

    qState.error = undefined;
    qState.isFetching = false;
    qState.data = messages.map((x) => ({ type: 'message', ...x }));

    if (hasMore) {
      qState.data!.push({ type: 'next' });
    }
  },

  messages_rejected: (state: State, { chatId, error }: { chatId: string; error: any }) => {
    const qState = state.query.messages[chatId];
    qState.error = error;
    qState.isFetching = false;
  },

  fetchNext_fetch: (state: State, { chatId }: { chatId: string }) => {
    state.mutation.fetchNext = { isFetching: true };
  },

  fetchNext_fulfilled: (
    state: State,
    { chatId, messages, hasMore }: { chatId: string; messages: Message[]; hasMore: boolean },
  ) => {
    state.mutation.fetchNext = { isFetching: false, error: undefined };
    state.query.messages[chatId].data = state.query.messages[chatId].data!.filter((x) => x.type !== 'next');
    state.query.messages[chatId].data!.push(...messages.map((x) => ({ type: 'message' as const, ...x })));

    if (hasMore) state.query.messages[chatId].data!.push({ type: 'next' });
  },

  fetchNext_rejected: (state: State, { chatId, error }: { chatId: string; error: any }) => {
    state.mutation.fetchNext = { isFetching: false, error };
  },

  chatList_fetch: (state: State) => {
    state.query.chatList.isFetching = true;
    state.query.chatList.uninitialized = false;
  },

  chatList_fulfilled: (state: State, { chats }: { chats: ChatListItem[] }) => {
    chats = chats.sort((a, b) => -a.message.id.substring(47).localeCompare(b.message.id.substring(47)));

    state.query.chatList.isFetching = false;
    state.query.chatList.error = undefined;
    state.query.chatList.data = chats;

    if (state.ui.chatId === undefined && chats.length > 0) {
      state.ui.view = 'chat';
      state.ui.chatId = chats[0].id;
    }
  },

  chatList_rejected: (state: State, { error }: { error: any }) => {
    state.query.chatList.isFetching = false;
    state.query.chatList.error = error;
  },

  removeMember_fetch: (state: State) => {
    state.mutation.removeMember = { isFetching: true };
  },

  removeMember_fulfilled: (state: State) => {
    state.mutation.removeMember = { isFetching: false, error: undefined };
  },

  removeMember_rejected: (state: State, { error }: { error: any }) => {
    state.mutation.removeMember = { isFetching: false, error };
  },

  addMember_fetch: (state: State) => {
    state.mutation.addMember = { isFetching: true };
  },

  addMember_fulfilled: (state: State) => {
    state.mutation.addMember = { isFetching: false, error: undefined };
  },

  addMember_rejected: (state: State, { error }: { error: any }) => {
    state.mutation.addMember = { isFetching: false, error };
  },

  updateChatName_fetch: (state: State) => {
    state.mutation.updateChatName = { isFetching: true };
  },

  updateChatName_fulfilled: (state: State) => {
    state.mutation.updateChatName = { isFetching: false, error: undefined };
  },

  updateChatName_rejected: (state: State, { error }: { error: any }) => {
    state.mutation.updateChatName = { isFetching: false, error };
  },

  updateAccount_fetch: (state: State) => {
    state.mutation.updateAccount = { isFetching: true };
  },

  updateAccount_fulfilled: (state: State) => {
    state.mutation.updateAccount = { isFetching: false, error: undefined };
  },

  updateAccount_rejected: (state: State, { error }: { error: any }) => {
    state.mutation.updateAccount = { isFetching: false, error };
  },

  addChat_fetch: (state: State) => {
    state.mutation.addChat = { isFetching: true };
  },

  addChat_fulfilled: (state: State) => {
    state.mutation.addChat = { isFetching: false, error: undefined };
  },

  addChat_rejected: (state: State, { error }: { error: any }) => {
    state.mutation.addChat = { isFetching: false, error };
  },

  accounts_fetch: (state: State) => {
    state.query.accounts = { isFetching: true, uninitialized: false };
  },

  accounts_fulfilled: (state: State, { accounts }: { accounts: Account[] }) => {
    state.query.accounts.data = accounts;
    state.query.accounts.isFetching = false;
    state.query.accounts.error = undefined;
  },

  accounts_rejected: (state: State, { error }: { error: any }) => {
    state.query.accounts.isFetching = false;
    state.query.accounts.error = error;
  },

  chat_open: (state: State, { chatId }: { chatId: string }) => {
    state.ui.chatId = chatId;
    state.ui.view = 'chat';
  },

  new_chat_open: (state: State) => {
    state.ui.chatId = undefined;
    state.ui.view = 'new-chat';
  },

  toggle_members_view: (state: State) => {
    state.ui.showMembers = !state.ui.showMembers;
  },

  message_received: (
    state: State,
    { message, requestId, me }: { message: Message; requestId: string; me: string },
  ) => {
    const messagesState = state.query.messages[message.chatId];

    if (messagesState?.data) {
      messagesState.data.unshift({ type: 'message', ...message });
      messagesState.data = messagesState.data.filter(
        (x) => x.type !== 'sending' || x.requestId !== requestId,
      );
    }

    const chatListItemState = state.query.chatList?.data?.find((x) => x.id === message.chatId);

    if (chatListItemState) {
      chatListItemState.message = message;
    }

    if (chatListItemState && message.sentBy.id !== me) {
      chatListItemState.totalUnreadCount++;
    }

    if (chatListItemState && message.sentBy.id === me) {
      chatListItemState.lastReadMessageId = message.id;
      chatListItemState.totalUnreadCount = 0;
    }
  },

  chat_created: (state: State, { chat }: { chat: ChatListItem; requestId?: string }) => {
    state.query.chatList?.data?.unshift(chat);
  },

  read: (state: State, { id, timestamp }: { id: string; timestamp: number }) => {
    const queuedSameChat = state.queue.read.queued.filter((x) => x.substring(0, 36) === id.substring(0, 36));

    if (queuedSameChat.some((x) => x.localeCompare(id) >= 0)) {
      return;
    }

    state.queue.read.queued.filter((x) => queuedSameChat.includes(x));
    state.queue.read.queued.push(id);
    state.queue.read.timestamp = timestamp;
  },

  read_send: (state: State, { id }: { id: string }) => {
    state.queue.read.queued = state.queue.read.queued.filter((x) => x !== id);
    state.queue.read.processing.push(id);
  },

  read_reject: (state: State, { id, timestamp }: { id: string; timestamp: number }) => {
    state.queue.read.queued.push(id);
    state.queue.read.processing = state.queue.read.processing.filter((x) => x !== id);
    state.queue.read.timestamp = timestamp;
  },

  read_received: (state: State, { events }: { events: ReadEvent[] }) => {
    state.queue.read.processing = state.queue.read.processing.filter((item) => {
      const event = events.find((x) => x.chatId === item.substring(0, 36));
      return !event || event.newLastReadMessageId.localeCompare(item) >= 0;
    });

    for (const event of events) {
      state.queue.read.processing = state.queue.read.processing.filter(
        (x) => x.substring(0, 36) !== event.chatId || x.localeCompare(event.newLastReadMessageId) > 0,
      );
      const chatListItem = state.query.chatList.data?.find((x) => x.id === event.chatId);

      if (chatListItem) {
        chatListItem.lastReadMessageId = event.newLastReadMessageId;
        chatListItem.totalUnreadCount = chatListItem.totalUnreadCount - event.readCount;
      }
    }
  },

  member_added: (
    state: State,
    {
      chatId,
      chatName,
      account,
      me,
      message,
      withHistory,
    }: {
      chatId: string;
      chatName: string;
      account: Account;
      me: string;
      message: Message;
      withHistory: boolean;
    },
  ) => {
    const { chats, messages, chatList } = state.query;
    chats[chatId]?.data?.members.push(account);
    const chatListItem = chatList?.data?.find((x) => x.id === chatId);

    if (account.id !== me && chatListItem != null) {
      chatListItem.message = message;
      chatListItem.totalUnreadCount++;
    }

    messages[chatId]?.data?.unshift({ type: 'message', ...message });

    if (account.id === me && chatList.data !== undefined && !chatListItem) {
      chatList.data.unshift({
        id: chatId,
        name: chatName,
        message,
        totalUnreadCount: 1,
        active: true,
        lastReadMessageId: null,
      });
    }

    if (account.id === me && chatListItem) {
      chatListItem.active = true;
      chatListItem.message = message;
      chatListItem.totalUnreadCount++;
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
      message,
    }: { chatId: string; accountId: string; deleteHistory: boolean; me: string; message: Message },
  ) => {
    const { chats, chatList } = state.query;
    const chat = chats[chatId];

    if (chat?.data) {
      chat.data.members = chat.data.members.filter((x) => x.id !== accountId);
    }

    if (accountId === me && state.query.chats[chatId].data) {
      delete state.query.chats[chatId];
    }

    if (accountId === me && deleteHistory && chatList?.data) {
      chatList.data = chatList.data.filter((x) => x.id !== chatId);

      if (state.ui.chatId === chatId) {
        state.ui.chatId = chatList.data.at(0)?.id;
      }
    }

    if (accountId === me && !deleteHistory && chatList?.data) {
      const listItem = chatList.data.find((x) => x.id === chatId)!;
      listItem.active = false;
    }

    if ((accountId !== me || !deleteHistory) && state.query.messages[chatId].data) {
      state.query.messages[chatId].data!.unshift({ ...message, type: 'message' });
    }

    if ((accountId !== me || !deleteHistory) && state.query.chatList.data) {
      const listItem = state.query.chatList.data.find((x) => x.id === chatId)!;
      listItem.message = message;
      listItem.totalUnreadCount++;
    }

    if (accountId === me && deleteHistory && state.query.messages[chatId].data) {
      delete state.query.messages[chatId];
    }
  },

  chat_fetch: (state: State, { chatId }: { chatId: string }) => {
    state.query.chats[chatId] = { isFetching: true, uninitialized: false, data: undefined, error: undefined };
  },

  chat_fulfilled: (state: State, { chat }: { chat: Chat }) => {
    const chatState = state.query.chats[chat.id];
    chatState.isFetching = false;
    chatState.error = undefined;
    chatState.data = chat;
  },

  chat_rejected: (state: State, { chatId, error }: { chatId: string; error: any }) => {
    const chatState = state.query.chats[chatId];
    chatState.isFetching = false;
    chatState.error = error;
  },

  send_fetch: (
    state: State,
    {
      chatId,
      text,
      files,
      requestId,
    }: {
      chatId: string;
      text: string;
      files: File[];
      requestId: string;
    },
  ) => {
    state.mutation.send = { isFetching: true, error: undefined };
    state.query.messages[chatId].data?.push({ type: 'sending', text, files, requestId });
  },

  send_fulfilled: (state: State, { chatId, requestId }: { chatId: string; requestId: string }) => {
    state.mutation.send = { isFetching: false, error: undefined };
  },

  send_rejected: (
    state: State,
    { chatId, requestId, error }: { chatId: string; requestId: string; error: any },
  ) => {
    state.mutation.send = { isFetching: false, error };
    state.query.messages[chatId].data = state.query.messages[chatId].data?.filter(
      (x) => x.type !== 'sending' || x.requestId !== requestId,
    );
  },

  chatNameChanged_received: (
    state: State,
    { chatId, newName, message }: { chatId: string; newName: string; message: Message },
  ) => {
    if (state.query.chatList.data) {
      const chatListItem = state.query.chatList.data.find((x) => x.id === chatId);
      chatListItem!.name = newName;
      chatListItem!.message = message;
      chatListItem!.totalUnreadCount++;
    }

    if (state.query.chats[chatId].data) {
      state.query.chats[chatId].data!.name = newName;
    }

    if (state.query.messages[chatId].data) {
      state.query.messages[chatId].data!.unshift({ type: 'message', ...message });
    }
  },
};

export type Action = {
  [K in keyof typeof ACTIONS]: {
    type: K;
    data: Parameters<(typeof ACTIONS)[K]>[1];
  };
}[keyof typeof ACTIONS];
