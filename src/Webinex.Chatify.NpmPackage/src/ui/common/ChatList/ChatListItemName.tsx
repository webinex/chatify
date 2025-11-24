import { customize } from '../../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemName = customize('ChatListItemName', (props: ChatListItemBoxProps) => {
  const { chat } = props;

  return <div className="wxchtf-chat-name">{chat.name}</div>;
});
