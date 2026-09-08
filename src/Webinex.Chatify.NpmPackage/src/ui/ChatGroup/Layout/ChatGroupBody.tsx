import { customize } from '../../customize';
import { useChatGroupContext } from '../ChatGroupContext';
import { ChatGroupAside } from './ChatGroupAside';
import { ChatGroupMain } from './ChatGroupMain';

export const ChatGroupBody = customize('ChatGroupBody', () => {
  const { compact, view } = useChatGroupContext();
  const isAsideShown = !compact || !view;
  const isMainShown = !compact || view;

  return (
    <div className="wxchtf-body">
      {isAsideShown && <ChatGroupAside />}
      {isMainShown && <ChatGroupMain />}
    </div>
  );
});
