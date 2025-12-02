import { chatifyApi } from '../../../core';
import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';
import { useConversation } from '../ConversationContext';
import type { MessageBoxProps } from './MessageBox';

export const MessageInfoBox = customize('MessageInfoBox', (props: MessageBoxProps) => {
  const { message } = props;
  const { id, sentBy, sentAt } = message;
  const { lastReadMessageId, isReading, noReadTracking } = useConversation();
  const me = chatifyApi.settings.me();
  const my = sentBy.id === me;
  const localizer = useLocalizer();
  const isRead = lastReadMessageId != null && id.localeCompare(lastReadMessageId) <= 0;
  const reading = !isRead && isReading?.(id);

  return (
    <div className="wxchtf-message-info-box">
      {!my && <div className="wxchtf-message-author-name">{sentBy.name},</div>}
      <div className="wxchtf-message-sent-at">{localizer.timestamp(sentAt)}</div>
      {!my && !noReadTracking && (
        <div className="wxchtf-message-read-box">
          {reading ? localizer.message.reading() : localizer.message.read()}
        </div>
      )}
    </div>
  );
});
