import { customize } from '../../customize';
import { ChatListItemName } from './ChatListItemName';
import { ChatListItemUnreadCount } from './ChatListItemUnreadCount';
import { ChatListItemLastMessage } from './ChatListItemLastMessage';
import { ChatListItemAvatar } from './ChatListItemAvatar';
import { ChatListValue } from './ChatListValue';

export interface ChatListItemBoxProps {
  chat: ChatListValue;

  /**
   * If true, do not show read status.
   */
  noRead?: boolean;
}

export const ChatListItemBox = customize('ChatListItemBox', (props: ChatListItemBoxProps) => {
  const { noRead = false } = props;
  const { lastMessage, lastReadMessageId } = props.chat;
  const read = lastReadMessageId != null && lastReadMessageId.localeCompare(lastMessage.id) >= 0;

  return (
    <div className={'wxchtf-chat' + (!read && !noRead ? ' --unread' : '')}>
      <ChatListItemAvatar {...props} />
      <span className="wxchtf-chat-info">
        <ChatListItemName {...props} />
        <ChatListItemUnreadCount {...props} />
        <ChatListItemLastMessage {...props} />
      </span>
    </div>
  );
});
