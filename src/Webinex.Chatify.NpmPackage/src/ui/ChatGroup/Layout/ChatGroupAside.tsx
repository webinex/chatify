import { customize } from '../../customize';
import { ChatListPanel } from '../List';
import { CreateChatButton } from '../Create/CreateChatButton';

export const ChatGroupAside = customize('ChatGroupAside', () => {
  return (
    <div className="wxchtf-aside">
      <CreateChatButton />
      <ChatListPanel />
    </div>
  );
});
