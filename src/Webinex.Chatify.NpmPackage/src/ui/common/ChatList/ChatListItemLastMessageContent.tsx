import { useLocalizer } from '../../localizer';
import { formatSystemMessage } from '../../Conversation/SystemMessage';
import { FC } from 'react';
import { customize } from '../../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemLastMessageContent = customize<FC<ChatListItemBoxProps>>(
  'ChatListItemLastMessageContent',
  (props) => {
    const { lastMessage } = props.chat;
    const localizer = useLocalizer();

    return (
      <div className="wxchtf-chat-last-message">
        {lastMessage.sentBy.id === 'chatify::system'
          ? lastMessage.text && formatSystemMessage(localizer, lastMessage.text)
          : lastMessage.text}
      </div>
    );
  },
);
