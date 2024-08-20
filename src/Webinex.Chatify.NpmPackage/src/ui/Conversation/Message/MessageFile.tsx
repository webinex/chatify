import { Button } from 'antd';
import { File } from '../../../core';
import { customize, useCustomizeContext } from '../../customize';
import { useFormatter } from '../../useFormatter';
import { Icon } from '../../common';
import { ConversationCustomizeValue } from '../Conversation';
import { useCallback } from 'react';

function open(url: string) {
  const w = window.open(url, '_blank');
  w?.focus();
}

export interface MessageFileProps {
  file: File;
}

export const MessageFile = customize('MessageFile', (props: MessageFileProps) => {
  const { file } = props;
  const { flippo } = useCustomizeContext<ConversationCustomizeValue>();
  const formatter = useFormatter();

  const onOpen = useCallback(() => flippo?.getSasUrl(file.ref).then(open), [flippo, file.ref]);

  return (
    <div className="wxchtf-file">
      <div className="wxchtf-icon">
        <Button onClick={onOpen} type="link" icon={<Icon type="open-file" />} />
      </div>
      <div className="wxchtf-file-info">
        <div className="wxchtf-name">{file.name}</div>
        <div className="wxchtf-size">{formatter.size(file.bytes)}</div>
      </div>
    </div>
  );
});
