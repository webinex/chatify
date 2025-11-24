import { Space } from 'antd';
import { AuditPanel } from './AuditPanel';
import './index.d';
import './styles.scss';
import { Avatar, Chatify, defaultLocalizer } from '@webinex/chatify';

export * from './AuditPanel';
export * from './AuditPanelCustomizeValue';
export * from './AuditChatList';
export * from './AuditSearchValue';
export * from './AuditSearch';
export * from './auditApi';
export * from './AuditConversationPanel';

Chatify.Audit = AuditPanel;

defaultLocalizer.audit = {
  searchPlaceholder: 'Search chats by chat name or member name',
  noChatFound: 'No chats found',
  noInputBlurb: 'Start typing to search',
  textSearchOption: (text) => <span>{`Chat name includes "${text}"`}</span>,
  memberSearchOption: (account) => (
    <Space>
      <Avatar account={account} />
      {`Chat includes "${account.name}"`}
    </Space>
  ),
};
