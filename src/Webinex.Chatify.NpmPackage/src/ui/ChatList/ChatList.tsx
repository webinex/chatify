import { Menu, MenuProps } from 'antd';
import { useCallback, useMemo } from 'react';
import type { MenuItemType } from 'antd/es/menu/hooks/useItems';
import { useGetChatListQuery, useDispatch, useSelect } from '../../core';
import { ChatListItemBox } from './ChatListItemBox';
import { customize } from '../customize';

function useOnSelect() {
  const dispatch = useDispatch();

  return useCallback(
    (info: Parameters<NonNullable<MenuProps['onSelect']>>[0]) =>
      dispatch({ type: 'chat_open', data: { chatId: info.selectedKeys[0] } }),
    [dispatch],
  );
}

function useMenuItems() {
  const { data: chats } = useGetChatListQuery();

  return useMemo<MenuItemType[] | undefined>(() => {
    return chats
      ?.slice()
      .sort((a, b) => -a.message.id.substring(47).localeCompare(b.message.id.substring(47)))
      .map((x) => ({ key: x.id, label: <ChatListItemBox chat={x} /> }));
  }, [chats]);
}

export const ChatList = customize('ChatList', () => {
  const activeChatId = useSelect((x) => x.ui.chatId, []);
  const selected = useMemo(() => (activeChatId ? [activeChatId] : []), [activeChatId]);
  const onSelect = useOnSelect();
  const items = useMenuItems();

  return (
    <>
      {items && (
        <Menu className="wxchtf-chat-list" selectedKeys={selected} onSelect={onSelect} items={items} />
      )}
    </>
  );
});
