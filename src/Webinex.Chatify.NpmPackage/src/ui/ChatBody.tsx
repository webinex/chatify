import { MessageBox } from './Message';
import { useMessages } from '../core';
import { LoadMore } from './LoadMore';
import { SendingMessage } from './SendingMessage';
import { SystemMessage } from './SystemMessage';
import { MessageSkeleton } from './MessageSkeleton';

export interface ChatBodyProps {
  id: string;
}

export function ChatBody(props: ChatBodyProps) {
  const { id } = props;
  const { data: messages } = useMessages({ chatId: id });

  return (
    <div className="wxchtf-chat-body">
      {!messages && <MessageSkeleton count={5} />}
      {messages?.map((x, index) => {
        if (x.type === 'message' && x.sentBy.id === 'chatify::system') {
          return <SystemMessage text={x.text} sentAt={x.sentAt} />;
        }

        switch (x.type) {
          case 'message': {
            return <MessageBox message={x} key={x.id} />;
          }

          case 'sending': {
            return <SendingMessage key={x.requestId} text={x.text} files={x.files} />;
          }

          case 'more': {
            return <LoadMore key={index} chatId={id} />;
          }

          default:
            return null;
        }
      })}
    </div>
  );
}
