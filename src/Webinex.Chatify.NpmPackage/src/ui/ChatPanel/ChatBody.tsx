import { MessageBox } from '../Message';
import { useGetMessagesQuery } from '../../core';
import { Next } from '../Next';
import { SendingMessageBox } from '../SendingMessage';
import { SystemMessageBox } from '../SystemMessage/SystemMessageBox';
import { MessageSkeleton } from '../MessageSkeleton';
import { customize } from '../customize';

export interface ChatBodyProps {
  id: string;
}

export const ChatBody = customize('ChatBody', (props: ChatBodyProps) => {
  const { id } = props;
  const { data: messages } = useGetMessagesQuery({ chatId: id });

  return (
    <div className="wxchtf-chat-body">
      {!messages && <MessageSkeleton count={5} />}
      {messages?.map((x, index) => {
        if (x.type === 'message' && x.sentBy.id === 'chatify::system') {
          return <SystemMessageBox key={x.id} message={x} />;
        }

        switch (x.type) {
          case 'message': {
            return <MessageBox message={x} key={x.id} />;
          }

          case 'sending': {
            return <SendingMessageBox key={x.requestId} text={x.text} files={x.files} />;
          }

          case 'next': {
            return <Next key={'next-' + messages.length + '-' + index} chatId={id} />;
          }

          default:
            return null;
        }
      })}
    </div>
  );
});
