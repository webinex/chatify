import { Account } from '@/core';
import { AuditPanel } from './AuditPanel';

declare module '@webinex/chatify' {
  interface ChatifyType {
    Audit: typeof AuditPanel;
  }

  interface Localizer {
    audit: {
      searchPlaceholder: string;
      noChatFound: string;
      noInputBlurb: string;
      textSearchOption: (searchValue: string) => JSX.Element;
      memberSearchOption: (account: Account) => JSX.Element;
    };
  }
}
