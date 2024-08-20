import { File } from '../../../core';
import { customize } from '../../customize';
import { SendingMessageContent } from './SendingMessageContent';

export interface SendingMessageBoxProps {
  text: string;
  files: File[];
}

export const SendingMessageBox = customize('SendingMessageBox', (props: SendingMessageBoxProps) => {
  return (
    <div className="wxchtf-sending-message">
      <SendingMessageContent {...props} />
    </div>
  );
});
