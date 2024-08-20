import { useRef } from 'react';
import { customize } from '../../customize';
import { MessageBase } from '../../../core';
import { SystemMessageRow } from './SystemMessageRow';
import { MessageReadObserver } from '../ReadObserver';

export interface SystemMessageBoxProps {
  message: MessageBase;
}

export const SystemMessageBox = customize('SystemMessageBox', (props: SystemMessageBoxProps) => {
  const ref = useRef<HTMLDivElement | null>(null);

  return (
    <>
      <MessageReadObserver messageRef={ref} {...props} />
      <SystemMessageRow containerRef={ref} {...props} />
    </>
  );
});
