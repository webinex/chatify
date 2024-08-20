import { customize } from '../../customize';
import { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemUnreadCount = customize('ChatListItemUnreadCount', (props: ChatListItemBoxProps) => {
  const { totalUnreadCount: unreadCount } = props.chat;

  if (unreadCount <= 0) {
    return null;
  }

  return <div className="wxchtf-unread-count">{unreadCount}</div>;
});
