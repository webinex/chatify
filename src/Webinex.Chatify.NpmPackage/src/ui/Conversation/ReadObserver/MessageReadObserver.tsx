import { FC, RefObject, useEffect, useMemo, useRef } from 'react';
import { MessageBoxProps } from '../Message';
import { useConversation } from '../ConversationContext';

export interface MessageReadObserverProps extends MessageBoxProps {
  messageRef: RefObject<HTMLDivElement | null>;
}
const Observer: FC<MessageReadObserverProps> = (props) => {
  const { message, messageRef } = props;
  const { onRead, lastReadMessageId } = useConversation();
  const isRead = lastReadMessageId != null && message.id.localeCompare(lastReadMessageId) <= 0;
  const readRef = useRef(isRead);

  useEffect(() => {
    readRef.current = readRef.current || isRead;
  }, [isRead]);

  const observer = useMemo(
    () =>
      new IntersectionObserver(([entry]) => {
        if (entry.isIntersecting && !readRef.current) {
          readRef.current = true;
          onRead?.(message.id);
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
};

export function MessageReadObserver(props: MessageReadObserverProps) {
  const { noReadTracking } = useConversation();

  if (noReadTracking === true) {
    return null;
  }

  return <Observer {...props} />;
}
