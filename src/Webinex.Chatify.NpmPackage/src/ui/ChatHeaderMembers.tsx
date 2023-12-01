import { FC } from 'react';
import { useChat } from '../core';
import { Avatar, Tooltip } from 'antd';
import { customize } from '../util';

export interface ChatHeaderMembersProps {
  id: string;
}

const _ChatHeaderMembers: FC<ChatHeaderMembersProps> = (props) => {
  const { id } = props;
  const { data: chat } = useChat({ chatId: id });

  if (!chat?.members) {
    return null;
  }

  return (
    <Avatar.Group maxCount={5}>
      {chat.members.map((x) => (
        <Tooltip key={x.id} title={x.name}>
          <Avatar src={x.avatar}>{x.name}</Avatar>
        </Tooltip>
      ))}
    </Avatar.Group>
  );
};

_ChatHeaderMembers.displayName = 'ChatHeaderMembers';
export const ChatHeaderMembers = customize('ChatHeaderMembers', _ChatHeaderMembers);
