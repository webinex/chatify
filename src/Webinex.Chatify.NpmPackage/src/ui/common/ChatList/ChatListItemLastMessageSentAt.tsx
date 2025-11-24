import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';
import { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemLastMessageSentAt = customize(
  'ChatListItemLastMessageSentAt',
  (props: ChatListItemBoxProps) => {
    const {
      chat: {
        lastMessage: { sentAt },
      },
    } = props;

    const localizer = useLocalizer();

    return (
      <div className="wxchtf-chat-last-sent-at">
        {localizer.isToday(sentAt) ? localizer.timestamp(sentAt) : localizer.dateTime(sentAt)}
      </div>
    );
  },
);
