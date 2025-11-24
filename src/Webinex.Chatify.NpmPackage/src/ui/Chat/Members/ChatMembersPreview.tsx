import { chatifyApi } from '../../../core';
import { Tooltip } from 'antd';
import { customize } from '../../customize';
import { Avatar as AntdAvatar } from 'antd';
import { Avatar } from '../../common/Avatar';
import { useChatContext } from '../ChatContext';

export const ChatMembersPreview = customize('ChatMembersPreview', () => {
  const { id } = useChatContext();
  const { data: chat } = chatifyApi.useGetChatQuery({ id });

  if (!chat?.members) {
    return null;
  }

  return (
    <AntdAvatar.Group max={{ count: 5 }}>
      {chat.members.map((x) => (
        <Tooltip key={x.id} title={x.name}>
          <Avatar account={x} />
        </Tooltip>
      ))}
    </AntdAvatar.Group>
  );
});
