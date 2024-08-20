import { ChatMembersPreview } from './Members';
import { ChatMembersButton } from './Members';
import { customize } from '../customize';
import { useConversation } from '../Conversation';

export const ChatHeaderActions = customize('ChatHeaderActions', () => {
  const { active } = useConversation();

  if (!active) {
    return null;
  }

  return (
    <div className="wxchtf-chat-members">
      <ChatMembersPreview />
      <ChatMembersButton />
    </div>
  );
});
