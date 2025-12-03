import React, { PropsWithChildren, useContext, useMemo, useState } from 'react';

export interface ChatGroupContext {
  view: 'chat' | 'new-chat' | 'auto-reply' | null;
  chatId: string | null;
  showMembers: boolean;

  none(): void;
  openChat(chatId: string): void;
  toggleNewChat(): void;
  toggleMembers(): void;
  toggleAutoReply(): void;
}

const ChatGroupReactContext = React.createContext<ChatGroupContext>(null!);

export function useChatGroupContext() {
  return useContext(ChatGroupReactContext);
}

// eslint-disable-next-line @typescript-eslint/no-redeclare
export function ChatGroupContext(props: PropsWithChildren<{}>) {
  const { children } = props;
  const [state, setState] = useState<Pick<ChatGroupContext, 'view' | 'chatId' | 'showMembers'>>({
    chatId: null,
    view: null,
    showMembers: false,
  });

  const value = useMemo<ChatGroupContext>(
    () => ({
      ...state,
      openChat: (chatId: string) => setState((prev) => ({ ...prev, view: 'chat', chatId })),
      none: () => setState((prev) => ({ ...prev, view: null, chatId: null })),
      toggleMembers: () => setState((prev) => ({ ...prev, showMembers: !prev.showMembers })),
      toggleNewChat: () =>
        setState((prev) => {
          if (prev.view === 'new-chat' && !prev.chatId) {
            return { ...prev, view: null };
          } else if (prev.view === 'new-chat' && prev.chatId) {
            return { ...prev, view: 'chat' };
          } else {
            return { ...prev, view: 'new-chat' };
          }
        }),
      toggleAutoReply: () =>
        setState((prev) => {
          if (prev.view === 'auto-reply' && prev.chatId) {
            return { ...prev, view: 'chat' };
          } else if (prev.view === 'auto-reply' && !prev.chatId) {
            return { ...prev, view: 'new-chat' };
          } else {
            return { ...prev, view: 'auto-reply' };
          }
        }),
    }),
    [state],
  );

  return <ChatGroupReactContext.Provider value={value}>{children}</ChatGroupReactContext.Provider>;
}
