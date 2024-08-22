import { customize } from '../../customize';
import { MessageText } from './MessageText';
import type { MessageBoxProps } from './MessageBox';
import { MessageFileList } from './MessageFileList';

export const MessageContent = customize('MessageContent', (props: MessageBoxProps) => {
  const { files, text } = props.message;

  return (
    <div className="wxchtf-message-content">
      {text && <MessageText {...props} />}
      {files.length > 0 && <MessageFileList {...props} />}
    </div>
  );
});
