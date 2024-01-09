import React, { PropsWithChildren, useContext } from 'react';
import { ReducerProvider } from './core/reducer';
import { ChatifyClient } from './core/client';
import { useSignalRMonitor } from './core/useSignalRMonitor';

const MeContext = React.createContext<string>(null!);
const ClientContext = React.createContext<ChatifyClient>(null!);

export function useClient() {
  return useContext(ClientContext);
}

export function useMe() {
  return useContext(MeContext);
}

export interface ChatifyContextValue {
  me: string;
  client: ChatifyClient;
}

function ChatifyContextWrapper(props: PropsWithChildren<{}>) {
  const { children } = props;
  const me = useMe();
  const client = useClient();
  useSignalRMonitor(me, client);

  return <>{children}</>;
}

export function ChatifyContext({ value, children }: PropsWithChildren<{ value: ChatifyContextValue }>) {
  return (
    <ClientContext.Provider value={value.client}>
      <MeContext.Provider value={value.me}>
        <ReducerProvider>
          <ChatifyContextWrapper>{children}</ChatifyContextWrapper>
        </ReducerProvider>
      </MeContext.Provider>
    </ClientContext.Provider>
  );
}
