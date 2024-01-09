import { FC } from 'react';
import { File } from '../core';
import { customize } from '../util';
import { useLocalizer } from './localizer';

export interface SendingMessageProps {
  text: string;
  files: File[];
}

export const SendingMessageText = customize<FC<SendingMessageProps>, string>(
  'SendingMessageText',
  (props) => {
    const { text } = props;
    return <div className="wxchtf-sending-message-text">{text}</div>;
  },
);

export const SendingMessageInfoBox = customize<FC<SendingMessageProps>, string>(
  'SendingMessageInfoBox',
  () => {
    const localizer = useLocalizer();

    return (
      <div className="wxchtf-sending-message-info-box">
        <div className="wxchtf-sending-message-sending-box">{localizer.message.sending()}</div>
      </div>
    );
  },
);

export const SendingMessageContent = customize<FC<SendingMessageProps>, string>(
  'SendingMessageContent',
  (props) => {
    return (
      <div className="wxchtf-sending-message-content">
        <SendingMessageText {...props} />
        <SendingMessageInfoBox {...props} />
      </div>
    );
  },
);

export const SendingMessage = customize<FC<SendingMessageProps>, string>('SendingMessage', (props) => {
  return (
    <div className="wxchtf-sending-message">
      <SendingMessageContent {...props} />
    </div>
  );
});
