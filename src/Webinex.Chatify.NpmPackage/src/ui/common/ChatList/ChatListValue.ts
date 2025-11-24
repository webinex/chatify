import { MessageBase } from '../../../core';

export interface ChatListValue {
  id: string;
  name: string;
  lastMessage: MessageBase;
  totalUnreadCount?: number;
  lastReadMessageId?: string | null;
}
