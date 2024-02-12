import { ChatListItem as ChatListItemModel } from '../../core';
import { customize } from '../customize';
import { ChatListItemName } from './ChatListItemName';
import { ChatListItemUnreadCount } from './ChatListItemUnreadCount';
import { ChatListItemLastMessage } from './ChatListItemLastMessage';

export interface ChatListItemBoxProps {
  chat: ChatListItemModel;
}

export const ChatListItemBox = customize('ChatListItemBox', (props: ChatListItemBoxProps) => {
  const { message, lastReadMessageId } = props.chat;
  const read = lastReadMessageId != null && lastReadMessageId.localeCompare(message.id) >= 0;

  return (
    <div className={'wxchtf-chat' + (!read ? ' --unread' : '')}>
      <ChatListItemName {...props} />
      <ChatListItemUnreadCount {...props} />
      <ChatListItemLastMessage {...props} />
    </div>
  );
});
