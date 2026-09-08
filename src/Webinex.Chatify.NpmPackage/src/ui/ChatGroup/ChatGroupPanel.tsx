import { CSSProperties, FC, useEffect, useMemo, useRef } from 'react';
import { Localizer, LocalizerContext, defaultLocalizer } from '../localizer';
import { CreateChatPanelCustomizeValue } from './Create';
import { ChatGroupBody, ChatGroupFooter, ChatGroupHeader, ChatGroupLayoutCustomizeValue } from './Layout';
import { ChatListPanelCustomizeValue } from './List';
import { ChatGroupContext, useChatGroupContext } from './ChatGroupContext';
import { chatifyApi } from '../../core';
import { CustomizeContext } from '../customize';
import { ChatViewCustomizeValue } from '../Chat';
import { AutoReplyCustomizeValue } from './AutoReply';
import { useCompact } from '../useCompact';

export interface ChatGroupCustomizeValue
  extends ChatViewCustomizeValue,
    AutoReplyCustomizeValue,
    CreateChatPanelCustomizeValue,
    ChatListPanelCustomizeValue,
    ChatGroupLayoutCustomizeValue {}

export interface ChatGroupPanelProps {
  /**
   * Class name to be applied to the chat group panel container.
   */
  className?: string;

  /**
   * Style to be applied to the chat group panel container.
   */
  style?: CSSProperties;

  /**
   * Localizer to be used for the chat group panel. If not provided, the default localizer will be used.
   */
  localizer?: Localizer;

  /**
   * Chat group panel components customization. If not provided, the default customization will be used.
   */
  customize?: ChatGroupCustomizeValue;

  /**
   * If true, the chat group panel will be displayed in compact mode.
   * If a number is provided, the chat group panel will be displayed in compact mode if its width is less than the provided number of pixels.
   *
   * @default 768
   */
  compact?: boolean | number;
}

interface ContentProps extends Pick<ChatGroupPanelProps, 'className' | 'style'> {
  containerRef: React.RefObject<HTMLDivElement>;
}

function Content(props: ContentProps) {
  const { className = '', style, containerRef } = props;
  const { data: chats } = chatifyApi.useGetChatListQuery();
  const { openChat, none, chatId, compact } = useChatGroupContext();
  const openFirstChatRef = useRef(false);

  useEffect(() => {
    if (chats && chatId && !chats.some((x) => x.id === chatId)) {
      chats.length ? openChat(chats[0].id) : none();
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chats, chatId]);

  useEffect(() => {
    if (chats && !openFirstChatRef.current && !compact) {
      openFirstChatRef.current = true;
      chats.length && openChat(chats[0].id);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chats, compact]);

  return (
    <div
      className={['wxchtf-chatify', 'wxchtf-chat-group', compact && '--compact', className]
        .filter(Boolean)
        .join(' ')}
      style={style}
      ref={containerRef}
    >
      <ChatGroupHeader />
      <ChatGroupBody />
      <ChatGroupFooter />
    </div>
  );
}

const DEFAULT_CUSTOMIZE: ChatGroupCustomizeValue = {
  ChatGroupHeader: null,
  ChatGroupFooter: null,
};

export const ChatGroupPanel: FC<ChatGroupPanelProps> = (props) => {
  const { localizer = defaultLocalizer, customize = DEFAULT_CUSTOMIZE, compact: compactProp } = props;
  const customizeValue = useMemo(() => Object.assign({}, DEFAULT_CUSTOMIZE, customize), [customize]);
  const [containerRef, compact] = useCompact(compactProp);

  return (
    <LocalizerContext.Provider value={localizer}>
      <CustomizeContext.Provider value={customizeValue as any}>
        <ChatGroupContext compact={compact ?? true}>
          <Content {...props} containerRef={containerRef} />
        </ChatGroupContext>
      </CustomizeContext.Provider>
    </LocalizerContext.Provider>
  );
};
