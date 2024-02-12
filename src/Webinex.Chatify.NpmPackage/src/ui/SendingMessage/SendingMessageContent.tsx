import { customize } from '../customize';
import { SendingMessageText } from './SendingMessageText';
import { SendingMessageInfoBox } from './SendingMessageInfoBox';
import type { SendingMessageBoxProps } from './SendingMessageBox';

export const SendingMessageContent = customize('SendingMessageContent', (props: SendingMessageBoxProps) => {
  return (
    <div className="wxchtf-sending-message-content">
      <SendingMessageText {...props} />
      <SendingMessageInfoBox {...props} />
    </div>
  );
});
