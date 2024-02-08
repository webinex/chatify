import { Avatar } from '../Avatar';
import { customize } from '../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemLastMessageAuthor = customize(
  'ChatListItemLastMessageAuthor',
  (props: ChatListItemBoxProps) => {
    const { message } = props.chat;

    if (message.sentBy.id === 'chatify::system') {
      return null;
    }

    return (
      <div className="wxchtf-chat-last-author">
        <Avatar account={message.sentBy} /> {message.sentBy.name}
      </div>
    );
  },
);
