import { customize } from '../../customize';
import { CreateChatPanel } from '../Create';
import { ChatView } from '../../Chat';
import { useChatGroupContext } from '../ChatGroupContext';
import { chatifyApi } from '../../../core';
import { AutoReplyPanel } from '../AutoReply';

export const ChatGroupMain = customize('ChatGroupMain', () => {
  const { chatId, view } = useChatGroupContext();
  const chat = chatifyApi.useChatListItem(chatId);

  return (
    <div className="wxchtf-main">
      {view === 'chat' && chat && <ChatView value={chat} key={chatId} />}
      {view === 'new-chat' && <CreateChatPanel />}
      {view === 'auto-reply' && <AutoReplyPanel />}
    </div>
  );
});
