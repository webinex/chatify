import { useMe } from '../ChatifyContext';
import { MessageMd } from './MessageMd';
import { useEffect, useMemo, useRef } from 'react';
import { useDispatch, useSelect, Message as MessageModel } from '../core';

export interface MessageProps {
  message: MessageModel;
}

export function Message(props: MessageProps) {
  const { message } = props;
  const me = useMe();
  const my = message.sentBy.id === me;
  const dispatch = useDispatch();
  const ref = useRef<HTMLDivElement | null>(null);
  const readRef = useRef(false);
  const reading = useSelect((x) => x.read.queued.includes(message.id), []);

  const observer = useMemo(
    () =>
      new IntersectionObserver(([entry]) => {
        if (entry.isIntersecting && !readRef.current && !message.read) {
          readRef.current = true;
          dispatch({ type: 'read', data: { id: message.id, timestamp: new Date().getTime() } });
        }
      }),

    // eslint-disable-next-line react-hooks/exhaustive-deps
    [],
  );

  useEffect(() => {
    observer.observe(ref.current!);
    return () => observer.disconnect();
  }, [observer]);

  return (
    <MessageMd
      ref={ref}
      my={my}
      text={message.text}
      files={message.files}
      author={message.sentBy}
      timestamp={message.sentAt}
      read={message.read}
      sending={false}
      reading={reading}
    />
  );
}
