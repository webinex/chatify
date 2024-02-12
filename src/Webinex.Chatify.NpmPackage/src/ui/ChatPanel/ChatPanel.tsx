import { ChatHeader } from './ChatHeader';
import { ChatBody } from './ChatBody';
import { InputBox } from '../InputBox/InputBox';
import { useSelect } from '../../core';
import { customize } from '../customize';
import { ChatMembersPanel } from './ChatMembersPanel';

export interface ChatPanelProps {
  id: string;
}

export const ChatPanel = customize('ChatPanel', (props: ChatPanelProps) => {
  const { id } = props;
  const chat = useSelect((x) => x.query.chatList.data!.find((x) => x.id === id)!, [id]);
  const chatMembersOpen = useSelect((x) => x.ui.showMembers, []);

  return (
    <div className="wxchtf-chat-panel">
      <div className="wxchtf-chat-main">
        <ChatHeader id={id} />
        <ChatBody id={id} />
        {chat.active && <InputBox />}
      </div>
      {chatMembersOpen && <ChatMembersPanel id={id} />}
    </div>
  );
});
