import { customize } from '../../customize';
import { MessageText } from './MessageText';
import type { MessageBoxProps } from './MessageBox';
import { MessageFileList } from './MessageFileList';

export const MessageContent = customize('MessageContent', (props: MessageBoxProps) => {
  const { files } = props.message;

  return (
    <div className="wxchtf-message-content">
      <MessageText {...props} />
      {files.length > 0 && <MessageFileList {...props} />}
    </div>
  );
});
