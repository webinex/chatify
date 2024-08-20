import { CustomizeContext, customize, useCustomizeContext } from '../customize';
import { MessageBody, chatId, chatifyApi } from '../../core';
import {
  Conversation,
  ConversationCustomizeValue,
  ConversationProps,
  ConversationValue,
} from '../Conversation';
import { useCallback, useMemo } from 'react';
import { ChatEditableHeaderName } from './ChatEditableHeaderName';
import { ChatHeaderActions } from './ChatHeaderActions';
import { ChatMembersCustomizeValue, ChatMembersPanel } from './Members';
import { ChatContext, useChatContext } from './ChatContext';

export interface ChatProps {
  value: ChatValue;
}

export interface ChatValue extends ConversationValue {}

export interface ChatViewCustomizeValue extends ConversationCustomizeValue, ChatMembersCustomizeValue {
  ChatEditableHeaderName?: typeof ChatEditableHeaderName.Component | null;
  ChatHeaderActions?: typeof ChatHeaderActions.Component | null;
  ChatView?: typeof ChatView.Component | null;
}

export const DEFAULT_CHAT_VIEW_CUSTOMIZE_VALUE: ConversationCustomizeValue = {
  ConversationName: () => <ChatEditableHeaderName />,
  ConversationActions: () => <ChatHeaderActions />,
};

function useConversation(props: ChatProps) {
  const { value: chat } = props;
  const { id, name, active, lastReadMessageId } = chat;

  const { data: messageList } = chatifyApi.useGetChatMessageListQuery({ chatId: id });
  const { messages, hasMore } = messageList ?? {};

  const [fetchNext, { isLoading: isFetchNextLoading }] = chatifyApi.useFetchNextChatMessageListMutation();
  const onNext = useCallback(() => fetchNext({ chatId: id }), [fetchNext, id]);

  const [read] = chatifyApi.useReadChatMessageMutation();
  const onRead = useCallback((id: string) => read({ id }), [read]);

  const [send] = chatifyApi.useSendChatMessageMutation();
  const onSend = useCallback((message: MessageBody) => send({ ...message, chatId: id }).unwrap(), [id, send]);

  const { data: readQueue } = chatifyApi.useGetChatMessageReadQueueQuery();

  const conversation = useMemo(() => {
    if (!id) {
      return null;
    }

    const result: ConversationProps = {
      value: { id: id!, name: name!, active: active!, lastReadMessageId: lastReadMessageId! },
      messages,
      hasMore,
      onNext,
      nextLoading: isFetchNextLoading,
      onRead,
      onSend,
      isReading: (id: string) => readQueue!.some((x) => chatId(x.value) === chatId(id) && id <= x.value),
    };

    return result;
  }, [
    onNext,
    onRead,
    onSend,
    isFetchNextLoading,
    id,
    name,
    active,
    messages,
    hasMore,
    lastReadMessageId,
    readQueue,
  ]);

  return conversation;
}

function Content(props: ChatProps) {
  const conversation = useConversation(props);
  const { showMembers } = useChatContext();

  if (!conversation) {
    return null;
  }

  return (
    <div className="wxchtf-chat">
      <Conversation {...conversation} />
      {showMembers && <ChatMembersPanel />}
    </div>
  );
}

export const ChatView = customize('ChatView', (props: ChatProps) => {
  const { value } = props;
  const customized = useCustomizeContext();
  const customize: ChatViewCustomizeValue = useMemo(
    () => ({ ...(customized ?? {}), ...DEFAULT_CHAT_VIEW_CUSTOMIZE_VALUE }),
    [customized],
  );

  return (
    <CustomizeContext.Provider value={customize}>
      <ChatContext id={value.id}>
        <Content value={value} />
      </ChatContext>
    </CustomizeContext.Provider>
  );
});
