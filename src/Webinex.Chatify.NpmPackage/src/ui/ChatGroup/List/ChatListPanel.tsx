import { useMemo } from 'react';
import { chatifyApi } from '../../../core';
import { customize } from '../../customize';
import { useChatGroupContext } from '../ChatGroupContext';
import { ChatListValue, ChatList } from '../../common';

function useData() {
  const { data: chats } = chatifyApi.useGetChatListQuery();
  return useMemo(
    () =>
      chats?.map(
        (x): ChatListValue => ({
          id: x.id,
          name: x.name,
          lastMessage: x.message,
          lastReadMessageId: x.lastReadMessageId,
        }),
      ) ?? [],
    [chats],
  );
}

export const ChatListPanel = customize('ChatListPanel', () => {
  const { chatId, openChat } = useChatGroupContext();
  const data = useData();

  return <ChatList items={data} selected={chatId} onSelect={openChat} />;
});
