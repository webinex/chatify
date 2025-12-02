import { Col, Row } from 'antd';
import { customize } from '../customize';
import { ConversationName } from './ConversationName';
import { ConversationActions } from './ConversationActions';

export const ConversationHeader = customize('ConversationHeader', () => {
  return (
    <div className="wxchtf-conversation-header">
      <Row justify="space-between" align="middle">
        <Col>
          <ConversationName />
        </Col>
        <Col className="wxchtf-conversation-actions">
          <ConversationActions />
        </Col>
      </Row>
    </div>
  );
});
