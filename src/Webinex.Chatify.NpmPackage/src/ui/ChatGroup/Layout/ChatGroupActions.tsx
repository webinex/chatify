import { Space } from 'antd';
import { CreateChatButton } from '../Create';
import { AutoReplyButton } from '../AutoReply/AutoReplyButton';
import { customize } from '../../customize';

export const ChatGroupActions = customize('ChatGroupActions', () => {
  return (
    <span className="wxchtf-add-btn-wrapper">
      <Space.Compact className="wxchtf-add-btn-group">
        <CreateChatButton />
        <AutoReplyButton />
      </Space.Compact>
    </span>
  );
});
