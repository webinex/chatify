import { useMe } from '../ChatifyContext';
import { FC, Ref, RefObject, useEffect, useMemo, useRef } from 'react';
import { Message, useDispatch, useSelect } from '../core';
import { customize } from '../util';
import { useLocalizer } from './localizer';
import { Avatar } from './Avatar';

export interface MessageBoxProps {
  message: Message;
}

export interface MessageRowProps extends MessageBoxProps {
  containerRef?: Ref<HTMLDivElement>;
}

export const MessageAuthor = customize<FC<MessageBoxProps>, string>('MessageAuthor', (props) => {
  const { sentBy } = props.message;

  return (
    <div className="wxchtf-message-author">
      <Avatar account={sentBy} />
    </div>
  );
});

export const MessageText = customize<FC<MessageBoxProps>, string>('MessageText', (props) => {
  const { text } = props.message;
  return <div className="wxchtf-message-text">{text}</div>;
});

export const MessageInfoBox = customize<FC<MessageBoxProps>, string>('MessageInfoBox', (props) => {
  const { message } = props;
  const { id, sentBy, sentAt, read } = message;
  const me = useMe();
  const my = sentBy.id === me;
  const localizer = useLocalizer();
  const reading = useSelect((x) => x.read.queued.includes(id), []);

  return (
    <div className="wxchtf-message-info-box">
      <div className="wxchtf-message-sent-at">{localizer.timestamp(sentAt)}</div>
      {read !== undefined && !my && (
        <div className="wxchtf-message-read-box">
          {reading ? localizer.message.reading() : localizer.message.read()}
        </div>
      )}
    </div>
  );
});

export const MessageContent = customize<FC<MessageBoxProps>, string>('MessageContent', (props) => {
  return (
    <div className="wxchtf-message-content">
      <MessageText {...props} />
      <MessageInfoBox {...props} />
    </div>
  );
});

export const MessageRow = customize<FC<MessageRowProps>, string>('MessageRow', (props) => {
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

export const MessageBox = customize<FC<MessageBoxProps>, string>('MessageBox', (props) => {
  const ref = useRef<HTMLDivElement | null>(null);

  return (
    <>
      <MessageReadObserver messageRef={ref} {...props} />
      <MessageRow containerRef={ref} {...props} />
    </>
  );
});

export interface MessageReadObserverProps extends MessageBoxProps {
  messageRef: RefObject<HTMLDivElement | null>;
}

export const MessageReadObserver = customize<FC<MessageReadObserverProps>, string>(
  'MessageReadObserver',
  (props) => {
    const { message, messageRef } = props;
    const dispatch = useDispatch();
    const readRef = useRef(false);

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
      observer.observe(messageRef.current!);
      return () => observer.disconnect();

      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [observer]);

    return <></>;
  },
);
