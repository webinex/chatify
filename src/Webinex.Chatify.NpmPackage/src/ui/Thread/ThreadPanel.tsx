import { useCallback, useEffect, useMemo } from 'react';
import { MessageBody, chatifyApi, parseThreadMessageId } from '../../core';
import { Conversation, ConversationCustomizeValue, ConversationProps } from '../Conversation';
import { ThreadContext, ThreadContextValue } from './ThreadContext';
import { Localizer, LocalizerContext, defaultLocalizer } from '../localizer';
import { CustomizeContext } from '../customize';
import { ThreadActions } from './ThreadActions';
import { CommonCustomizeValue } from '../common';

export interface ThreadCustomizeValue extends ConversationCustomizeValue, CommonCustomizeValue {
  ThreadActions?: typeof ThreadActions.Component | null;
}

export interface ThreadPanelProps {
  threadId: string;
  className?: string;
  customize?: ThreadCustomizeValue;
  localizer?: Localizer;
}

function usePanelState(props: ThreadPanelProps) {
  const { threadId } = props;
  const { data: thread } = chatifyApi.useGetThreadQuery({ id: threadId });
  const { data: messageList } = chatifyApi.useGetThreadMessageListQuery({ threadId });
  const { id, name, archive, lastReadMessageId, watch } = thread ?? {};
  const { messages, hasMore } = messageList ?? {};

  const [read] = chatifyApi.useReadThreadMessageMutation();
  const onRead = useCallback((id: string) => read({ id }), [read]);

  const [fetchNext, { isLoading: isFetchNextLoading }] = chatifyApi.useFetchNextThreadMessageListMutation();
  const onNext = useCallback(() => fetchNext({ threadId }), [fetchNext, threadId]);

  const [send] = chatifyApi.useSendThreadMessageMutation();
  const onSend = useCallback(
    (message: MessageBody) => send({ body: message, threadId }).unwrap(),
    [threadId, send],
  );

  const { data: readQueue } = chatifyApi.useGetThreadMessageReadQueueQuery();

  const conversation = useMemo(() => {
    if (!id) {
      return null;
    }

    const result: ConversationProps = {
      value: { id: id!, name: name!, active: !archive, lastReadMessageId: lastReadMessageId ?? null },
      messages,
      hasMore,
      onNext,
      nextLoading: isFetchNextLoading,
      onRead,
      onSend,
      noReadTracking: !watch,
      isReading: (id: string) =>
        readQueue!.some(
          (unit) => parseThreadMessageId(unit.value)[0] === parseThreadMessageId(id)[0] && id <= unit.value,
        ),
    };

    return result;
  }, [
    onNext,
    onRead,
    onSend,
    isFetchNextLoading,
    id,
    name,
    archive,
    messages,
    hasMore,
    lastReadMessageId,
    readQueue,
    watch,
  ]);

  const [watchThread] = chatifyApi.useWatchThreadMutation();

  const threadContext = useMemo<ThreadContextValue | null>(
    () =>
      thread
        ? {
            id: thread.id,
            watch: thread.watch,
            onWatch: (value) => watchThread({ id: thread.id, watch: value }).unwrap(),
          }
        : null,
    [thread, watchThread],
  );

  return [conversation, threadContext] as const;
}

const DEFAULT_CUSTOMIZE: ThreadCustomizeValue = {
  ConversationActions: () => <ThreadActions />,
};

export function ThreadPanel(props: ThreadPanelProps) {
  const { threadId, customize, localizer = defaultLocalizer, className = '' } = props;
  const [conversation, threadContext] = usePanelState(props);
  const customizeMergeValue = useMemo(() => ({ ...DEFAULT_CUSTOMIZE, ...customize }), [customize]);

  useEffect(() => {
    chatifyApi.settings.client.connectToThread(threadId);

    return () => {
      chatifyApi.settings.client.disconnectFromThread(threadId);
    };
  }, [threadId]);

  if (!conversation || !threadContext) {
    return null;
  }

  return (
    <LocalizerContext.Provider value={localizer}>
      <ThreadContext.Provider value={threadContext}>
        <CustomizeContext.Provider value={customizeMergeValue as any}>
          <div className={`wxchtf-chatify wxchtf-thread ${className}`}>
            {conversation && <Conversation {...conversation} />}
          </div>
        </CustomizeContext.Provider>
      </ThreadContext.Provider>
    </LocalizerContext.Provider>
  );
}
