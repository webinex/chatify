import { ChatList, customize } from '@webinex/chatify';
import { Empty, Skeleton } from 'antd';
import { AuditChatListItem } from './auditApi';

export interface AuditChatListProps {
  items: AuditChatListItem[];
  loading: boolean;
  selected: string | null;
  onSelect: (id: string) => void;
  top?: number;
}

export const AuditChatList = customize('AuditChatList', (props: AuditChatListProps) => {
  const { items, selected, onSelect, loading, top } = props;

  if (loading) {
    return <Skeleton />;
  }

  if (items.length === 0) {
    return <Empty />;
  }

  return <ChatList items={items} selected={selected} onSelect={onSelect} noRead top={top} />;
});
