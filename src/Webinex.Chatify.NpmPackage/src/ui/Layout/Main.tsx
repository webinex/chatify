import { useSelect } from '../../core';
import { customize } from '../customize';
import { CreateChatPanel } from '../CreateChatPanel';
import { ChatPanel } from '../ChatPanel';

export const Main = customize('Main', () => {
  const { view, chatId } = useSelect((x) => ({ view: x.ui.view, chatId: x.ui.chatId }), []);

  return (
    <div className="wxchtf-main">
      {view === 'chat' && chatId && <ChatPanel id={chatId} key={chatId} />}
      {view === 'new-chat' && <CreateChatPanel />}
    </div>
  );
});
