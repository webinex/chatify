import React from 'react';
import { MessageBase, MessageBody } from '../../core';
import type { Flippo } from '@webinex/flippo';

export interface ConversationValue {
  id: string;
  name: string;
  active: boolean;
  lastReadMessageId: string | null;
}

export interface ConversationContextValue extends ConversationValue {
  messages?: MessageBase[];
  hasMore?: boolean;
  nextLoading: boolean;
  noReadTracking?: boolean;

  isReading?: (id: string) => boolean;
  onNext: () => void;
  onSend?: (message: MessageBody) => void;
  onRead?: (id: string) => void;
  flippo?: Flippo;
  onClose?: () => void;
}

export const ConversationContext = React.createContext<ConversationContextValue>(undefined!);

export function useConversation() {
  return React.useContext(ConversationContext);
}
