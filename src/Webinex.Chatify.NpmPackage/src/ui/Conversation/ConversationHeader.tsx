import { Col, Row } from 'antd';
import { customize, useCustomize } from '../customize';
import { ConversationName } from './ConversationName';
import { ConversationActions } from './ConversationActions';

export const ConversationHeader = customize('ConversationHeader', () => {
  const Actions = useCustomize(ConversationActions);

  return (
    <div className="wxchtf-conversation-header">
      <Row justify="space-between" align="middle">
        <Col>
          <ConversationName />
        </Col>
        {Actions && (
          <Col className="wxchtf-conversation-actions">
            <Actions />
          </Col>
        )}
      </Row>
    </div>
  );
});
