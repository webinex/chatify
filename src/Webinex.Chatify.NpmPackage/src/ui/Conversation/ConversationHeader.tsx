import { Button, Col, Row } from 'antd';
import { customize } from '../customize';
import { ConversationName } from './ConversationName';
import { ConversationActions } from './ConversationActions';
import { LeftOutlined } from '@ant-design/icons';

export interface ConversationHeaderProps {
  onBackClick?: () => void;
}

export const ConversationHeader = customize('ConversationHeader', (props: ConversationHeaderProps) => {
  const { onBackClick } = props;

  return (
    <div className="wxchtf-conversation-header">
      <Row gutter={10}>
        {onBackClick && (
          <Col flex="none">
            <Button type="text" icon={<LeftOutlined />} onClick={onBackClick} />
          </Col>
        )}
        <Col flex="auto">
          <Row justify="space-between" align="middle">
            <Col>
              <ConversationName />
            </Col>
            <Col className="wxchtf-conversation-actions">
              <ConversationActions />
            </Col>
          </Row>
        </Col>
      </Row>
    </div>
  );
});
