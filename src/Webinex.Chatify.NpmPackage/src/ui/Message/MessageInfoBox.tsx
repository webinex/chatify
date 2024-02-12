import { useMe } from '../../ChatifyContext';
import { useSelect } from '../../core';
import { customize } from '../customize';
import { useLocalizer } from '../localizer';
import type { MessageBoxProps } from './MessageBox';

export const MessageInfoBox = customize('MessageInfoBox', (props: MessageBoxProps) => {
  const { message } = props;
  const { id, sentBy, sentAt } = message;
  const me = useMe();
  const my = sentBy.id === me;
  const localizer = useLocalizer();
  const reading = useSelect((x) => x.queue.read.queued.includes(id), []);

  return (
    <div className="wxchtf-message-info-box">
      <div className="wxchtf-message-sent-at">{localizer.timestamp(sentAt)}</div>
      {!my && (
        <div className="wxchtf-message-read-box">
          {reading ? localizer.message.reading() : localizer.message.read()}
        </div>
      )}
    </div>
  );
});
