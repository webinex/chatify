import { ChatName } from '../ChatName';
import { customize } from '../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemName = customize('ChatListItemName', (props: ChatListItemBoxProps) => {
  const { name } = props.chat;

  return (
    <div className="wxchtf-chat-name">
      <ChatName name={name} />
    </div>
  );
});
