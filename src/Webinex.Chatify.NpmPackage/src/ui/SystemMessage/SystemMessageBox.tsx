import { useRef } from 'react';
import { customize } from '../customize';
import { Message } from '../../core';
import { MessageReadObserver } from '../MessageReadObserver';
import { SystemMessageRow } from './SystemMessageRow';

export interface SystemMessageBoxProps {
  message: Message;
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
