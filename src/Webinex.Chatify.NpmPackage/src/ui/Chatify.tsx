import { ChatPanel } from './Chat';
import { ChatGroupPanel } from './ChatGroup';
import { ThreadPanel } from './Thread';

export interface ChatifyType {
  Thread: typeof ThreadPanel;
  ChatGroup: typeof ChatGroupPanel;
  Chat: typeof ChatPanel;
}

export const Chatify = {
  Thread: ThreadPanel,
  ChatGroup: ChatGroupPanel,
  Chat: ChatPanel,
} as unknown as ChatifyType;
