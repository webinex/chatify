import { Menu, MenuProps } from 'antd';
import { useCallback, useEffect, useMemo } from 'react';
import type { MenuItemType } from 'antd/es/menu/hooks/useItems';
import { useChatList, useDispatch, useSelect } from '../core';
import { ChatListItemBox } from './ChatListItem';

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

    return chats.map((x) => ({ key: x.id, label: <ChatListItemBox chat={x} /> }));
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
