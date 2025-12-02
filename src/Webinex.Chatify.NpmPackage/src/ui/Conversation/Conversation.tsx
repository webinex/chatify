import { ConversationHeader } from './ConversationHeader';
import { ConversationBody } from './ConversationBody';
import { customize } from '../customize';
import { ConversationName } from './ConversationName';
import { ConversationContext, ConversationContextValue, ConversationValue } from './ConversationContext';
import { useMemo } from 'react';
import { InputBox, InputBoxCustomizeValue } from './InputBox';
import { ConversationActions } from './ConversationActions';
import { MessageCustomizeValue } from './Message';
import { SendingMessageCustomizeValue } from './SendingMessage';
import { SystemMessageCustomizeValue } from './SystemMessage';
import { CommonCustomizeValue } from '../common';
import type { Flippo } from '@webinex/flippo';

export interface ConversationProps extends Omit<ConversationContextValue, keyof ConversationValue> {
  value: ConversationValue;
}

export interface ConversationCustomizeValue
  extends InputBoxCustomizeValue,
    MessageCustomizeValue,
    SendingMessageCustomizeValue,
    SystemMessageCustomizeValue,
    CommonCustomizeValue {
  ConversationActions?: typeof ConversationActions.Component | null;
  ConversationBody?: typeof ConversationBody.Component | null;
  ConversationHeader?: typeof ConversationHeader.Component | null;
  ConversationName?: typeof ConversationName.Component | null;
  flippo?: Flippo;
}

export const Conversation = customize('Conversation', (props: ConversationProps) => {
  const {
    value,
    hasMore,
    messages,
    nextLoading,
    onNext,
    onRead,
    onSend,
    isReading,
    noReadTracking,
    onClose,
  } = props;

  const { id, active, name, lastReadMessageId } = value;

  const context = useMemo<ConversationContextValue>(
    () => ({
      id,
      name,
      active,
      lastReadMessageId,
      hasMore,
      messages,
      nextLoading,
      onNext,
      onRead,
      onSend,
      isReading,
      noReadTracking,
      onClose,
    }),
    [
      id,
      name,
      active,
      onNext,
      onRead,
      onSend,
      hasMore,
      messages,
      nextLoading,
      lastReadMessageId,
      isReading,
      noReadTracking,
      onClose,
    ],
  );

  return (
    <ConversationContext.Provider value={context}>
      <div className="wxchtf-conversation">
        <div className="wxchtf-conversation-main">
          <ConversationHeader />
          <ConversationBody />
          {active && <InputBox />}
        </div>
      </div>
    </ConversationContext.Provider>
  );
});
