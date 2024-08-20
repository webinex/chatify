import React from 'react';

export interface ThreadContextValue {
  id: string;
  watch: boolean;
  onWatch: (value: boolean) => void;
}

export const ThreadContext = React.createContext<ThreadContextValue>(null!);

export function useThreadContext() {
  return React.useContext(ThreadContext);
}
