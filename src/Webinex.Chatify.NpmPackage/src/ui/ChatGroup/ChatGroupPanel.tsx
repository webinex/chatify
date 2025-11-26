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

export interface ChatGroupCustomizeValue
  extends ChatViewCustomizeValue,
    AutoReplyCustomizeValue,
    CreateChatPanelCustomizeValue,
    ChatListPanelCustomizeValue,
    ChatGroupLayoutCustomizeValue {}

export interface ChatifyProps {
  className?: string;
  style?: CSSProperties;
  localizer?: Localizer;
  customize?: ChatGroupCustomizeValue;
}

function Content(props: Pick<ChatifyProps, 'className' | 'style'>) {
  const { className = '', style } = props;
  const { data: chats } = chatifyApi.useGetChatListQuery();
  const { openChat, none, chatId } = useChatGroupContext();
  const openFirstChatRef = useRef(false);

  useEffect(() => {
    if (chats && !chats.some((x) => x.id === chatId)) {
      chats.length ? openChat(chats[0].id) : none();
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chats, chatId]);

  useEffect(() => {
    if (chats && !openFirstChatRef.current) {
      openFirstChatRef.current = true;
      chats.length && openChat(chats[0].id);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chats]);

  return (
    <div className={className + ' wxchtf-chatify wxchtf-chat-group'} style={style}>
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

export const ChatGroupPanel: FC<ChatifyProps> = (props) => {
  const { localizer = defaultLocalizer, customize = DEFAULT_CUSTOMIZE } = props;
  const customizeValue = useMemo(() => Object.assign({}, DEFAULT_CUSTOMIZE, customize), [customize]);

  return (
    <LocalizerContext.Provider value={localizer}>
      <CustomizeContext.Provider value={customizeValue as any}>
        <ChatGroupContext>
          <Content {...props} />
        </ChatGroupContext>
      </CustomizeContext.Provider>
    </LocalizerContext.Provider>
  );
};
