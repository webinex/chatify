import { useParams } from 'react-router-dom';
import { ChatPanel, ChatPanelCustomizeValue } from '../../src';
import React, { useMemo } from 'react';
import { Flippo } from '@webinex/flippo';
import { FLIPPO_AXIOS } from '../client';

export interface ChatPageProps {
  modal?: boolean;
}

function useCustomize(props: ChatPageProps): ChatPanelCustomizeValue {
  const { modal } = props;

  return useMemo<ChatPanelCustomizeValue>(() => {
    if (!modal) {
      return {};
    }

    return {
      ChatMembersButton: null,
      flippo: new Flippo({ axios: FLIPPO_AXIOS }),
    };
  }, [modal]);
}

export function ChatPage(props: ChatPageProps) {
  const { modal } = props;
  const { id } = useParams<'id'>();
  const customize = useCustomize(props);
  return <ChatPanel id={id!} className={modal ? 'modal' : undefined} customize={customize} />;
}
