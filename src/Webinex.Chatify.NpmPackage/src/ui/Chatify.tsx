import { CSSProperties, FC } from 'react';
import { useReadMonitor } from '../core';
import { Aside } from './Aside';
import { Main } from './Main';
import { Localizer, LocalizerContext, defaultLocalizer } from './localizer';
import { Avatar } from './Avatar';
import { CustomizeProvider } from '../util';
import { ChatHeaderMembers } from './ChatHeaderMembers';
import { ChatSettingsButton } from './ChatHeaderSettingsButton';
import { Header } from './Header';
import { AddChatButton } from './AddChatButton';
import { Footer } from './Footer';
import { ChatName } from './ChatName';
import {
  ChatListItemBox,
  ChatListItemLastMessage,
  ChatListItemLastMessageAuthor,
  ChatListItemLastMessageContent,
  ChatListItemName,
  ChatListItemUnreadCount,
} from './ChatListItem';
import {
  SendingMessage,
  SendingMessageContent,
  SendingMessageInfoBox,
  SendingMessageText,
} from './SendingMessage';
import {
  MessageAuthor,
  MessageContent,
  MessageBox,
  MessageInfoBox,
  MessageReadObserver,
  MessageRow,
  MessageText,
} from './Message';
import { Icon } from './Icon';
import { SystemMessage } from './SystemMessage';

export interface ChatifyCustomizeValue {
  Avatar?: typeof Avatar.Component | null;
  Header?: typeof Header.Component | null;
  Footer?: typeof Footer.Component | null;
  AddChatButton?: typeof AddChatButton.Component | null;
  ChatHeaderMembers?: typeof ChatHeaderMembers.Component | null;
  ChatSettingsButton?: typeof ChatSettingsButton.Component | null;
  ChatName?: typeof ChatName.Component | null;
  ChatListItemBox?: typeof ChatListItemBox.Component | null;
  ChatListItemName?: typeof ChatListItemName.Component | null;
  ChatListItemUnreadCount?: typeof ChatListItemUnreadCount.Component | null;
  ChatListItemLastMessage?: typeof ChatListItemLastMessage.Component | null;
  ChatListItemLastMessageAuthor?: typeof ChatListItemLastMessageAuthor.Component | null;
  ChatListItemLastMessageContent?: typeof ChatListItemLastMessageContent.Component | null;
  SystemMessage?: typeof SystemMessage.Component | null;
  SendingMessage?: typeof SendingMessage.Component | null;
  SendingMessageBody?: typeof SendingMessageContent.Component | null;
  SendingMessageInfoBox?: typeof SendingMessageInfoBox.Component | null;
  SendingMessageText?: typeof SendingMessageText.Component | null;
  MessageBox?: typeof MessageBox.Component | null;
  MessageReadObserver?: typeof MessageReadObserver.Component | null;
  MessageRow?: typeof MessageRow.Component | null;
  MessageContent?: typeof MessageContent.Component | null;
  MessageInfoBox?: typeof MessageInfoBox.Component | null;
  MessageText?: typeof MessageText.Component | null;
  MessageAuthor?: typeof MessageAuthor.Component | null;
  Icon?: typeof Icon.Component | null;
}

export interface ChatifyProps {
  className?: string;
  style?: CSSProperties;
  localizer?: Localizer;
  customize?: ChatifyCustomizeValue;
}

function Content(props: Pick<ChatifyProps, 'className' | 'style'>) {
  const { className = '', style } = props;
  useReadMonitor();

  return (
    <div className={className + ' wxchtf-chatify'} style={style}>
      <Header />
      <div className="wxchtf-body">
        <Aside />
        <Main />
      </div>
      <Footer />
    </div>
  );
}

const EMPTY_CUSTOMIZE = {};

export const Chatify: FC<ChatifyProps> = (props) => {
  const { localizer = defaultLocalizer, customize = EMPTY_CUSTOMIZE } = props;

  return (
    <LocalizerContext.Provider value={localizer}>
      <CustomizeProvider value={customize}>
        <Content {...props} />
      </CustomizeProvider>
    </LocalizerContext.Provider>
  );
};

Chatify.displayName = 'Chatify';
