import { CSSProperties, FC } from 'react';
import { useReadMonitor } from '../core';
import { Localizer, LocalizerContext, defaultLocalizer } from './localizer';
import { Avatar } from './Avatar';
import { CustomizeProvider } from './customize';
import { ChatHeaderMembers } from './ChatPanel/ChatHeaderMembers';
import { ChatMembersButton } from './ChatPanel/ChatMembersButton';
import { AddChatButton } from './AddChatButton';
import { ChatName } from './ChatName';

import { Aside, Main, Body, Footer, Header } from './Layout';

import {
  ChatListItemBox,
  ChatListItemLastMessageContent,
  ChatListItemLastMessageAuthor,
  ChatListItemLastMessage,
  ChatListItemUnreadCount,
  ChatListItemName,
} from './ChatList';

import {
  SendingMessageBox,
  SendingMessageContent,
  SendingMessageInfoBox,
  SendingMessageText,
} from './SendingMessage';

import {
  MessageBox,
  MessageRow,
  MessageContent,
  MessageText,
  MessageAuthor,
  MessageInfoBox,
} from './Message';

import { MessageReadObserver } from './MessageReadObserver';
import { Icon } from './Icon';
import { SystemMessageRow } from './SystemMessage/SystemMessageRow';
import { ChatList } from './ChatList';
import { ChatBody, ChatHeader, ChatHeaderName, ChatPanel } from './ChatPanel';
import { InputBox, InputFilesBox, InputSubmitButtonBox, InputTextBox } from './InputBox';
import { SystemMessageBox } from './SystemMessage';
import {
  CreateChatForm,
  CreateChatMemberInput,
  CreateChatNameInput,
  CreateChatPanel,
} from './CreateChatPanel';

export interface ChatifyCustomizeValue {
  // ./ChatList
  ChatList?: typeof ChatList.Component | null;
  ChatListItemBox?: typeof ChatListItemBox.Component | null;
  ChatListItemName?: typeof ChatListItemName.Component | null;
  ChatListItemUnreadCount?: typeof ChatListItemUnreadCount.Component | null;
  ChatListItemLastMessage?: typeof ChatListItemLastMessage.Component | null;
  ChatListItemLastMessageAuthor?: typeof ChatListItemLastMessageAuthor.Component | null;
  ChatListItemLastMessageContent?: typeof ChatListItemLastMessageContent.Component | null;

  // ./Layout
  Header?: typeof Header.Component | null;
  Footer?: typeof Footer.Component | null;
  Body?: typeof Body.Component | null;
  Main?: typeof Main.Component | null;
  Aside?: typeof Aside.Component | null;

  // ./Message
  MessageAuthor?: typeof MessageAuthor.Component | null;
  MessageBox?: typeof MessageBox.Component | null;
  MessageContent?: typeof MessageContent.Component | null;
  MessageInfoBox?: typeof MessageInfoBox.Component | null;
  MessageRow?: typeof MessageRow.Component | null;
  MessageText?: typeof MessageText.Component | null;

  // ./SendingMessage
  SendingMessageBox?: typeof SendingMessageBox.Component | null;
  SendingMessageContent?: typeof SendingMessageContent.Component | null;
  SendingMessageInfoBox?: typeof SendingMessageInfoBox.Component | null;
  SendingMessageText?: typeof SendingMessageText.Component | null;

  // ./ChatPanel
  ChatBody?: typeof ChatBody.Component | null;
  ChatHeader?: typeof ChatHeader.Component | null;
  ChatHeaderMembers?: typeof ChatHeaderMembers.Component | null;
  ChatHeaderName?: typeof ChatHeaderName.Component | null;
  ChatPanel?: typeof ChatPanel.Component | null;
  ChatMembersButton?: typeof ChatMembersButton.Component | null;

  // ./InputBox
  InputBox?: typeof InputBox.Component | null;
  InputFilesBox?: typeof InputFilesBox.Component | null;
  InputSubmitButtonBox?: typeof InputSubmitButtonBox.Component | null;
  InputTextBox?: typeof InputTextBox.Component | null;

  // ./SystemMessage
  SystemMessageBox?: typeof SystemMessageBox.Component | null;
  SystemMessageRow?: typeof SystemMessageRow.Component | null;

  // ./CreateChatPanel
  CreateChatForm?: typeof CreateChatForm.Component | null;
  CreateChatMemberInput?: typeof CreateChatMemberInput.Component | null;
  CreateChatNameInput?: typeof CreateChatNameInput.Component | null;
  CreateChatPanel?: typeof CreateChatPanel.Component | null;

  MessageReadObserver?: typeof MessageReadObserver.Component | null;
  Avatar?: typeof Avatar.Component | null;
  AddChatButton?: typeof AddChatButton.Component | null;
  ChatName?: typeof ChatName.Component | null;
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
      <Body />
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
