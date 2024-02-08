import { useMe } from '../../ChatifyContext';
import { Ref } from 'react';
import { customize } from '../customize';
import { MessageAuthor } from './MessageAuthor';
import { MessageContent } from './MessageContent';
import { MessageBoxProps } from './MessageBox';

export interface MessageRowProps extends MessageBoxProps {
  containerRef?: Ref<HTMLDivElement>;
}

export const MessageRow = customize('MessageRow', (props: MessageRowProps) => {
  const { message, containerRef } = props;
  const { sentBy } = message;
  const me = useMe();
  const my = sentBy.id === me;

  return (
    <div ref={containerRef} className={'wxchtf-message' + (my ? ' --my' : '')}>
      <MessageAuthor {...props} />
      <MessageContent {...props} />
    </div>
  );
});
