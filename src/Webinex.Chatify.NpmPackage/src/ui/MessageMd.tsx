import { Account, File } from '../core';
import { Avatar } from './Avatar';
import { useLocalizer } from './localizer';
import React from 'react';

export interface MessageMdProps {
  author?: Account;
  text: string;
  files: File[];
  my: boolean;
  read?: boolean;
  sending?: boolean;
  reading?: boolean;
  timestamp: string;
}

export const MessageMd = React.forwardRef(
  (props: MessageMdProps, forwardRef: React.ForwardedRef<HTMLDivElement>) => {
    const { author, text, my, read, reading, sending, timestamp } = props;
    const localizer = useLocalizer();

    return (
      <div
        ref={forwardRef}
        className={'wxchtf-message' + (my ? ' --my' : '') + (sending ? ' --sending' : '')}
      >
        {author && (
          <div className="wxchtf-message-author">
            <Avatar account={author} />
          </div>
        )}
        <div className="wxchtf-message-body">
          <div className="wxchtf-message-text">{text}</div>
          <div className="wxchtf-message-info-box">
            <div className="wxchtf-message-sent-at">{localizer.timestamp(timestamp)}</div>
            {read !== undefined && !my && (
              <div className="wxchtf-message-read-box">
                {reading ? localizer.message.reading() : localizer.message.read()}
              </div>
            )}
            {sending && <div className="wxchtf-message-sending-box">{localizer.message.sending()}</div>}
          </div>
        </div>
      </div>
    );
  },
);
