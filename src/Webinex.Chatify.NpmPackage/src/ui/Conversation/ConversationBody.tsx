import { MessageBox } from './Message';
import { NextMessagesObserver } from './NextObserver';
import { SystemMessageBox } from './SystemMessage';
import { MessageSkeleton } from '../common/MessageSkeleton';
import { customize } from '../customize';
import { SYSTEM_ID } from '../../core';
import { useConversation } from './ConversationContext';

// TODO: Add sending message box
export const ConversationBody = customize('ConversationBody', () => {
  const { id, messages = [], hasMore, onNext, nextLoading } = useConversation();

  return (
    <div className="wxchtf-conversation-body">
      {!messages && <MessageSkeleton count={5} />}

      {messages?.map((x) => {
        if (x.sentBy.id === SYSTEM_ID) {
          return <SystemMessageBox key={x.id} message={x} />;
        }

        return <MessageBox message={x} key={x.id} />;
      })}

      {hasMore && <NextMessagesObserver key={messages.length + id} onLoad={onNext} loading={nextLoading} />}
    </div>
  );
});
