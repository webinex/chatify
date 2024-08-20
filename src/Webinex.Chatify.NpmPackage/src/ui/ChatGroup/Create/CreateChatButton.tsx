import { Button } from 'antd';
import { customize } from '../../customize';
import { useChatGroupContext } from '../ChatGroupContext';

export const CreateChatButton = customize('CreateChatButton', () => {
  const { toggleNewChat } = useChatGroupContext();

  return (
    <span className="wxchtf-add-btn-wrapper">
      <Button className="wxchtf-add-btn" onClick={toggleNewChat} icon="+">
        Add chat
      </Button>
    </span>
  );
});
