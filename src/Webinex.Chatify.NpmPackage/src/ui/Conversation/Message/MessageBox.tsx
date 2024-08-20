import { useRef } from 'react';
import { customize } from '../../customize';
import { MessageReadObserver } from '../ReadObserver';
import { MessageRow } from './MessageRow';
import { MessageBase } from '../../../core';

export interface MessageBoxProps {
  message: MessageBase;
}

export const MessageBox = customize('MessageBox', (props: MessageBoxProps) => {
  const ref = useRef<HTMLDivElement | null>(null);

  return (
    <>
      <MessageReadObserver messageRef={ref} {...props} />
      <MessageRow containerRef={ref} {...props} />
    </>
  );
});
