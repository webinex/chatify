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
  read: boolean;
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
  unreadCount: number;
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
  ids: string[];
}

export interface ReadEvent {
  messageId: string;
  chatId: string;
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
