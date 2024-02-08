import { useGetChatQuery } from '../../core';
import { Avatar, Tooltip } from 'antd';
import { customize } from '../customize';

export interface ChatHeaderMembersProps {
  id: string;
}

export const ChatHeaderMembers = customize('ChatHeaderMembers', (props: ChatHeaderMembersProps) => {
  const { id } = props;
  const { data: chat } = useGetChatQuery({ chatId: id });

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
});
