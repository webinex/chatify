import { useSelect } from '../core';
import { AddChat } from './AddChat';
import { Chat } from './Chat';
import { ChatSettings } from './ChatSettings';

export function Main() {
  const { view, chatId } = useSelect((x) => ({ view: x.ui.view, chatId: x.ui.chatId }), []);

  return (
    <div className="wxchtf-main">
      {view === 'chat' && chatId && <Chat id={chatId} key={chatId} />}
      {view === 'new-chat' && <AddChat />}
      {view === 'chat-settings' && <ChatSettings />}
    </div>
  );
}
