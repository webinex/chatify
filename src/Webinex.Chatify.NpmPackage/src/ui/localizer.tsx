import dayjs from 'dayjs';
import React, { useContext } from 'react';

export interface Localizer {
  timestamp: (value: string) => string;
  system: {
    chatCreated: () => string;
    memberAdded: () => string;
    memberRemoved: () => string;
    chatNameChanged: (newName: string) => string;
  };
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
}

export const LocalizerContext = React.createContext<Localizer>(null!);

export function useLocalizer() {
  return useContext(LocalizerContext);
}

export const defaultLocalizer: Localizer = {
  timestamp: (value: string) => dayjs(value).format('HH:mm'),
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
    chatCreated: () => 'Chat created',
    memberAdded: () => 'New member added',
    memberRemoved: () => 'Member removed',
    chatNameChanged: (newName: string) => `Chat name changed to '${newName}'`,
  },

  input: {
    placeholder: () => 'Start typing (Ctrl/⌘Cmd + Enter to send) ...',
  },

  settings: {
    members: () => 'Members',
  },
};
