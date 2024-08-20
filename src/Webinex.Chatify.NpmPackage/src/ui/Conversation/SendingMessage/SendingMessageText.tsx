import { customize } from '../../customize';
import type { SendingMessageBoxProps } from './SendingMessageBox';

export const SendingMessageText = customize('SendingMessageText', (props: SendingMessageBoxProps) => {
  const { text } = props;
  return <div className="wxchtf-sending-message-text">{text}</div>;
});
