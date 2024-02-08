import { customize } from '../customize';
import { MessageText } from './MessageText';
import { MessageInfoBox } from './MessageInfoBox';
import type { MessageBoxProps } from './MessageBox';

export const MessageContent = customize('MessageContent', (props: MessageBoxProps) => {
  return (
    <div className="wxchtf-message-content">
      <MessageText {...props} />
      <MessageInfoBox {...props} />
    </div>
  );
});
