import { ChatList } from './ChatList';
import { ChatListItemAvatar } from './ChatListItemAvatar';
import { ChatListItemBox } from './ChatListItemBox';
import { ChatListItemLastMessage } from './ChatListItemLastMessage';
import { ChatListItemLastMessageAuthor } from './ChatListItemLastMessageAuthor';
import { ChatListItemLastMessageContent } from './ChatListItemLastMessageContent';
import { ChatListItemLastMessageSentAt } from './ChatListItemLastMessageSentAt';
import { ChatListItemName } from './ChatListItemName';
import { ChatListItemUnreadCount } from './ChatListItemUnreadCount';

export * from './ChatList';
export * from './ChatListItemAvatar';
export * from './ChatListItemBox';
export * from './ChatListItemLastMessage';
export * from './ChatListItemLastMessageAuthor';
export * from './ChatListItemLastMessageContent';
export * from './ChatListItemLastMessageSentAt';
export * from './ChatListItemName';
export * from './ChatListItemUnreadCount';

export interface ChatListCustomizeValue {
  ChatList?: typeof ChatList.Component | null;
  ChatListItemAvatar?: typeof ChatListItemAvatar.Component | null;
  ChatListItemBox?: typeof ChatListItemBox.Component | null;
  ChatListItemLastMessage?: typeof ChatListItemLastMessage.Component | null;
  ChatListItemLastMessageAuthor?: typeof ChatListItemLastMessageAuthor.Component | null;
  ChatListItemLastMessageContent?: typeof ChatListItemLastMessageContent.Component | null;
  ChatListItemLastMessageSentAt?: typeof ChatListItemLastMessageSentAt.Component | null;
  ChatListItemName?: typeof ChatListItemName.Component | null;
  ChatListItemUnreadCount?: typeof ChatListItemUnreadCount.Component | null;
}
