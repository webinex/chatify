import dayjs from 'dayjs';
import React, { useContext } from 'react';
import { SystemMessageText } from '../core';

export interface Localizer {
  isToday: (value: string) => boolean;
  timestamp: (value: string) => string;
  dateTime: (value: string) => string;
  system: Record<SystemMessageText, (key: SystemMessageText, payload: any | null) => string>;
  message: {
    read: () => React.ReactNode;
    reading: () => React.ReactNode;
    sending: () => React.ReactNode;
  };
  addChat: {
    title: () => React.ReactNode;
    name: {
      label: () => React.ReactNode;
      required: () => string;
      max250: () => string;
    };
    members: {
      label: () => React.ReactNode;
      required: () => string;
    };
    submitBtn: () => React.ReactNode;
  };

  input: {
    placeholder: () => string;
  };

  settings: {
    members: () => string;
  };

  size: {
    bytes: (value: number) => string;
    kb: (value: number) => string;
    mb: (value: number) => string;
  };
}

export const LocalizerContext = React.createContext<Localizer>(null!);

export function useLocalizer() {
  return useContext(LocalizerContext);
}

export const defaultLocalizer: Localizer = {
  isToday: (value: string) => dayjs(value).isSame(dayjs(), 'day'),
  timestamp: (value: string) => dayjs(value).format('HH:mm'),
  dateTime: (value: string) => dayjs(value).format('DD/MM/YYYY HH:mm'),
  message: {
    read: () => 'Read',
    reading: () => 'Reading...',
    sending: () => 'Sending...',
  },
  addChat: {
    title: () => 'Add chat',
    name: {
      label: () => 'Name',
      required: () => `Name is required`,
      max250: () => 'Max 250 characters allowed',
    },
    members: {
      label: () => 'Members',
      required: () => 'Members is required',
    },
    submitBtn: () => 'Submit',
  },
  system: {
    'chatify://chat-created': () => 'Chat created',
    'chatify://member-added': () => 'New member added',
    'chatify://member-removed': () => 'Member removed',
    'chatify://chat-name-changed': (_, { newName }: { newName: string }) =>
      `Chat name changed to '${newName}'`,
  },

  input: {
    placeholder: () => 'Start typing (Ctrl/⌘Cmd + Enter to send) ...',
  },

  settings: {
    members: () => 'Members',
  },

  size: {
    bytes: (value: number) =>
      `${value.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 2 })} bytes`,
    kb: (value: number) =>
      `${value.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 2 })} KB`,
    mb: (value: number) =>
      `${value.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 2 })} MB`,
  },
};
