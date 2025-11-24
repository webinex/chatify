import { ChatListCustomizeValue, ConversationCustomizeValue } from '@webinex/chatify';
import { AuditConversationPanel } from './AuditConversationPanel';
import { AuditChatList } from './AuditChatList';
import { AuditSearch } from './AuditSearch';

export type AuditPanelCustomizeValue = ConversationCustomizeValue &
  ChatListCustomizeValue & {
    AuditConversationPanel?: typeof AuditConversationPanel.Component;
    AuditChatList?: typeof AuditChatList.Component;
    AuditSearch?: typeof AuditSearch.Component;
  };
