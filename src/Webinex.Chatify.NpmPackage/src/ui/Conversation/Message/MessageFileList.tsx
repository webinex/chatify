import { customize } from '../../customize';
import { MessageBoxProps } from './MessageBox';
import { MessageFile } from './MessageFile';

export const MessageFileList = customize('MessageFileList', (props: MessageBoxProps) => {
  const { files } = props.message;
  return (
    <div className="wxchtf-file-list">
      {files.map((file) => (
        <MessageFile key={file.ref} file={file} />
      ))}
    </div>
  );
});
