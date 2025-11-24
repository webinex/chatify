import { Conversation, customize } from '@webinex/chatify';
import { auditApi } from './auditApi';
import { useMemo } from 'react';

export interface AuditConversationPanelProps {
  id: string;
  onClose?: () => void;

  /**
   * Number of messages to load per page.
   * @default 20
   */
  pageSize?: number;
}

export const AuditConversationPanel = customize(
  'AuditConversationPanel',
  (props: AuditConversationPanelProps) => {
    const { id, onClose, pageSize = 20 } = props;
    const { data: chat } = auditApi.useGetAuditChatQuery({ id });

    const {
      data: messageSegments,
      isFetchingNextPage,
      fetchNextPage,
      hasNextPage,
    } = auditApi.useGetAuditChatMessagesInfiniteInfiniteQuery(
      { id },
      { initialPageParam: { skip: 0, take: pageSize } },
    );

    const messages = useMemo(() => messageSegments?.pages.flatMap((x) => x.items) ?? [], [messageSegments]);

    if (!chat) return null;

    return (
      <Conversation
        value={{ active: false, id, lastReadMessageId: null, name: chat.name }}
        nextLoading={isFetchingNextPage}
        onNext={fetchNextPage}
        hasMore={!isFetchingNextPage && hasNextPage}
        messages={messages}
        noReadTracking
        onClose={onClose}
      />
    );
  },
);
