import { Button } from 'antd';
import { customize } from '../customize';
import { useConversation } from './ConversationContext';
import { Icon } from '../common';

export const ConversationActions = customize('ConversationActions', () => {
  const { onClose } = useConversation();

  if (!onClose) return null;

  return <Button onClick={onClose} type="link" icon={<Icon type="close" />} />;
});
