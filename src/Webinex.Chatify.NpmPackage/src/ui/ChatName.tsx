import { FC } from 'react';
import { customize } from '../util';

export interface ChatNameProps {
  name: string;
}

const _ChatName: FC<ChatNameProps> = (props) => {
  const { name } = props;
  return <>{name}</>;
};

_ChatName.displayName = 'ChatName';

export const ChatName = customize('ChatName', _ChatName);
