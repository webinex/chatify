import { customize } from '../../customize';
import { ChatListPanel } from '../List';
import { ChatGroupActions } from './ChatGroupActions';

export const ChatGroupAside = customize('ChatGroupAside', () => {
  return (
    <div className="wxchtf-aside">
      <ChatGroupActions />
      <ChatListPanel />
    </div>
  );
});
