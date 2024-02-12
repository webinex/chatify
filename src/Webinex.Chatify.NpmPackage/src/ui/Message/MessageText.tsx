import { customize } from '../customize';
import { MessageBoxProps } from './MessageBox';

export const MessageText = customize('MessageText', (props: MessageBoxProps) => {
  const { text } = props.message;
  return <div className="wxchtf-message-text">{text}</div>;
});
