import { useLocalizer } from '../../localizer';
import { formatSystemMessage } from '../../Conversation/SystemMessage';
import { FC } from 'react';
import { customize } from '../../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemLastMessageContent = customize<FC<ChatListItemBoxProps>>(
  'ChatListItemLastMessageContent',
  (props) => {
    const { message } = props.chat;
    const localizer = useLocalizer();

    return (
      <div className="wxchtf-chat-last-message">
        {message.sentBy.id === 'chatify::system'
          ? formatSystemMessage(localizer, message.text)
          : message.text}
      </div>
    );
  },
);
