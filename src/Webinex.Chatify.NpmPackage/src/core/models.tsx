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

export interface Message {
  id: string;
  chatId: string;
  text: string;
  files: File[];
  sentAt: string;
  sentBy: Account;
  requestId?: string;
}

export interface Chat {
  id: string;
  name: string;
  members: Account[];
}

export interface ChatListItem {
  id: string;
  name: string;
  message: Message;
  totalUnreadCount: number;
  active: boolean;
  lastReadMessageId: string | null;
}

export interface SendMessageRequest {
  chatId: string;
  text: string;
  files: File[];
  requestId: string;
}

export interface MessageBody {
  text: string;
  files: File[];
}

export interface AddChatRequest {
  requestId: string;
  name: string;
  message?: MessageBody;
  members: string[];
}

export interface ReadRequest {
  id: string;
}

export interface ReadEvent {
  chatId: string;
  newLastReadMessageId: string;
  readCount: number;
}

export interface RemoveMemberRequest {
  chatId: string;
  accountId: string;
  deleteHistory: boolean;
}

export interface AddMemberRequest {
  chatId: string;
  accountId: string;
  withHistory: boolean;
}

export interface UpdateChatNameRequest {
  id: string;
  name: string;
}
