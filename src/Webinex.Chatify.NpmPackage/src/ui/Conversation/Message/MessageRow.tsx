import { Ref } from 'react';
import { customize } from '../../customize';
import { MessageAuthor } from './MessageAuthor';
import { MessageContent } from './MessageContent';
import { MessageBoxProps } from './MessageBox';
import { MessageInfoBox } from './MessageInfoBox';
import { chatifyApi } from '../../../core';
import { MessageActions } from './MessageActions';

export interface MessageRowProps extends MessageBoxProps {
  containerRef?: Ref<HTMLDivElement>;
}

export const MessageRow = customize('MessageRow', (props: MessageRowProps) => {
  const { message, containerRef } = props;
  const { sentBy } = message;
  const me = chatifyApi.settings.me();
  const my = sentBy.id === me;

  return (
    <div
      ref={containerRef}
      className={'wxchtf-message' + (my ? ' --my' : '') + (message.removed ? ' --removed' : '')}
    >
      <MessageAuthor {...props} />
      <div>
        <MessageInfoBox {...props} />
        <MessageContent {...props} />
        <MessageActions {...props} />
      </div>
    </div>
  );
});
