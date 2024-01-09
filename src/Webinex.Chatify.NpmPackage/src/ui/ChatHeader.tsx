import { useChat, useSelect, ChatListItem } from '../core';
import { Col, Row, Space } from 'antd';
import { ChatHeaderMembers } from './ChatHeaderMembers';
import { ChatSettingsButton } from './ChatHeaderSettingsButton';
import { ChatName } from './ChatName';

export interface ChatHeaderProps {
  id: string;
}

export function ChatHeader(props: ChatHeaderProps) {
  const { id } = props;
  const { data: chat } = useChat({ chatId: id });
  const chatListItem = useSelect(
    (x) => x.query['chat-list'].data?.find((x: ChatListItem) => x.id === id),
    [id],
  );
  const name = chat?.name ?? chatListItem?.name;

  return (
    <div className="wxchtf-chat-header">
      <Row justify="space-between" align="middle">
        <Col className="wxchtf-chat-name">{name && <ChatName name={name} />}</Col>
        <Col className="wxchtf-chat-members">
          <Space align="center">
            <ChatHeaderMembers id={id} />
            <ChatSettingsButton id={id} />
          </Space>
        </Col>
      </Row>
    </div>
  );
}
