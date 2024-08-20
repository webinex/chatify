import { Ref } from 'react';
import { customize } from '../../customize';
import { Localizer, useLocalizer } from '../../localizer';
import { MessageBase } from '../../../core';

export interface SystemMessageRowProps {
  message: MessageBase;
  containerRef?: Ref<HTMLDivElement>;
}

export function formatSystemMessage(localizer: Localizer, text: string): React.ReactNode {
  const [key, payload] = text.split('::', 2);
  const data = payload ? JSON.parse(payload) : null;
  return localizer.system[key as keyof Localizer['system']](key as any, data);
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
