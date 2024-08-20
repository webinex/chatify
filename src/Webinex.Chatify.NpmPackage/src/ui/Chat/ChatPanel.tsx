import { CSSProperties } from 'react';
import { Localizer, LocalizerContext, defaultLocalizer } from '../localizer';
import { CustomizeContext } from '../customize';
import { ChatView, ChatViewCustomizeValue } from './ChatView';
import { chatifyApi } from '../../core';

export interface ChatPanelCustomizeValue extends ChatViewCustomizeValue {}

export interface ChatPanelProps {
  id: string;
  className?: string;
  style?: CSSProperties;
  localizer?: Localizer;
  customize?: ChatPanelCustomizeValue;
}

export function ChatPanel(props: ChatPanelProps) {
  const { id, className = '', customize, localizer = defaultLocalizer, style } = props;
  const { data: chat } = chatifyApi.useGetChatQuery({ id });

  return (
    <div className={`wxchtf-chatify wxchtf-chat-panel ${className}`} style={style}>
      <LocalizerContext.Provider value={localizer}>
        <CustomizeContext.Provider value={customize as any}>
          {chat && <ChatView value={chat} key={id} />}
        </CustomizeContext.Provider>
      </LocalizerContext.Provider>
    </div>
  );
}
