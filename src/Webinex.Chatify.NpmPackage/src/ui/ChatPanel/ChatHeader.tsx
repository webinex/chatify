import { Col, Row, Space } from 'antd';
import { ChatHeaderMembers } from './ChatHeaderMembers';
import { ChatMembersButton } from './ChatMembersButton';
import { customize } from '../customize';
import { ChatHeaderName } from './ChatHeaderName';
import { useSelect } from '../../core';

export interface ChatHeaderProps {
  id: string;
}

export const ChatHeader = customize('ChatHeader', (props: ChatHeaderProps) => {
  const { id } = props;
  const chat = useSelect((x) => x.query.chatList.data!.find((x) => x.id === id)!, [id]);

  return (
    <div className="wxchtf-chat-header">
      <Row justify="space-between" align="middle">
        <Col>
          <ChatHeaderName id={id} />
        </Col>
        {chat.active && (
          <Col className="wxchtf-chat-members">
            <Space align="center">
              <ChatHeaderMembers id={id} />
              <ChatMembersButton id={id} />
            </Space>
          </Col>
        )}
      </Row>
    </div>
  );
});
