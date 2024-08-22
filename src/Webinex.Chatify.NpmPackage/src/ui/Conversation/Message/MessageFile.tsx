import { Button } from 'antd';
import { File } from '../../../core';
import { customize, useCustomizeContext } from '../../customize';
import { useFormatter } from '../../useFormatter';
import { Icon } from '../../common';
import { ConversationCustomizeValue } from '../Conversation';
import { useCallback, useState } from 'react';

function open(url: string) {
  const w = window.open(url, '_blank');
  w?.focus();
}

export interface MessageFileProps {
  file: File;
}

function useOpen(props: MessageFileProps) {
  const { file } = props;
  const { flippo } = useCustomizeContext<ConversationCustomizeValue>();
  const [isOpenFetching, setIsOpenFetching] = useState(false);

  const onOpen = useCallback(
    () =>
      flippo &&
      Promise.resolve(setIsOpenFetching(true))
        .then(() => flippo!.getSasUrl(file.ref))
        .then(open)
        .finally(() => setIsOpenFetching(false)),
    [flippo, file.ref],
  );

  return [onOpen, { isOpenFetching }] as const;
}

export const MessageFile = customize('MessageFile', (props: MessageFileProps) => {
  const { file } = props;
  const formatter = useFormatter();
  const [onOpen, { isOpenFetching }] = useOpen(props);

  return (
    <div className="wxchtf-file">
      <div className="wxchtf-icon">
        <Button loading={isOpenFetching} onClick={onOpen} type="link" icon={<Icon type="open-file" />} />
      </div>
      <div className="wxchtf-file-info">
        <div className="wxchtf-name" title={file.name}>
          {file.name}
        </div>
        <div className="wxchtf-size">{formatter.size(file.bytes)}</div>
      </div>
    </div>
  );
});
