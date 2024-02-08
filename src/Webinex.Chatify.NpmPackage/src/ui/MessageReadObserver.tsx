import { FC, RefObject, useEffect, useMemo, useRef } from 'react';
import { useDispatch, useSelect } from '../core';
import { customize } from './customize';
import { MessageBoxProps } from './Message/MessageBox';

export interface MessageReadObserverProps extends MessageBoxProps {
  messageRef: RefObject<HTMLDivElement | null>;
}

export const MessageReadObserver = customize<FC<MessageReadObserverProps>>('MessageReadObserver', (props) => {
  const { message, messageRef } = props;
  const dispatch = useDispatch();
  const isRead = useSelect((x) => {
    const lastReadMessageId = x.query.chatList.data!.find((x) => x.id === message.chatId)!.lastReadMessageId;
    return lastReadMessageId != null && message.id.localeCompare(lastReadMessageId) <= 0;
  }, []);

  const readRef = useRef(isRead);

  useEffect(() => {
    readRef.current = readRef.current || isRead;
  }, [isRead]);

  const observer = useMemo(
    () =>
      new IntersectionObserver(([entry]) => {
        if (entry.isIntersecting && !readRef.current) {
          readRef.current = true;
          dispatch({ type: 'read', data: { id: message.id, timestamp: new Date().getTime() } });
        }
      }),

    // eslint-disable-next-line react-hooks/exhaustive-deps
    [],
  );

  useEffect(() => {
    observer.observe(messageRef.current!);
    return () => observer.disconnect();

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [observer]);

  return <></>;
});
