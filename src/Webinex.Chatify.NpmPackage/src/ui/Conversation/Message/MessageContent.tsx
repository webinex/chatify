import { customize } from '../../customize';
import { MessageText } from './MessageText';
import type { MessageBoxProps } from './MessageBox';
import { MessageFileList } from './MessageFileList';
import { useLocalizer } from '@/ui/localizer';

export const MessageContent = customize('MessageContent', (props: MessageBoxProps) => {
  const { files, text, removed } = props.message;
  const localizer = useLocalizer();
  const removedMessageText = localizer.message.removed();

  return (
    <div className="wxchtf-message-content">
      {removed ? (
        <div className="wxchtf-message-removed">{removedMessageText}</div>
      ) : (
        <>
          {text && <MessageText {...props} />}
          {files.length > 0 && <MessageFileList {...props} />}
        </>
      )}
    </div>
  );
});
