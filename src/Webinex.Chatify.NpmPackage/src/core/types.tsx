export interface File {
  name: string;
  bytes: number;
  ref: string;
}

export interface Account {
  id: string;
  name: string;
  avatar: string;
}

export interface MessageBase {
  id: string;
  text: string | null;
  files: File[];
  sentAt: string;
  sentBy: Account;
}

export interface ChatMessage extends MessageBase {
  chatId: string;
}

export interface Chat {
  id: string;
  name: string;
  members: Account[];
  active: boolean;
  lastReadMessageId: string | null;
}

export interface ChatListItem {
  id: string;
  name: string;
  message: ChatMessage;
  totalUnreadCount: number;
  active: boolean;
  lastReadMessageId: string | null;
}

export interface SendChatMessageRequest {
  chatId: string;
  text: string | null;
  files: File[];
}

export interface MessageBody {
  text: string | null;
  files: File[];
}

export interface AddChatRequest {
  name: string;
  message?: MessageBody;
  members: string[];
}

export interface ReadChatMessageRequest {
  id: string;
}

export interface ReadChatMessageEvent {
  chatId: string;
  newLastReadMessageId: string;
  readCount: number;
}

export interface RemoveChatMemberRequest {
  chatId: string;
  accountId: string;
  deleteHistory: boolean;
}

export interface AddChatMemberRequest {
  chatId: string;
  accountId: string;
  withHistory: boolean;
}

export interface UpdateChatNameRequest {
  id: string;
  name: string;
}

export function chatId(messageId: string) {
  return messageId.slice(0, 36);
}

export interface ThreadWatchListItem {
  id: string;
  name: string;
  createdById: string;
  createdAt: string;
  archive: boolean;
  lastMessage: ThreadMessage | null;
  lastReadMessageId: string | null;
}

export interface Thread {
  id: string;
  name: string;
  createdBy: Account;
  createdAt: string;
  archive: boolean;
  lastMessageId: string | null;
  lastReadMessageId: string | null;
  watch: boolean;
}

export interface ThreadMessage extends MessageBase {
  threadId: string;
}

export interface SendThreadMessageRequest {
  threadId: string;
  body: MessageBody;
}

export type SystemMessageText =
  | 'chatify://chat-created'
  | 'chatify://member-added'
  | 'chatify://member-removed'
  | `chatify://chat-name-changed`;

export function parseThreadMessageId(messageId: string) {
  const [threadId, index] = messageId.split('::', 2);
  return [threadId, parseInt(index, 10)] as const;
}

export function calcThreadUnreadCount(
  thread: Pick<ThreadWatchListItem, 'lastMessage' | 'lastReadMessageId'>,
) {
  const lastReadMessageIndex =
    thread.lastReadMessageId != null ? parseThreadMessageId(thread.lastReadMessageId)[1] : -1;

  const lastThreadMessageIndex =
    thread.lastMessage?.id != null ? parseThreadMessageId(thread.lastMessage?.id)[1] : -1;

  return lastThreadMessageIndex - lastReadMessageIndex;
}

export function calcThreadListUnreadCount(
  threads: Pick<ThreadWatchListItem, 'lastMessage' | 'lastReadMessageId'>[],
) {
  return threads.reduce((prev, thread) => {
    return prev + calcThreadUnreadCount(thread);
  }, 0);
}
