import { useRef } from 'react';
import { Message } from '../../core';
import { customize } from '../customize';
import { MessageReadObserver } from '../MessageReadObserver';
import { MessageRow } from './MessageRow';

export interface MessageBoxProps {
  message: Message;
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
