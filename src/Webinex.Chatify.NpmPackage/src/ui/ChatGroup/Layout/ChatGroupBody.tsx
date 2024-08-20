import { customize } from '../../customize';
import { ChatGroupAside } from './ChatGroupAside';
import { ChatGroupMain } from './ChatGroupMain';

export const ChatGroupBody = customize('ChatGroupBody', () => (
  <div className="wxchtf-body">
    <ChatGroupAside />
    <ChatGroupMain />
  </div>
));
