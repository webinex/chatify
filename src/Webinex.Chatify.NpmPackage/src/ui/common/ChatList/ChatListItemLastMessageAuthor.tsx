import { customize } from '../../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemLastMessageAuthor = customize(
  'ChatListItemLastMessageAuthor',
  (props: ChatListItemBoxProps) => {
    const { lastMessage } = props.chat;

    if (lastMessage.sentBy.id === 'chatify::system') {
      return null;
    }

    return <div className="wxchtf-chat-last-author">{lastMessage.sentBy.name}</div>;
  },
);
