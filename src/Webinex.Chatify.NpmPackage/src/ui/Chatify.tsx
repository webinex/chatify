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

export interface ChatifyCustomizeValue {
  Avatar?: typeof Avatar.Component | null;
  Header?: typeof Header.Component | null;
  Footer?: typeof Footer.Component | null;
  AddChatButton?: typeof AddChatButton.Component | null;
  ChatHeaderMembers?: typeof ChatHeaderMembers.Component | null;
  ChatSettingsButton?: typeof ChatSettingsButton.Component | null;
  ChatName?: typeof ChatName.Component | null;
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
