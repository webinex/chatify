import { useLocalizer } from './localizer';
import { formatSystemMessage } from './SystemMessage';
import { Avatar } from './Avatar';
import { ChatName } from './ChatName';
import { ChatListItem as ChatListItemModel } from '../core';
import { FC } from 'react';
import { customize } from '../util';

export interface ChatListItemBoxProps {
  chat: ChatListItemModel;
}

const _ChatListItemName: FC<ChatListItemBoxProps> = (props) => {
  const { name } = props.chat;

  return (
    <div className="wxchtf-chat-name">
      <ChatName name={name} />
    </div>
  );
};

_ChatListItemName.displayName = 'ChatListItemName';

export const ChatListItemName = customize('ChatListItemName', _ChatListItemName);

const _ChatListItemUnreadCount: FC<ChatListItemBoxProps> = (props) => {
  const { unreadCount } = props.chat;

  if (unreadCount <= 0) {
    return null;
  }

  return <div className="wxchtf-unread-count">{unreadCount}</div>;
};

_ChatListItemUnreadCount.displayName = 'ChatListItemUnreadCount';

export const ChatListItemUnreadCount = customize('ChatListItemUnreadCount', _ChatListItemUnreadCount);

const _ChatListItemLastMessage: FC<ChatListItemBoxProps> = (props) => {
  return (
    <div className="wxchtf-chat-last">
      <ChatListItemLastMessageAuthor {...props} />
      <ChatListItemLastMessageContent {...props} />
    </div>
  );
};

_ChatListItemLastMessage.displayName = 'ChatListItemLastMessage';

export const ChatListItemLastMessage = customize('ChatListItemLastMessage', _ChatListItemLastMessage);

export const ChatListItemLastMessageAuthor = customize<FC<ChatListItemBoxProps>, string>(
  'ChatListItemLastMessageAuthor',
  (props) => {
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

export const ChatListItemLastMessageContent = customize<FC<ChatListItemBoxProps>, string>(
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

const _ChatListItemBox: FC<ChatListItemBoxProps> = (props) => {
  const { message } = props.chat;

  return (
    <div className={'wxchtf-chat' + (!message.read ? ' --unread' : '')}>
      <ChatListItemName {...props} />
      <ChatListItemUnreadCount {...props} />
      <ChatListItemLastMessage {...props} />
    </div>
  );
};

_ChatListItemBox.displayName = 'ChatListItemBox';

export const ChatListItemBox = customize('ChatListItemBox', _ChatListItemBox);
