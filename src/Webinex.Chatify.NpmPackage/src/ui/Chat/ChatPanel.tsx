import { CSSProperties } from 'react';
import { Localizer, LocalizerContext, defaultLocalizer } from '../localizer';
import { CustomizeContext } from '../customize';
import { ChatView, ChatViewCustomizeValue } from './ChatView';
import { chatifyApi } from '../../core';
import { useCompact } from '../useCompact';

export interface ChatPanelCustomizeValue extends ChatViewCustomizeValue {}

export interface ChatPanelProps {
  id: string;
  className?: string;
  style?: CSSProperties;
  localizer?: Localizer;
  customize?: ChatPanelCustomizeValue;

  /**
   * If true, the chat panel will be displayed in compact mode.
   * If a number is provided, the chat panel will be displayed in compact mode if its width is less than the provided number of pixels.
   *
   * @default 768
   */
  compact?: boolean | number;
}

export function ChatPanel(props: ChatPanelProps) {
  const { id, className = '', customize, localizer = defaultLocalizer, style, compact: compactProp } = props;
  const { data: chat } = chatifyApi.useGetChatQuery({ id });
  const [ref, compact] = useCompact(compactProp);

  return (
    <div
      className={['wxchtf-chatify', 'wxchtf-chat-panel', compact && '--compact', className]
        .filter(Boolean)
        .join(' ')}
      style={style}
      ref={ref}
    >
      <LocalizerContext.Provider value={localizer}>
        <CustomizeContext.Provider value={customize as any}>
          {chat && <ChatView value={chat} key={id} compact={compact} />}
        </CustomizeContext.Provider>
      </LocalizerContext.Provider>
    </div>
  );
}
