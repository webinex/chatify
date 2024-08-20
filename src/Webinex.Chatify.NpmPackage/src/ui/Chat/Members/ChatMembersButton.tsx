import { Button } from 'antd';
import { customize } from '../../customize';
import { Icon } from '../../common/Icon';
import { useChatContext } from '../ChatContext';

export const ChatMembersButton = customize('ChatMembersButton', () => {
  const { showMembers, onShowMembers } = useChatContext();
  return <Button onClick={() => onShowMembers(!showMembers)} type="link" icon={<Icon type="members" />} />;
});
