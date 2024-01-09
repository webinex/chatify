import { Localizer, useLocalizer } from './localizer';

export interface SystemMessageProps {
  text: string;
  sentAt: string;
}

export function formatSystemMessage(localizer: Localizer, text: string): React.ReactNode {
  if (text === 'chatify://chat-created') {
    return localizer.system.chatCreated();
  }

  if (text.startsWith('chatify://member-added::')) {
    return localizer.system.memberAdded();
  }

  return text;
}

export function SystemMessage(props: SystemMessageProps) {
  const { text, sentAt } = props;
  const localizer = useLocalizer();

  return (
    <div className="wxchtf-system-message">
      <div className="wxchtf-text">{formatSystemMessage(localizer, text)}</div>
      <div className="wxchtf-timestamp">{localizer.timestamp(sentAt)}</div>
    </div>
  );
}