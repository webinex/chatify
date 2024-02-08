import { customize } from './customize';

export interface ChatNameProps {
  name: string;
}

export const ChatName = customize('ChatName', (props: ChatNameProps) => {
  const { name } = props;
  return <>{name}</>;
});
