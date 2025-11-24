import { ChatListCustomizeValue } from '../../common';
import { ChatListPanel } from './ChatListPanel';

export * from './ChatListPanel';

export type ChatListPanelCustomizeValue = ChatListCustomizeValue & {
  ChatListPanel?: typeof ChatListPanel.Component | null;
};
