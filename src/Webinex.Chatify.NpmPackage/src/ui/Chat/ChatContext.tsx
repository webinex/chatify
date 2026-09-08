import React, { PropsWithChildren, useContext, useMemo, useState } from 'react';

export interface ChatContext {
  id: string;
  compact: boolean;
  showMembers: boolean;
  onShowMembers: (value: boolean) => void;
}

const ChatReactContext = React.createContext<ChatContext>(null!);

export function useChatContext() {
  return useContext(ChatReactContext);
}

// eslint-disable-next-line @typescript-eslint/no-redeclare
export function ChatContext(props: PropsWithChildren<{ id: string; compact: boolean }>) {
  const { id, children, compact } = props;
  const [showMembers, onShowMembers] = useState(false);

  const value = useMemo<ChatContext>(
    () => ({
      id,
      showMembers,
      onShowMembers,
      compact,
    }),
    [id, showMembers, compact],
  );

  return <ChatReactContext.Provider value={value}>{children}</ChatReactContext.Provider>;
}
