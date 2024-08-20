import { Menu, MenuProps } from 'antd';
import { useCallback, useMemo } from 'react';
import { chatifyApi } from '../../../core';
import { ChatListItemBox } from './ChatListItemBox';
import { customize } from '../../customize';
import { useChatGroupContext } from '../ChatGroupContext';

function useOnSelect() {
  const { openChat } = useChatGroupContext();

  return useCallback(
    (info: Parameters<NonNullable<MenuProps['onSelect']>>[0]) => openChat(info.key as string),
    [openChat],
  );
}

function useMenuItems() {
  const { data: chats } = chatifyApi.useGetChatListQuery();

  return useMemo<MenuProps['items']>(() => {
    return chats
      ?.slice()
      .sort((a, b) => -a.message.id.substring(47).localeCompare(b.message.id.substring(47)))
      .map((x) => ({ key: x.id, label: <ChatListItemBox chat={x} /> }));
  }, [chats]);
}

export const ChatList = customize('ChatList', () => {
  const { chatId } = useChatGroupContext();
  const selected = useMemo(() => (chatId ? [chatId] : []), [chatId]);
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
