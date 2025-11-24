import { customize } from '../../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';
import { ChatListItemLastMessageContent } from './ChatListItemLastMessageContent';
import { ChatListItemLastMessageAuthor } from './ChatListItemLastMessageAuthor';
import { ChatListItemLastMessageSentAt } from './ChatListItemLastMessageSentAt';

export const ChatListItemLastMessage = customize('ChatListItemLastMessage', (props: ChatListItemBoxProps) => {
  return (
    <div className="wxchtf-chat-last">
      <ChatListItemLastMessageAuthor {...props} />
      <ChatListItemLastMessageContent {...props} />
      <ChatListItemLastMessageSentAt {...props} />
    </div>
  );
});
