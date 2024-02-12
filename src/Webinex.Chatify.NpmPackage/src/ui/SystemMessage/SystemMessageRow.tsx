import { Ref } from 'react';
import { customize } from '../customize';
import { Localizer, useLocalizer } from '../localizer';
import { Message } from '../../core';

export interface SystemMessageRowProps {
  message: Message;
  containerRef?: Ref<HTMLDivElement>;
}

export function formatSystemMessage(localizer: Localizer, text: string): React.ReactNode {
  if (text === 'chatify://chat-created') {
    return localizer.system.chatCreated();
  }

  if (text.startsWith('chatify://member-added')) {
    return localizer.system.memberAdded();
  }

  if (text.startsWith('chatify://member-removed')) {
    return localizer.system.memberRemoved();
  }

  if (text.startsWith('chatify://chat-name-changed::')) {
    const data = JSON.parse(text.substring('chatify://chat-name-changed::'.length));
    return localizer.system.chatNameChanged(data.newName);
  }

  return text;
}

export const SystemMessageRow = customize('SystemMessageRow', (props: SystemMessageRowProps) => {
  const { message, containerRef } = props;
  const { text, sentAt } = message;
  const localizer = useLocalizer();

  return (
    <div ref={containerRef} className="wxchtf-system-message">
      <div className="wxchtf-text">{formatSystemMessage(localizer, text)}</div>
      <div className="wxchtf-timestamp">{localizer.timestamp(sentAt)}</div>
    </div>
  );
});
