import React, { PropsWithChildren, useContext, useMemo, useState } from 'react';

export interface ChatContext {
  id: string;
  showMembers: boolean;
  onShowMembers: (value: boolean) => void;
}

const ChatReactContext = React.createContext<ChatContext>(null!);

export function useChatContext() {
  return useContext(ChatReactContext);
}

// eslint-disable-next-line @typescript-eslint/no-redeclare
export function ChatContext(props: PropsWithChildren<{ id: string }>) {
  const { id, children } = props;
  const [showMembers, onShowMembers] = useState(false);

  const value = useMemo<ChatContext>(
    () => ({
      id,
      showMembers,
      onShowMembers,
    }),
    [id, showMembers],
  );

  return <ChatReactContext.Provider value={value}>{children}</ChatReactContext.Provider>;
}
