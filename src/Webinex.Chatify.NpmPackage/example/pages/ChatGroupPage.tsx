import React from 'react';
import { Chatify, ChatGroupCustomizeValue, ChatHeaderActions, useConversation } from '../../src';
import { Button } from 'antd';
import { Flippo } from '@webinex/flippo';
import { FLIPPO_AXIOS } from '../client';

const CUSTOMIZE: ChatGroupCustomizeValue = {
  ChatHeaderActions: () => {
    const { id, name } = useConversation();

    const onOpenInNewWindow = () => {
      window.open(
        `/chat/${id}?modal`,
        name,
        'width=400,height=600,titlebar=no,toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=no,directories=no',
      );
    };

    return (
      <div style={{ display: 'inline-flex', alignItems: 'center', gap: 20 }}>
        <Button type="link" onClick={onOpenInNewWindow}>
          Open in new window
        </Button>
        <ChatHeaderActions.Component />
      </div>
    );
  },

  flippo: new Flippo({ axios: FLIPPO_AXIOS }),
};

export function ChatGroupPage() {
  return <Chatify.ChatGroup customize={CUSTOMIZE} />;
}
