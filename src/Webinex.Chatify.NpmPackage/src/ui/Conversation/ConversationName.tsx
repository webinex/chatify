import { customize } from '../customize';
import { useConversation } from './ConversationContext';

export const ConversationName = customize('ConversationName', () => {
  const { name } = useConversation();
  return <span className="wxchtf-conversation-name">{name}</span>;
});
