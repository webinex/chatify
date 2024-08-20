import { customize } from '../../customize';
import { ChatList } from '../List';
import { CreateChatButton } from '../Create/CreateChatButton';

export const ChatGroupAside = customize('ChatGroupAside', () => {
  return (
    <div className="wxchtf-aside">
      <CreateChatButton />
      <ChatList />
    </div>
  );
});
