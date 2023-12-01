import { Menu, MenuProps } from 'antd';
import { useCallback, useEffect, useMemo } from 'react';
import type { MenuItemType } from 'antd/es/menu/hooks/useItems';
import { useChatList, useDispatch, useSelect, ChatListItem } from '../core';
import { useLocalizer } from './localizer';
import { formatSystemMessage } from './SystemMessage';
import { Avatar } from './Avatar';
import { ChatName } from './ChatName';

function Chat(props: { chat: ChatListItem }) {
  const { chat } = props;
  const { name, message, unreadCount } = chat;
  const localizer = useLocalizer();

  return (
    <div className={'wxchtf-chat' + (!message.read ? ' --unread' : '')}>
      <div className="wxchtf-chat-name">
        <ChatName name={name} />
      </div>
      {unreadCount > 0 && <div className="wxchtf-unread-count">{unreadCount}</div>}
      <div className="wxchtf-chat-last">
        {message.sentBy.id !== 'chatify::system' && (
          <div className="wxchtf-chat-last-author">
            <Avatar account={message.sentBy} /> {message.sentBy.name}
          </div>
        )}
        <div className="wxchtf-chat-last-message">
          {message.sentBy.id === 'chatify::system'
            ? formatSystemMessage(localizer, message.text)
            : message.text}
        </div>
      </div>
    </div>
  );
}

export function Aside() {
  const activeChatId = useSelect((x) => x.ui.chatId, []);
  const dispatch = useDispatch();
  const selected = useMemo(() => (activeChatId ? [activeChatId] : []), [activeChatId]);

  const onSelect = useCallback(
    (info: Parameters<NonNullable<MenuProps['onSelect']>>[0]) =>
      dispatch({ type: 'chat_open', data: { chatId: info.selectedKeys[0] } }),
    [dispatch],
  );

  const { data: chats } = useChatList();

  const items = useMemo<MenuItemType[]>(() => {
    if (!chats) {
      return [];
    }

    return chats.map((x) => ({ key: x.id, label: <Chat chat={x} /> }));
  }, [chats]);

  useEffect(() => {
    if (!activeChatId && chats && chats.length > 0) {
      dispatch({ type: 'chat_open', data: { chatId: chats[0].id } });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chats, dispatch]);

  return (
    <div className="wxchtf-aside">
      {chats && <Menu className="wxchtf-chats" selectedKeys={selected} onSelect={onSelect} items={items} />}
    </div>
  );
}
