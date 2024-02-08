import { customize } from '../customize';
import { Avatar } from '../Avatar';
import type { MessageBoxProps } from './MessageBox';

export const MessageAuthor = customize('MessageAuthor', (props: MessageBoxProps) => {
  const { sentBy } = props.message;

  return (
    <div className="wxchtf-message-author">
      <Avatar account={sentBy} />
    </div>
  );
});
